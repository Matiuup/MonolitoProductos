using CapaNegocio;
using System;
using System.Web.UI;

namespace SistemaProductos
{
    public partial class Categorias : System.Web.UI.Page
    {
        CN_Categoria cnCat = new CN_Categoria();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UsuarioId"] == null) Response.Redirect("~/Login.aspx");
            if (!IsPostBack) CargarGrid();
        }

        private void CargarGrid()
        {
            gvCategorias.DataSource = cnCat.ListarTodas();
            gvCategorias.DataBind();
        }

        protected void btnNuevo_Click(object sender, EventArgs e)
        {
            pnlFormulario.Visible = true;
            lblTituloForm.Text = "Nueva Categoría";
            txtNombreCat.Text = "";
            txtDescripcionCat.Text = "";
            hdnCatId.Value = "";
        }

        protected void btnGuardarCat_Click(object sender, EventArgs e)
        {
            try
            {
                string nombre = txtNombreCat.Text.Trim();
                string desc = txtDescripcionCat.Text.Trim();
                string id = hdnCatId.Value;

                if (string.IsNullOrEmpty(id) || id == "0")
                    cnCat.Insertar(nombre, desc);
                else
                    cnCat.Actualizar(id, nombre, desc);

                pnlFormulario.Visible = false;
                CargarGrid();
                ScriptManager.RegisterStartupScript(this, GetType(), "success",
                    "showMessage('Categoría guardada correctamente.', 'success')", true);
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "error",
                    $"showMessage('{ex.Message.Replace("'", "\\'")}', 'error')", true);
            }
        }

        protected void btnCancelarCat_Click(object sender, EventArgs e)
        {
            pnlFormulario.Visible = false;
        }

        protected void gvCategorias_RowCommand(object sender, System.Web.UI.WebControls.GridViewCommandEventArgs e)
        {
            string id = e.CommandArgument.ToString();

            if (e.CommandName == "Editar")
            {
                var cat = cnCat.ObtenerPorId(id);
                if (cat != null)
                {
                    txtNombreCat.Text = cat.Nombre;
                    txtDescripcionCat.Text = cat.Descripcion;
                    hdnCatId.Value = cat.Id;
                    lblTituloForm.Text = "Editar Categoría";
                    pnlFormulario.Visible = true;
                }
            }
            else if (e.CommandName == "ToggleEstado")
            {
                try
                {
                    var cat = cnCat.ObtenerPorId(id);
                    if (cat != null)
                    {
                        if (cat.Estado == "A")
                            cnCat.Desactivar(id);
                        else
                            cnCat.Activar(id);
                        CargarGrid();
                        ScriptManager.RegisterStartupScript(this, GetType(), "success",
                            "showMessage('Estado actualizado correctamente.', 'success')", true);
                    }
                }
                catch (Exception ex)
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "error",
                        $"showMessage('{ex.Message.Replace("'", "\\'")}', 'error')", true);
                }
            }
        }
    }
}