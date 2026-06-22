using CapaDatos.Modelos;
using CapaNegocio;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SistemaProductos
{
    public partial class Productos : System.Web.UI.Page
    {
        CN_Producto cnProd = new CN_Producto();
        CN_Categoria cnCat = new CN_Categoria();
        CN_Proveedor cnProv = new CN_Proveedor();
        CN_ImagenProducto cnImg = new CN_ImagenProducto();

        public int paginaActual { get; set; } = 1;
        private int totalRegistros = 0;
        private string productoEditandoId = null; // ahora string (ObjectId)

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UsuarioId"] == null) Response.Redirect("~/Login.aspx");

            if (Request.QueryString["error"] == "tamano")
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "errorTamano",
                    "showMessage('El archivo supera el tamaño máximo permitido (5 MB). Solo se permiten imágenes JPG, PNG o GIF.', 'error')", true);
            }

            if (!IsPostBack)
            {
                hdnProdId.Value = ""; // vacío para nuevo
                CargarCategorias();
                CargarProveedoresFiltro();
                CargarProveedores(null);
                CargarGrid();
                CrearPaginacion();
            }
            else
            {
                if (Session["ImagenTempBytes"] != null && !fuImagenProd.HasFile)
                {
                    imgPreviewProd.ImageUrl = "~/Handlers/PreviewImagen.ashx?t=" + DateTime.Now.Ticks;
                    imgPreviewProd.Visible = true;
                }
                if (!string.IsNullOrEmpty(hdnProdId.Value) && hdnProdId.Value != "")
                    productoEditandoId = hdnProdId.Value;
            }
        }

        private void CargarCategorias()
        {
            var cats = cnCat.ListarActivas();
            ddlFiltroCategoria.DataSource = cats;
            ddlFiltroCategoria.DataTextField = "Nombre";
            ddlFiltroCategoria.DataValueField = "Id";
            ddlFiltroCategoria.DataBind();
            ddlFiltroCategoria.Items.Insert(0, new ListItem("Todas", ""));

            ddlCategoriaProd.DataSource = cats;
            ddlCategoriaProd.DataTextField = "Nombre";
            ddlCategoriaProd.DataValueField = "Id";
            ddlCategoriaProd.DataBind();
            ddlCategoriaProd.Items.Insert(0, new ListItem("Seleccione...", ""));
        }

        private void CargarProveedoresFiltro()
        {
            var provs = cnProv.ListarActivas();
            ddlFiltroProveedor.DataSource = provs;
            ddlFiltroProveedor.DataTextField = "prov_nombre";
            ddlFiltroProveedor.DataValueField = "Id";
            ddlFiltroProveedor.DataBind();
            ddlFiltroProveedor.Items.Insert(0, new ListItem("Todos", ""));
        }

        private void CargarProveedores(Producto prod)
        {
            var lista = cnProv.ListarActivas().ToList();
            // Si el producto tiene un proveedor que ya no está activo, lo agregamos a la lista para que aparezca
            if (prod != null && !string.IsNullOrEmpty(prod.ProveedorId))
            {
                if (!lista.Any(p => p.Id == prod.ProveedorId))
                {
                    var provInactivo = cnProv.ObtenerPorId(prod.ProveedorId);
                    if (provInactivo != null) lista.Add(provInactivo);
                }
            }
            ddlProveedorProd.DataSource = lista;
            ddlProveedorProd.DataTextField = "prov_nombre";
            ddlProveedorProd.DataValueField = "Id";
            ddlProveedorProd.DataBind();
            ddlProveedorProd.Items.Insert(0, new ListItem("Sin proveedor", ""));
        }

        private void CargarGrid()
        {
            string nombre = txtFiltroNombre.Text.Trim();
            string catId = ddlFiltroCategoria.SelectedValue;
            string provId = ddlFiltroProveedor.SelectedValue;
            decimal? precioMin = !string.IsNullOrEmpty(txtPrecioMin.Text) ? Convert.ToDecimal(txtPrecioMin.Text) : (decimal?)null;
            decimal? precioMax = !string.IsNullOrEmpty(txtPrecioMax.Text) ? Convert.ToDecimal(txtPrecioMax.Text) : (decimal?)null;
            int tamaño = Convert.ToInt32(ddlPageSize.SelectedValue);
            string estado = ddlFiltroEstado.SelectedValue;

            var lista = cnProd.BuscarPaginado(nombre, catId, precioMin, precioMax, estado, provId, paginaActual, tamaño, out totalRegistros);
            gvProductos.DataSource = lista;
            gvProductos.DataBind();
        }

        private void CrearPaginacion()
        {
            int tamaño = Convert.ToInt32(ddlPageSize.SelectedValue);
            int totalPaginas = (int)Math.Ceiling((double)totalRegistros / tamaño);

            int rango = 5;
            int inicio = Math.Max(1, paginaActual - rango);
            int fin = Math.Min(totalPaginas, paginaActual + rango);

            var paginas = new List<object>();
            for (int i = inicio; i <= fin; i++)
                paginas.Add(new { Text = i.ToString(), Value = i.ToString() });

            rptPaginas.DataSource = paginas;
            rptPaginas.DataBind();

            lnkPrimera.Enabled = paginaActual > 1;
            lnkAnterior.Enabled = paginaActual > 1;
            lnkSiguiente.Enabled = paginaActual < totalPaginas;
            lnkUltima.Enabled = paginaActual < totalPaginas;
        }

        private void Refrescar() { paginaActual = 1; CargarGrid(); CrearPaginacion(); }

        protected void txtFiltroNombre_TextChanged(object sender, EventArgs e) => Refrescar();
        protected void ddlFiltroCategoria_SelectedIndexChanged(object sender, EventArgs e) => Refrescar();
        protected void ddlFiltroProveedor_SelectedIndexChanged(object sender, EventArgs e) => Refrescar();
        protected void ddlFiltroEstado_SelectedIndexChanged(object sender, EventArgs e) => Refrescar();
        protected void btnBuscar_Click(object sender, EventArgs e) => Refrescar();

        protected void lnkPagina_Click(object sender, EventArgs e)
        {
            paginaActual = Convert.ToInt32(((LinkButton)sender).CommandArgument);
            CargarGrid(); CrearPaginacion();
        }
        protected void lnkAnterior_Click(object sender, EventArgs e) { if (paginaActual > 1) { paginaActual--; CargarGrid(); CrearPaginacion(); } }
        protected void lnkSiguiente_Click(object sender, EventArgs e)
        {
            int tamaño = Convert.ToInt32(ddlPageSize.SelectedValue);
            int totalPaginas = (int)Math.Ceiling((double)totalRegistros / tamaño);
            if (paginaActual < totalPaginas) { paginaActual++; CargarGrid(); CrearPaginacion(); }
        }
        protected void ddlPageSize_Changed(object sender, EventArgs e) => Refrescar();

        protected void lnkPrimera_Click(object sender, EventArgs e) { paginaActual = 1; CargarGrid(); CrearPaginacion(); }

        protected void lnkUltima_Click(object sender, EventArgs e)
        {
            int tamaño = Convert.ToInt32(ddlPageSize.SelectedValue);
            int totalPaginas = (int)Math.Ceiling((double)totalRegistros / tamaño);
            paginaActual = totalPaginas;
            CargarGrid(); CrearPaginacion();
        }

        // ---------- CRUD ----------
        protected void btnNuevoProd_Click(object sender, EventArgs e)
        {
            LimpiarForm();
            CargarProveedores(null);
            pnlForm.Visible = true;
            lblTituloForm.Text = "Nuevo Producto";
            productoEditandoId = null;
            pnlCarrusel.Visible = false;
        }

        protected void btnGuardarProd_Click(object sender, EventArgs e)
        {
            try
            {
                string nombre = txtNombreProd.Text.Trim();
                string desc = txtDescripcionProd.Text.Trim();
                decimal precio = Convert.ToDecimal(txtPrecioProd.Text);
                int stock = Convert.ToInt32(txtStockProd.Text);
                string catId = ddlCategoriaProd.SelectedValue;
                string provId = ddlProveedorProd.SelectedValue;
                string prodId = hdnProdId.Value;

                if (string.IsNullOrEmpty(prodId))
                {
                    // Primero guardar imagen si hay
                    string rutaImagen = null;
                    if (Session["ImagenTempBytes"] != null || fuImagenProd.HasFile)
                        rutaImagen = ProcesarYGuardarImagen();

                    string nuevoId = cnProd.Insertar(nombre, desc, precio, stock, catId, provId, rutaImagen);
                    productoEditandoId = nuevoId;
                    hdnProdId.Value = nuevoId;
                }
                else
                {
                    cnProd.Actualizar(prodId, nombre, desc, precio, stock, catId, provId);

                    // Actualizar imagen si se ha subido una nueva
                    if (Session["ImagenTempBytes"] != null || fuImagenProd.HasFile)
                    {
                        string rutaImagen = ProcesarYGuardarImagen();
                        cnProd.ActualizarRutaImagen(prodId, rutaImagen);
                    }
                    productoEditandoId = prodId;
                }

                Session.Remove("ImagenTempBytes"); Session.Remove("ImagenTempExt");
                pnlForm.Visible = false;
                CargarGrid(); CrearPaginacion();
                ScriptManager.RegisterStartupScript(this, GetType(), "success", "showMessage('Producto guardado correctamente.', 'success')", true);
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "error", $"showMessage('{ex.Message.Replace("'", "\\'")}', 'error')", true);
            }
        }

        private string ProcesarYGuardarImagen()
        {
            byte[] bytes = Session["ImagenTempBytes"] as byte[];
            string ext = Session["ImagenTempExt"] as string;

            if (bytes == null || string.IsNullOrEmpty(ext))
            {
                if (!fuImagenProd.HasFile) return null;
                ext = Path.GetExtension(fuImagenProd.FileName).ToLower().Replace(".", "");
                string[] permitidas = { "jpg", "jpeg", "png", "gif" };
                if (!permitidas.Contains(ext)) throw new Exception("Formato de imagen no permitido.");
                if (fuImagenProd.FileBytes.Length > 2 * 1024 * 1024) throw new Exception("La imagen supera los 2MB.");
                bytes = fuImagenProd.FileBytes;
            }

            string nombreArchivo = Guid.NewGuid().ToString() + "." + ext;
            string rutaCarpeta = Server.MapPath("~/Imagenes/Productos/");
            if (!Directory.Exists(rutaCarpeta)) Directory.CreateDirectory(rutaCarpeta);
            string rutaCompleta = Path.Combine(rutaCarpeta, nombreArchivo);
            File.WriteAllBytes(rutaCompleta, bytes);

            return "~/Imagenes/Productos/" + nombreArchivo;
        }

        protected void btnCancelarProd_Click(object sender, EventArgs e)
        {
            pnlForm.Visible = false;
            Session.Remove("ImagenTempBytes"); Session.Remove("ImagenTempExt");
        }

        protected void gvProductos_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            string id = e.CommandArgument.ToString(); // ahora es string (ObjectId)
            if (e.CommandName == "Editar")
            {
                var prod = cnProd.ObtenerPorId(id);
                if (prod != null)
                {
                    txtNombreProd.Text = prod.pro_nombre;
                    txtDescripcionProd.Text = prod.pro_descripcion;
                    txtPrecioProd.Text = prod.pro_precio.ToString("F2");
                    txtStockProd.Text = prod.pro_stock.ToString();
                    ddlCategoriaProd.SelectedValue = prod.CategoriaId;
                    hdnProdId.Value = prod.Id;
                    productoEditandoId = prod.Id;
                    lblTituloForm.Text = "Editar Producto";
                    pnlForm.Visible = true;
                    CargarProveedores(prod);
                    ddlProveedorProd.SelectedValue = prod.ProveedorId ?? "";
                    if (!string.IsNullOrEmpty(prod.pro_ruta_imagen))
                    {
                        imgPreviewProd.ImageUrl = ResolveUrl(prod.pro_ruta_imagen);
                        imgPreviewProd.Visible = true;
                    }
                    else imgPreviewProd.Visible = false;
                    Session.Remove("ImagenTempBytes"); Session.Remove("ImagenTempExt");
                    CargarImagenesProducto(id);
                }
            }
            else if (e.CommandName == "ToggleEstado")
            {
                try
                {
                    var prod = cnProd.ObtenerPorId(id);
                    if (prod != null)
                    {
                        if (prod.pro_estado == "A") cnProd.Desactivar(id);
                        else cnProd.Activar(id);
                        CargarGrid(); CrearPaginacion();
                        ScriptManager.RegisterStartupScript(this, GetType(), "success", "showMessage('Estado actualizado.', 'success')", true);
                    }
                }
                catch (Exception ex) { ScriptManager.RegisterStartupScript(this, GetType(), "error", $"showMessage('{ex.Message.Replace("'", "\\'")}', 'error')", true); }
            }
        }

        // ---------- IMAGEN PRINCIPAL ----------
        protected void btnPrevisualizarImg_Click(object sender, EventArgs e)
        {
            try
            {
                if (!fuImagenProd.HasFile) { ScriptManager.RegisterStartupScript(this, GetType(), "warning", "showMessage('Seleccione una imagen.', 'warning')", true); return; }
                string ext = Path.GetExtension(fuImagenProd.FileName).ToLower().Replace(".", "");
                string[] permitidas = { "jpg", "jpeg", "png", "gif" };
                if (!permitidas.Contains(ext)) { ScriptManager.RegisterStartupScript(this, GetType(), "error", "showMessage('Solo JPG, PNG o GIF.', 'error')", true); return; }
                if (fuImagenProd.FileBytes.Length > 2 * 1024 * 1024) { ScriptManager.RegisterStartupScript(this, GetType(), "error", "showMessage('La imagen supera los 2MB.', 'error')", true); return; }
                byte[] bytes = fuImagenProd.FileBytes;
                Session["ImagenTempBytes"] = bytes; Session["ImagenTempExt"] = ext;
                imgPreviewProd.ImageUrl = "~/Handlers/PreviewImagen.ashx?t=" + DateTime.Now.Ticks;
                imgPreviewProd.Visible = true;
            }
            catch (Exception ex) { ScriptManager.RegisterStartupScript(this, GetType(), "error", $"showMessage('Error: {ex.Message.Replace("'", "\\'")}', 'error')", true); }
        }

        // ---------- CARRUSEL DE IMÁGENES ----------
        private void CargarImagenesProducto(string prodId)
        {
            var imagenes = cnImg.ObtenerPorProducto(prodId);
            if (imagenes != null && imagenes.Count > 0)
            {
                rptImagenes.DataSource = imagenes;
                rptImagenes.DataBind();
                rptThumbs.DataSource = imagenes;
                rptThumbs.DataBind();
                pnlCarrusel.Visible = true;
                lblSinImagenes.Visible = false;
            }
            else
            {
                rptImagenes.DataSource = null;
                rptImagenes.DataBind();
                rptThumbs.DataSource = null;
                rptThumbs.DataBind();
                pnlCarrusel.Visible = true;
                lblSinImagenes.Visible = true;
            }
        }

        protected void btnAgregarImagen_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(productoEditandoId))
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "warning", "showMessage('Primero guarde el producto para agregar imágenes.', 'warning')", true);
                return;
            }

            if (!fuImagenCarrusel.HasFile)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "warning", "showMessage('Seleccione una imagen.', 'warning')", true);
                return;
            }

            try
            {
                string ext = Path.GetExtension(fuImagenCarrusel.FileName).ToLower().Replace(".", "");
                string[] permitidas = { "jpg", "jpeg", "png" };
                if (!permitidas.Contains(ext))
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "error", "showMessage('Solo JPG o PNG.', 'error')", true);
                    return;
                }
                if (fuImagenCarrusel.FileBytes.Length > 2 * 1024 * 1024)
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "error", "showMessage('La imagen supera los 2MB.', 'error')", true);
                    return;
                }

                string nombreArchivo = Guid.NewGuid().ToString() + "." + ext;
                string rutaCarpeta = Server.MapPath("~/Imagenes/Productos/");
                if (!Directory.Exists(rutaCarpeta)) Directory.CreateDirectory(rutaCarpeta);
                string rutaCompleta = Path.Combine(rutaCarpeta, nombreArchivo);
                fuImagenCarrusel.SaveAs(rutaCompleta);

                string rutaRelativa = "~/Imagenes/Productos/" + nombreArchivo;
                cnImg.Insertar(productoEditandoId, rutaRelativa, nombreArchivo);

                CargarImagenesProducto(productoEditandoId);
                ScriptManager.RegisterStartupScript(this, GetType(), "success", "showMessage('Imagen agregada correctamente.', 'success')", true);
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "error", $"showMessage('{ex.Message.Replace("'", "\\'")}', 'error')", true);
            }
        }

        protected void btnEliminarImagen_Click(object sender, EventArgs e)
        {
            string imgId = hdnImagenSeleccionada.Value;
            if (string.IsNullOrEmpty(imgId))
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "warning", "showMessage('Seleccione una imagen del carrusel.', 'warning')", true);
                return;
            }

            try
            {
                cnImg.Eliminar(imgId);
                CargarImagenesProducto(productoEditandoId);
                hdnImagenSeleccionada.Value = "";
                ScriptManager.RegisterStartupScript(this, GetType(), "success", "showMessage('Imagen eliminada.', 'success')", true);
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "error", $"showMessage('{ex.Message.Replace("'", "\\'")}', 'error')", true);
            }
        }

        protected void btnThumb_Click(object sender, ImageClickEventArgs e)
        {
            var btn = sender as ImageButton;
            hdnImagenSeleccionada.Value = btn.CommandArgument;
        }

        // ---------- MÉTODOS AUXILIARES ----------
        private void LimpiarForm()
        {
            txtNombreProd.Text = "";
            txtDescripcionProd.Text = "";
            txtPrecioProd.Text = "";
            txtStockProd.Text = "";
            ddlCategoriaProd.SelectedIndex = 0;
            ddlProveedorProd.SelectedIndex = 0;
            hdnProdId.Value = "";
            imgPreviewProd.Visible = false;
            Session.Remove("ImagenTempBytes"); Session.Remove("ImagenTempExt");
            productoEditandoId = null;
            pnlCarrusel.Visible = false;
            lblSinImagenes.Visible = false;
            hdnImagenSeleccionada.Value = "";
        }

        public string ObtenerClasePagina(int pagina) => pagina == paginaActual ? "active" : "";

        public string ObtenerUrlImagen(object ruta)
        {
            if (ruta == null || ruta == DBNull.Value || string.IsNullOrEmpty(ruta.ToString()))
                return "Images/no-image.png";
            string url = ResolveUrl(ruta.ToString());
            return url + "?t=" + DateTime.Now.Ticks;
        }

        public string ObtenerUrlImagenCarrusel(object ruta)
        {
            if (ruta == null || ruta == DBNull.Value || string.IsNullOrEmpty(ruta.ToString()))
                return "Images/no-image.png";
            return ResolveUrl(ruta.ToString());
        }

        protected void btnSubirMasImagenes_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(hdnProdId.Value) || hdnProdId.Value == "")
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "warning",
                    "showMessage('Primero guarde el producto para agregar imágenes adicionales.', 'warning')", true);
                return;
            }

            productoEditandoId = hdnProdId.Value;
            pnlCarrusel.Visible = true;
            CargarImagenesProducto(hdnProdId.Value);
        }
    }
}