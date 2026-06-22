using System;
using System.Web;

namespace SistemaProductos.Handlers
{
    public class PreviewImagen : IHttpHandler, System.Web.SessionState.IRequiresSessionState
    {
        public void ProcessRequest(HttpContext context)
        {
            byte[] bytes = context.Session["ImagenTempBytes"] as byte[];
            string ext = context.Session["ImagenTempExt"]?.ToString() ?? "jpg";

            if (bytes != null && bytes.Length > 0)
            {
                string mime = ext == "png" ? "image/png"
                            : ext == "gif" ? "image/gif"
                            : "image/jpeg";

                context.Response.ContentType = mime;
                context.Response.BinaryWrite(bytes);
            }
            else
            {
                context.Response.Redirect("~/Images/no-image.png");
            }
        }

        public bool IsReusable => false;
    }
}
