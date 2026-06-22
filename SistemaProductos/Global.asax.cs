using System;
using System.Web;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.SessionState;

namespace SistemaProductos
{
    public class Global : HttpApplication
    {
        void Application_Start(object sender, EventArgs e)
        {
            // Desactivado temporalmente para evitar bucles de redirección
            // RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
        protected void Application_Error(object sender, EventArgs e)
        {
            Exception ex = Server.GetLastError();

            // Error de archivo demasiado grande
            if (ex?.InnerException is System.Web.HttpException httpEx &&
                httpEx.Message.Contains("maximum request length"))
            {
                Server.ClearError();
                Response.Redirect("~/Productos.aspx?error=tamano");
            }
        }
    }
}