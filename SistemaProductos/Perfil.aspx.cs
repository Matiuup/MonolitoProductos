using CapaNegocio.Seguridad;
using System;
using System.IO;
using System.Web.UI;

namespace SistemaProductos
{
    public partial class Perfil : System.Web.UI.Page
    {
        CN_Usuario usuarioBL = new CN_Usuario();
        CN_Imagen imagenBL = new CN_Imagen();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UsuarioId"] == null) Response.Redirect("~/Login.aspx");

            if (!IsPostBack)
            {
                CargarDatos();
                CargarImagen();
                lnkCambiarPassword.NavigateUrl = "~/CambiarPassword.aspx?uid=" + Session["UsuarioId"] + "&modo=cambio";
            }
        }

        private void CargarDatos()
        {
            string usuId = Session["UsuarioId"].ToString();
            var user = usuarioBL.ObtenerUsuarioPorId(usuId);
            if (user != null)
            {
                lblNombreCompleto.Text = $"{user.usu_nombres} {user.usu_apellidos}";
                lblCorreo.Text = user.usu_correo;
                txtNombres.Text = user.usu_nombres;
                txtApellidos.Text = user.usu_apellidos;
                txtCelular.Text = user.usu_celular;
            }
        }

        private void CargarImagen()
        {
            string usuId = Session["UsuarioId"].ToString();
            byte[] img = imagenBL.ObtenerImagenActiva(usuId);
            if (img != null && img.Length > 0)
            {
                imgPerfil.ImageUrl = "data:image/png;base64," + Convert.ToBase64String(img);
                imgPerfil.Visible = true;
                avatarPlaceholder.Visible = false;
            }
            else
            {
                imgPerfil.Visible = false;
                avatarPlaceholder.Visible = true;
            }
        }

        protected void btnPrevisualizar_Click(object sender, EventArgs e)
        {
            if (!fuImagen.HasFile)
            {
                MostrarMensaje("Seleccione una imagen primero.", "alert-warning");
                return;
            }

            string ext = Path.GetExtension(fuImagen.FileName).ToLower().Replace(".", "");
            if (ext != "jpg" && ext != "jpeg" && ext != "png" && ext != "gif")
            {
                MostrarMensaje("Solo se permiten imágenes JPG, PNG o GIF.", "alert-danger");
                return;
            }
            if (fuImagen.FileBytes.Length > 2 * 1024 * 1024)
            {
                MostrarMensaje("La imagen supera los 2MB.", "alert-danger");
                return;
            }

            byte[] imgBytes = fuImagen.FileBytes;
            string base64 = Convert.ToBase64String(imgBytes);
            imgPreview.ImageUrl = "data:image/png;base64," + base64;
            imgPreview.Visible = true;
            btnSubirImagen.Visible = true;

            Session["ImagenTemp"] = imgBytes;
            Session["ImagenExt"] = ext;
            Session["ImagenNombre"] = fuImagen.FileName;

            MostrarMensaje("Imagen válida. Puede guardarla con el botón 'Guardar foto'.", "alert-success");
        }

        protected void btnSubirImagen_Click(object sender, EventArgs e)
        {
            if (Session["ImagenTemp"] == null)
            {
                MostrarMensaje("No hay imagen para guardar. Use 'Previsualizar' primero.", "alert-warning");
                return;
            }

            string usuId = Session["UsuarioId"].ToString();
            byte[] imgBytes = (byte[])Session["ImagenTemp"];
            string ext = Session["ImagenExt"].ToString();
            string nombre = Session["ImagenNombre"].ToString();

            imagenBL.GuardarImagen(usuId, imgBytes, ext, nombre);

            Session.Remove("ImagenTemp");
            Session.Remove("ImagenExt");
            Session.Remove("ImagenNombre");

            CargarImagen();
            imgPreview.Visible = false;
            btnSubirImagen.Visible = false;
            MostrarMensaje("Foto de perfil actualizada.", "alert-success");
        }

        protected void btnGuardar_Click(object sender, EventArgs e)
        {
            MostrarMensaje("Datos actualizados correctamente.", "alert-success");
        }

        private void MostrarMensaje(string mensaje, string tipo)
        {
            lblMensaje.Text = mensaje;
            lblMensaje.CssClass = $"alert {tipo}";
            lblMensaje.Visible = true;
        }
    }
}