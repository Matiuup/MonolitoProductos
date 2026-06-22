using CapaNegocio.Seguridad;
using System;
using System.Web.UI;

namespace SistemaProductos
{
    public partial class CambiarPassword : System.Web.UI.Page
    {
        CN_Usuario usuarioBL = new CN_Usuario();
        CN_Seguridad segBL = new CN_Seguridad();
        string usuId;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.QueryString["uid"] == null)
            {
                MostrarMensaje("Solicitud inválida.", "alert-danger");
                pnlToken.Visible = false;
                return;
            }

            usuId = Request.QueryString["uid"];
            string modo = Request.QueryString["modo"];

            if (modo == "cambio" && Session["UsuarioId"] == null)
                Response.Redirect("~/Login.aspx");

            if (!IsPostBack)
            {
                if (modo == "cambio")
                {
                    var user = usuarioBL.ObtenerUsuarioPorId(usuId);
                    if (user?.usu_correo != null)
                    {
                        string token = segBL.GenerarCodigoRecuperacion(usuId);
                        segBL.EnviarCorreoOTP(user.usu_correo, token);
                    }
                }
                lnkVolver.NavigateUrl = modo == "cambio" ? "~/Perfil.aspx" : "~/Login.aspx";
            }
        }

        protected void btnValidarToken_Click(object sender, EventArgs e)
        {
            string token = txtToken.Text.Trim();
            if (string.IsNullOrEmpty(token) || token.Length != 6)
            {
                MostrarMensaje("Ingrese un token de 6 dígitos.", "alert-warning");
                return;
            }

            if (segBL.ValidarOTP(usuId, token))
            {
                pnlToken.Visible = false;
                pnlNueva.Visible = true;
            }
            else
            {
                MostrarMensaje("Token inválido o expirado.", "alert-danger");
            }
        }

        protected void btnCambiar_Click(object sender, EventArgs e)
        {
            string nueva = txtNueva.Text.Trim();
            string confirmar = txtConfirmar.Text.Trim();

            if (nueva != confirmar)
            {
                MostrarMensaje("Las contraseñas no coinciden.", "alert-danger");
                return;
            }
            if (!usuarioBL.ValidarComplejidadPassword(nueva))
            {
                MostrarMensaje("La contraseña no cumple los requisitos.", "alert-danger");
                return;
            }

            usuarioBL.CambiarPassword(usuId, nueva);
            string script = "Swal.fire('Éxito','Contraseña actualizada.','success').then(() => window.location='" +
                            (Session["UsuarioId"] != null ? "Default.aspx" : "Login.aspx") + "')";
            ClientScript.RegisterStartupScript(GetType(), "exito", script, true);
        }

        private void MostrarMensaje(string mensaje, string tipo)
        {
            lblMensaje.Text = mensaje;
            lblMensaje.CssClass = $"alert {tipo}";
            lblMensaje.Visible = true;
        }
    }
}