using CapaNegocio;
using System;
using System.Collections.Generic;
using System.IO;
using System.Web.UI;

namespace SistemaProductos
{
    public partial class CargaMasiva : System.Web.UI.Page
    {
        CN_Producto cnProd = new CN_Producto();
        List<CN_Producto.FilaCargaProducto> datosCarga;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UsuarioId"] == null) Response.Redirect("~/Login.aspx");
        }

        protected void btnPrevisualizar_Click(object sender, EventArgs e)
        {
            if (!fuArchivo.HasFile)
            {
                MostrarMensaje("Seleccione un archivo.", "warning");
                return;
            }

            string ext = Path.GetExtension(fuArchivo.FileName).ToLower();
            if (ext != ".xlsx" && ext != ".xls" && ext != ".csv")
            {
                MostrarMensaje("Formato no válido. Use XLSX, XLS o CSV.", "error");
                return;
            }

            try
            {
                datosCarga = cnProd.PrevisualizarCarga(fuArchivo.FileContent);
                Session["DatosCarga"] = datosCarga;

                gvPreview.DataSource = datosCarga;
                gvPreview.DataBind();

                int nuevos = datosCarga.FindAll(f => f.Estado == "Nuevo").Count;
                int actualizar = datosCarga.FindAll(f => f.Estado == "Actualizar").Count;
                int errores = datosCarga.FindAll(f => f.Estado.StartsWith("Error")).Count;

                lblResumen.Text = $"Nuevos: {nuevos} | Actualizar: {actualizar} | Errores: {errores}";
                lblResumen.CssClass = "alert alert-info";

                pnlPreview.Visible = true;
                btnConfirmar.Visible = true;
            }
            catch (Exception ex)
            {
                MostrarMensaje("Error al leer archivo: " + ex.Message, "error");
            }
        }

        protected void btnConfirmar_Click(object sender, EventArgs e)
        {
            datosCarga = Session["DatosCarga"] as List<CN_Producto.FilaCargaProducto>;
            if (datosCarga == null || datosCarga.Count == 0)
            {
                MostrarMensaje("No hay datos para procesar.", "warning");
                return;
            }

            try
            {
                int procesados = cnProd.EjecutarCargaMasiva(datosCarga);
                Session.Remove("DatosCarga");
                pnlPreview.Visible = false;
                btnConfirmar.Visible = false;
                MostrarMensaje($"Carga masiva completada. {procesados} productos procesados.", "success");
            }
            catch (Exception ex)
            {
                MostrarMensaje("Error al ejecutar carga: " + ex.Message, "error");
            }
        }

       
        /// Muestra un mensaje con SweetAlert2 (toast) usando la misma función global que las demás páginas.
       
        private void MostrarMensaje(string mensaje, string icono)
        {
            string script = $"showMessage('{mensaje.Replace("'", "\\'")}', '{icono}');";
            ScriptManager.RegisterStartupScript(this, GetType(), "swal", script, true);
        }
    }
}