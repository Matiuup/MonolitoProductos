using CapaDatos.Modelos;
using CapaDatos.Repositorios;
using System;
using System.Data;
using System.Text.RegularExpressions;
using MongoDB.Bson;

namespace CapaNegocio.Seguridad
{
    public class CN_Usuario
    {
        private readonly UsuarioRepository _repo = new UsuarioRepository();
        private readonly CN_Seguridad _seg = new CN_Seguridad();

        public class RespuestaLogin
        {
            public int StatusCode { get; set; }
            public string Mensaje { get; set; }
            public string UsuId { get; set; }
            public string Nombres { get; set; }
            public string Apellidos { get; set; }
            public string TipoUsuario { get; set; }
            public string TipoUsuarioId { get; set; }
            // Para depuración
            public string DebugInfo { get; set; }
        }

        public RespuestaLogin Login(string usuario, string clave)
        {
            var user = _repo.BuscarPorNickOCorreo(usuario);
            if (user == null)
                return new RespuestaLogin { StatusCode = 0, Mensaje = "Usuario no existe." };

            // Verificar estado de bloqueo por fecha
            if (user.usu_estado == "B" && user.usu_fecha_bloqueo.HasValue)
            {
                if ((DateTime.Now - user.usu_fecha_bloqueo.Value).TotalHours >= 24)
                {
                    // Desbloquear automáticamente después de 24 horas
                    user.usu_estado = "A";
                    user.usu_intentos = 0;
                    user.usu_fecha_bloqueo = null;
                    _repo.Actualizar(user);
                }
                else
                {
                    return new RespuestaLogin { StatusCode = 0, Mensaje = "Usuario bloqueado. Intente en 24 horas." };
                }
            }

            if (user.usu_estado != "A")
                return new RespuestaLogin { StatusCode = 0, Mensaje = "Usuario inactivo." };

            // Reiniciar intentos si cambió el día
            if (user.usu_fecha_ultimo_intento?.Date != DateTime.Today)
            {
                user.usu_intentos = 0;
                user.usu_fecha_ultimo_intento = DateTime.Today;
                _repo.Actualizar(user);
            }

            // Verificar si ya está bloqueado por intentos
            if (user.usu_intentos >= 3)
            {
                user.usu_estado = "B";
                user.usu_fecha_bloqueo = DateTime.Now;
                _repo.Actualizar(user);
                return new RespuestaLogin { StatusCode = 0, Mensaje = "Usuario bloqueado por exceder intentos." };
            }

            // ***********************
            // COMPARACIÓN DE CONTRASEÑA
            // ***********************
            string hashIngresado = HashHelper.HashPassword(clave);
            bool passwordCorrecto = (hashIngresado == user.usu_contraseña);

            // Depuración: incluir información en la respuesta
            string debugInfo = $"Hash ingresado: {hashIngresado.Substring(0, 10)}... | Hash BD: {user.usu_contraseña?.Substring(0, 10)}...";

            if (passwordCorrecto)
            {
                // Login exitoso: resetear intentos
                user.usu_intentos = 0;
                user.usu_fecha_ultimo_intento = DateTime.Today;
                _repo.Actualizar(user);
                _repo.RegistrarIntento(user.Id, true);

                var tipo = _repo.ObtenerTipoUsuario(user.TipoUsuarioId);
                return new RespuestaLogin
                {
                    StatusCode = 1,
                    Mensaje = "Login exitoso",
                    UsuId = user.Id,
                    Nombres = user.usu_nombres,
                    Apellidos = user.usu_apellidos,
                    TipoUsuario = tipo?.tusu_nombre,
                    TipoUsuarioId = user.TipoUsuarioId,
                    DebugInfo = debugInfo
                };
            }
            else
            {
                // Incrementar intentos fallidos
                user.usu_intentos++;
                user.usu_fecha_ultimo_intento = DateTime.Today;
                _repo.Actualizar(user);
                _repo.RegistrarIntento(user.Id, false);

                int restantes = 3 - user.usu_intentos;
                if (restantes <= 0)
                {
                    user.usu_estado = "B";
                    user.usu_fecha_bloqueo = DateTime.Now;
                    _repo.Actualizar(user);
                    return new RespuestaLogin { StatusCode = 0, Mensaje = "Usuario bloqueado por exceder intentos.", DebugInfo = debugInfo };
                }
                return new RespuestaLogin { StatusCode = 0, Mensaje = $"Contraseña incorrecta. Intentos restantes: {restantes}", DebugInfo = debugInfo };
            }
        }

        public string RegistrarUsuario(string cedula, string nombres, string apellidos,
            string direccion, string celular, string correo, DateTime? fechaCumple,
            string nick, string password, string tipoUsuarioId = null)
        {
            if (string.IsNullOrEmpty(tipoUsuarioId) || !ObjectId.TryParse(tipoUsuarioId, out _))
            {
                var tipo = _repo.ObtenerTipoUsuarioPorNombre("Usuario");
                tipoUsuarioId = tipo?.Id;
            }

            var user = new Usuario
            {
                usu_cedula = cedula,
                usu_nombres = nombres,
                usu_apellidos = apellidos,
                usu_direccion = direccion,
                usu_celular = celular,
                usu_correo = correo,
                usu_fecha_cumple = fechaCumple,
                usu_nick = nick,
                usu_contraseña = HashHelper.HashPassword(password),
                TipoUsuarioId = tipoUsuarioId
            };
            return _repo.Insertar(user);
        }

        public string ObtenerIdPorNick(string nick)
        {
            var user = _repo.BuscarPorNickOCorreo(nick);
            return user?.Id;
        }

        public Usuario ObtenerUsuarioPorId(string id) => _repo.ObtenerPorId(id);

        public DataTable ObtenerUsuarioPorIdDT(string id) => _repo.ObtenerUsuarioDataTable(id);

        public DataTable ObtenerUsuarioPorCorreoDT(string correo)
        {
            var user = _repo.ObtenerPorCorreo(correo);
            DataTable dt = new DataTable();
            dt.Columns.Add("usu_id");
            if (user != null) dt.Rows.Add(user.Id);
            return dt;
        }

        public void CambiarPassword(string usuId, string nuevaClave)
        {
            var user = _repo.ObtenerPorId(usuId);
            if (user != null)
            {
                user.usu_contraseña = HashHelper.HashPassword(nuevaClave);
                user.usu_token_recuperacion = null;
                user.usu_token_expiracion = null;
                _repo.Actualizar(user);
            }
        }

        public bool ValidarCorreo(string correo) => Regex.IsMatch(correo ?? "", @"^[a-zA-Z0-9._%+\-]+@[a-zA-Z0-9.\-]+\.[a-zA-Z]{2,}$");

        public bool ValidarComplejidadPassword(string password) =>
            password?.Length >= 8 &&
            Regex.IsMatch(password, @"[A-Z]") &&
            Regex.IsMatch(password, @"[a-z]") &&
            Regex.IsMatch(password, @"[0-9]") &&
            Regex.IsMatch(password, @"[!@#$%^&*()_+\-=\[\]{};':""\\|,.<>\/?]");
    }
}