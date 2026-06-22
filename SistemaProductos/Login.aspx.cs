using CapaNegocio.Seguridad;
using System;
using System.Web;
using System.Web.Services;
using System.Web.UI;

namespace SistemaProductos
{
    public partial class Login : System.Web.UI.Page
    {
        CN_Usuario usuarioBL = new CN_Usuario();
        CN_Seguridad seguridadBL = new CN_Seguridad();

        protected void Page_Load(object sender, EventArgs e)
        {
            // Ya no usamos Session["MostrarOTP"] para alternar los paneles,
            // porque ahora lo hacemos directamente en el evento del botón.
            if (!IsPostBack && Request.Cookies["UsuarioRecordado"] != null)
            {
                txtUsuario.Text = Request.Cookies["UsuarioRecordado"].Value;
                chkRecordar.Checked = true;
            }
        }

        protected void btnValidar_Click(object sender, EventArgs e)
        {
            string usuario = txtUsuario.Text.Trim();
            string clave = txtClave.Text.Trim();

            // Validamos credenciales con la nueva capa de negocio (MongoDB)
            var respuesta = usuarioBL.Login(usuario, clave);
            // Justo después de: var respuesta = usuarioBL.Login(usuario, clave);
            // Agrega:
            if (respuesta.DebugInfo != null)
            {
                // Mostrar información de depuración en un SweetAlert
                string debugScript = $"Swal.fire('Debug', '{respuesta.DebugInfo.Replace("'", "\\'")}', 'info');";
                ScriptManager.RegisterStartupScript(this, GetType(), "debug", debugScript, true);
            }

            if (respuesta.StatusCode == 1) // Login exitoso
            {
                // Generamos el OTP y lo guardamos en MongoDB
                string otp = seguridadBL.GenerarOTP(respuesta.UsuId);

                // Obtenemos el correo del usuario para enviar el OTP
                var user = usuarioBL.ObtenerUsuarioPorId(respuesta.UsuId);
                if (user?.usu_correo != null)
                    seguridadBL.EnviarCorreoOTP(user.usu_correo, otp); // Envío real

                // Guardamos en sesión los datos necesarios para la verificación OTP
                Session["UsuarioValidadoId"] = respuesta.UsuId;
                Session["NombresTemp"] = respuesta.Nombres;
                Session["ApellidosTemp"] = respuesta.Apellidos;
                Session["TipoUsuarioIdTemp"] = respuesta.TipoUsuarioId;
                Session["TipoUsuarioTemp"] = respuesta.TipoUsuario;
                Session["RecordarTemp"] = chkRecordar.Checked;
                Session["UsuarioTemp"] = usuario;

                // Mostramos el panel del OTP y ocultamos el de credenciales
                pnlCredenciales.Visible = false;
                pnlOTP.Visible = true;

                // Mostramos el código OTP en un SweetAlert para que puedas verlo en pantalla
                string script = $"Swal.fire('Código OTP', 'Tu código es: {otp}', 'info');";
                ScriptManager.RegisterStartupScript(this, GetType(), "otpAlert", script, true);
            }
            else
            {
                // En caso de error, mostramos el mensaje con SweetAlert (usa showMessage definida en el .aspx)
                string script = $"showMessage('{respuesta.Mensaje.Replace("'", "\\'")}', 'error');";
                ScriptManager.RegisterStartupScript(this, GetType(), "error", script, true);
            }
        }

        protected void btnCancelarOTP_Click(object sender, EventArgs e)
        {
            // Limpiamos las variables temporales y volvemos al panel de credenciales
            LimpiarTemp();
            pnlCredenciales.Visible = true;
            pnlOTP.Visible = false;
        }

        private void LimpiarTemp()
        {
            Session.Remove("UsuarioValidadoId");
            Session.Remove("NombresTemp");
            Session.Remove("ApellidosTemp");
            Session.Remove("TipoUsuarioIdTemp");
            Session.Remove("TipoUsuarioTemp");
            Session.Remove("RecordarTemp");
            Session.Remove("UsuarioTemp");
        }

        // WebMethod para verificar el OTP desde JavaScript (se mantiene igual)
        [WebMethod(EnableSession = true)]
        public static object VerificarOTP(string codigo)
        {
            if (HttpContext.Current.Session["UsuarioValidadoId"] == null)
                return new { success = false, message = "Sesión expirada." };

            string usuId = (string)HttpContext.Current.Session["UsuarioValidadoId"];
            CN_Seguridad seguridadBL = new CN_Seguridad();

            if (seguridadBL.ValidarOTP(usuId, codigo))
            {
                // Iniciar sesión definitiva
                HttpContext.Current.Session["UsuarioId"] = usuId;
                HttpContext.Current.Session["Nombres"] = HttpContext.Current.Session["NombresTemp"];
                HttpContext.Current.Session["Apellidos"] = HttpContext.Current.Session["ApellidosTemp"];
                HttpContext.Current.Session["TipoUsuarioId"] = HttpContext.Current.Session["TipoUsuarioIdTemp"];
                HttpContext.Current.Session["TipoUsuario"] = HttpContext.Current.Session["TipoUsuarioTemp"];

                // Limpiar temporales
                HttpContext.Current.Session.Remove("UsuarioValidadoId");
                HttpContext.Current.Session.Remove("NombresTemp");
                HttpContext.Current.Session.Remove("ApellidosTemp");
                HttpContext.Current.Session.Remove("TipoUsuarioIdTemp");
                HttpContext.Current.Session.Remove("TipoUsuarioTemp");

                return new { success = true, message = "Inicio de sesión exitoso", redirect = "Perfil.aspx" };
            }
            else
            {
                return new { success = false, message = "Código OTP inválido o expirado." };
            }
        }
    }
}