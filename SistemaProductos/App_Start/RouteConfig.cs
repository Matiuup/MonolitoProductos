using System.Web.Routing;
using Microsoft.AspNet.FriendlyUrls;

namespace SistemaProductos
{
    public static class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            var settings = new FriendlyUrlSettings();
            // Desactiva las redirecciones automáticas que causaban el bucle
            settings.AutoRedirectMode = RedirectMode.Off;
            routes.EnableFriendlyUrls(settings);
        }
    }
}