using CapaNegocio.Seguridad;
using System;
using System.IO;
using System.Web.UI;

namespace SistemaProductos
{
    public partial class Registro : System.Web.UI.Page
    {
        // ── Instancias de capa de negocio ───────────────────────────────
        private readonly CN_Usuario usuarioBL = new CN_Usuario();
        private readonly CN_Imagen imagenBL = new CN_Imagen();

        // ── Constantes ──────────────────────────────────────────────────
        private const int MAX_MB = 4;
        private const int MAX_BYTES = MAX_MB * 1024 * 1024;
        private static readonly string[] EXTS = { "jpg", "jpeg", "png" };

        // ────────────────────────────────────────────────────────────────
        // PAGE LOAD
        // ────────────────────────────────────────────────────────────────
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Limitar rango de fecha de nacimiento
                txtFechaCumple.Attributes["min"] = DateTime.Now.AddYears(-70).ToString("yyyy-MM-dd");
                txtFechaCumple.Attributes["max"] = DateTime.Now.AddYears(-5).ToString("yyyy-MM-dd");
            }
            else
            {
                // Restaurar preview de imagen si existe en sesión
                RestaurarPreviewImagen();
            }
        }

        // ────────────────────────────────────────────────────────────────
        // PREVISUALIZAR FOTO
        // ────────────────────────────────────────────────────────────────
        protected void btnPrevisualizar_Click(object sender, EventArgs e)
        {
            if (!fuFoto.HasFile)
            {
                Mensaje("Selecciona una imagen primero.", "msg-box msg-warning");
                return;
            }

            string ext = Ext(fuFoto.FileName);
            if (!EsExtValida(ext))
            {
                Mensaje("Solo se permiten PNG o JPG.", "msg-box msg-danger");
                return;
            }

            if (fuFoto.FileBytes.Length > MAX_BYTES)
            {
                Mensaje($"La imagen supera los {MAX_MB}MB.", "msg-box msg-danger");
                return;
            }

            byte[] bytes = fuFoto.FileBytes;

            // Guardar en sesión para que sobreviva al postback
            Session["ImgBytes"] = bytes;
            Session["ImgExt"] = ext;
            Session["ImgNombre"] = fuFoto.FileName;

            // Mostrar preview como base64
            imgPreview.ImageUrl = $"data:image/{ext};base64,{Convert.ToBase64String(bytes)}";
            imgPreview.Visible = true;
            iconPlaceholder.Visible = false;

            Mensaje("Imagen previsualizada correctamente.", "msg-box msg-success");
        }

        // ────────────────────────────────────────────────────────────────
        // REGISTRAR USUARIO
        // ────────────────────────────────────────────────────────────────
        protected void btnRegistrar_Click(object sender, EventArgs e)
        {
            try
            {
                // Leer campos de texto
                string nombres = txtNombres.Text.Trim();
                string apellidos = txtApellidos.Text.Trim();
                string nick = txtNick.Text.Trim();
                string correo = txtCorreo.Text.Trim();
                string cedula = txtCedula.Text.Trim();
                string celular = txtCelular.Text.Trim();
                string direccion = txtDireccion.Text.Trim();

                // Leer contraseñas desde campos ocultos (sobreviven al togglePassword JS)
                // Si el oculto viene vacío, intentar con el UniqueID del control
                string clave = LeerClave("hdnClave", txtClave.UniqueID);
                string confirmar = LeerClave("hdnConfirmar", txtConfirmarClave.UniqueID);

                // ── Validaciones ────────────────────────────────────────
                if (!Requeridos(nombres, apellidos, nick, clave, confirmar)) return;
                if (!DosPalabras(nombres, "nombres")) return;
                if (!DosPalabras(apellidos, "apellidos")) return;

                DateTime? fecha = ParseFecha();
                if (fecha == null && !string.IsNullOrWhiteSpace(txtFechaCumple.Text)) return;

                if (clave != confirmar)
                {
                    Mensaje("Las contraseñas no coinciden.", "msg-box msg-danger");
                    return;
                }

                if (!usuarioBL.ValidarComplejidadPassword(clave))
                {
                    Mensaje("La contraseña debe tener mín. 8 caracteres, mayúscula, minúscula, número y símbolo.",
                            "msg-box msg-danger");
                    return;
                }

                if (!string.IsNullOrEmpty(correo) && !usuarioBL.ValidarCorreo(correo))
                {
                    Mensaje("El formato del correo no es válido.", "msg-box msg-danger");
                    return;
                }

                // ── Registrar en MongoDB ─────────────────────────────────
                string usuarioId = usuarioBL.RegistrarUsuario(
                    cedula, nombres, apellidos, direccion,
                    celular, correo, fecha, nick, clave);

                if (string.IsNullOrEmpty(usuarioId))
                {
                    Mensaje("No se pudo crear la cuenta. Intenta nuevamente.", "msg-box msg-danger");
                    return;
                }

                // ── Guardar imagen de perfil (no bloquea el registro) ───
                try { GuardarFotoPerfil(usuarioId); }
                catch { /* imagen opcional, no interrumpir */ }

                LimpiarSesion();
                Mensaje("¡Cuenta creada con éxito! Redirigiendo al login...", "msg-box msg-success");
                Response.AddHeader("REFRESH", "2;URL=Login.aspx");
            }
            catch (Exception ex)
            {
                Mensaje("Error: " + ex.Message, "msg-box msg-danger");
            }
        }

        // ────────────────────────────────────────────────────────────────
        // HELPERS PRIVADOS
        // ────────────────────────────────────────────────────────────────

        /// <summary>
        /// Lee la contraseña primero del campo oculto, luego del UniqueID del control.
        /// Esto garantiza que sobreviva aunque el JS haya cambiado type="text".
        /// </summary>
        private string LeerClave(string hdnName, string uniqueId)
        {
            string val = (Request.Form[hdnName] ?? "").Trim();
            if (string.IsNullOrEmpty(val))
                val = (Request.Form[uniqueId] ?? "").Trim();
            return val;
        }

        private bool Requeridos(string nombres, string apellidos, string nick,
                                 string clave, string confirmar)
        {
            if (string.IsNullOrEmpty(nombres) || string.IsNullOrEmpty(apellidos) ||
                string.IsNullOrEmpty(nick) || string.IsNullOrEmpty(clave) ||
                string.IsNullOrEmpty(confirmar))
            {
                Mensaje("Los campos marcados con * son obligatorios.", "msg-box msg-danger");
                return false;
            }
            return true;
        }

        private bool DosPalabras(string valor, string campo)
        {
            var arr = valor.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (arr.Length < 2)
            {
                Mensaje($"Ingresa al menos dos {campo}.", "msg-box msg-danger");
                return false;
            }
            return true;
        }

        private DateTime? ParseFecha()
        {
            if (string.IsNullOrWhiteSpace(txtFechaCumple.Text)) return null;

            if (!DateTime.TryParse(txtFechaCumple.Text, out DateTime f))
            {
                Mensaje("Fecha de nacimiento no válida.", "msg-box msg-danger");
                return null;
            }

            var hoy = DateTime.Now.Date;
            if (f > hoy) { Mensaje("La fecha no puede ser futura.", "msg-box msg-danger"); return null; }
            if (f < hoy.AddYears(-70)) { Mensaje("Fecha demasiado antigua (máx. 70 años).", "msg-box msg-danger"); return null; }
            if (f > hoy.AddYears(-5)) { Mensaje("Debes tener al menos 5 años.", "msg-box msg-danger"); return null; }

            return f;
        }

        private void GuardarFotoPerfil(string usuarioId)
        {
            byte[] bytes = null;
            string ext = null;
            string nombre = null;

            if (Session["ImgBytes"] != null)
            {
                bytes = (byte[])Session["ImgBytes"];
                ext = Session["ImgExt"]?.ToString();
                nombre = Session["ImgNombre"]?.ToString();
            }
            else if (fuFoto.HasFile)
            {
                ext = Ext(fuFoto.FileName);
                bytes = fuFoto.FileBytes;
                nombre = fuFoto.FileName;
            }

            if (bytes != null && bytes.Length <= MAX_BYTES && EsExtValida(ext))
                imagenBL.GuardarImagen(usuarioId, bytes, ext, nombre);
        }

        private void RestaurarPreviewImagen()
        {
            if (Session["ImgBytes"] == null || Session["ImgExt"] == null) return;

            byte[] bytes = (byte[])Session["ImgBytes"];
            string ext = Session["ImgExt"].ToString();

            imgPreview.ImageUrl = $"data:image/{ext};base64,{Convert.ToBase64String(bytes)}";
            imgPreview.Visible = true;
            iconPlaceholder.Visible = false;
        }

        private void LimpiarSesion()
        {
            Session.Remove("ImgBytes");
            Session.Remove("ImgExt");
            Session.Remove("ImgNombre");
        }

        private void Mensaje(string texto, string css)
        {
            lblMensaje.Text = texto;
            lblMensaje.CssClass = css;
            lblMensaje.Visible = true;
        }

        private string Ext(string fileName) =>
            Path.GetExtension(fileName).ToLower().TrimStart('.');

        private bool EsExtValida(string ext) =>
            Array.Exists(EXTS, x => x.Equals(ext, StringComparison.OrdinalIgnoreCase));
    }
}
