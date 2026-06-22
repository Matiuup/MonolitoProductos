using CapaDatos.Modelos;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace CapaDatos.Repositorios
{
    public class UsuarioRepository
    {
        private readonly IMongoCollection<Usuario> _usuarios;
        private readonly IMongoCollection<TipoUsuario> _tipos;
        private readonly IMongoCollection<IntentosAcceso> _intentos;

        public UsuarioRepository()
        {
            var db = ConexionMongo.ObtenerBaseDatos();
            _usuarios = db.GetCollection<Usuario>("usuarios");
            _tipos = db.GetCollection<TipoUsuario>("tipos_usuario");
            _intentos = db.GetCollection<IntentosAcceso>("intentos_acceso");
        }

        public Usuario BuscarPorNickOCorreo(string usuario)
        {
            return _usuarios.Find(u => u.usu_nick == usuario || u.usu_correo == usuario).FirstOrDefault();
        }

        public Usuario ObtenerPorId(string id)
        {
            if (!ObjectId.TryParse(id, out _)) return null;
            return _usuarios.Find(u => u.Id == id).FirstOrDefault();
        }

        public Usuario ObtenerPorCorreo(string correo)
        {
            return _usuarios.Find(u => u.usu_correo == correo).FirstOrDefault();
        }

        public string Insertar(Usuario user)
        {
            _usuarios.InsertOne(user);
            return user.Id;
        }

        public void Actualizar(Usuario user) => _usuarios.ReplaceOne(u => u.Id == user.Id, user);

        public void RegistrarIntento(string usuarioId, bool exitoso, string ip = null)
        {
            var intento = new IntentosAcceso
            {
                int_usu_id = usuarioId,
                int_ip = ip,
                int_exitoso = exitoso
            };
            _intentos.InsertOne(intento);
        }

        public TipoUsuario ObtenerTipoUsuario(string tipoId)
        {
            if (!ObjectId.TryParse(tipoId, out _)) return null;
            return _tipos.Find(t => t.Id == tipoId).FirstOrDefault();
        }

        public TipoUsuario ObtenerTipoUsuarioPorNombre(string nombre)
        {
            return _tipos.Find(t => t.tusu_nombre == nombre && t.tusu_estado == "A").FirstOrDefault();
        }

        // Para compatibilidad con método antiguo que esperaba DataTable
        public DataTable ObtenerUsuarioDataTable(string id)
        {
            var user = ObtenerPorId(id);
            DataTable dt = new DataTable();
            dt.Columns.Add("usu_id");
            dt.Columns.Add("usu_nombres");
            dt.Columns.Add("usu_apellidos");
            dt.Columns.Add("usu_correo");
            dt.Columns.Add("usu_celular");
            dt.Columns.Add("usu_nick");
            if (user != null)
            {
                dt.Rows.Add(user.Id, user.usu_nombres, user.usu_apellidos, user.usu_correo, user.usu_celular, user.usu_nick);
            }
            return dt;
        }
    }
}