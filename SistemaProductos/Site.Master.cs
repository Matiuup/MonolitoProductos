using System;

namespace SistemaProductos
{
    public partial class Site : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Mostrar menú solo si hay sesión
            pnlMenu.Visible = (Session["UsuarioId"] != null);

            // Cambiar el fondo del body según si hay sesión o no
            if (Session["UsuarioId"] != null)
            {
                bodyMaster.Attributes["class"] = "private-bg";
            }
            else
            {
                bodyMaster.Attributes["class"] = "public-bg";
            }

            // Protección de páginas internas
            string paginaActual = System.IO.Path.GetFileName(Request.Path).ToLower();
            if (paginaActual != "login.aspx" &&
                paginaActual != "registro.aspx" &&
                paginaActual != "recuperarpassword.aspx" &&
                paginaActual != "cambiarpassword.aspx")
            {
                if (Session["UsuarioId"] == null)
                    Response.Redirect("~/Login.aspx");
            }
        }

        protected void lnkCerrar_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Response.Redirect("~/Login.aspx");
        }
    }
}