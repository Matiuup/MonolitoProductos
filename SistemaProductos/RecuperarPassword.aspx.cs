using CapaNegocio.Seguridad;
using System;
using System.Data;
using System.Web.UI;

namespace SistemaProductos
{
    public partial class RecuperarPassword : System.Web.UI.Page
    {
        CN_Usuario usuarioBL = new CN_Usuario();
        CN_Seguridad seguridadBL = new CN_Seguridad();

        protected void btnEnviar_Click(object sender, EventArgs e)
        {
            string correo = txtCorreo.Text.Trim();
            if (string.IsNullOrEmpty(correo) || !usuarioBL.ValidarCorreo(correo))
            {
                MostrarMensaje("Ingrese un correo electrónico válido.", "alert-danger");
                return;
            }

            // Llamamos al método de seguridad que genera el token y lo guarda en MongoDB
            DataSet ds = seguridadBL.GenerarTokenRecuperacion(correo);
            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                DataRow row = ds.Tables[0].Rows[0];
                int status = Convert.ToInt32(row["StatusCode"]);
                if (status == 1)
                {
                    string token = row["Token"].ToString();
                    string usuarioId = row["UsuarioId"].ToString();
                    seguridadBL.EnviarCorreoRecuperacion(correo, token);
                    Response.Redirect("CambiarPassword.aspx?uid=" + usuarioId + "&modo=recuperacion");
                }
                else
                {
                    MostrarMensaje(row["Mensaje"].ToString(), "alert-danger");
                }
            }
            else
            {
                MostrarMensaje("No se pudo generar el token. Intente de nuevo.", "alert-danger");
            }
        }

        private void MostrarMensaje(string msg, string tipo)
        {
            lblMensaje.Text = msg;
            lblMensaje.CssClass = $"alert {tipo}";
            lblMensaje.Visible = true;
        }
    }
}