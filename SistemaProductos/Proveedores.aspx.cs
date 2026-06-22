using CapaNegocio;
using System;
using System.Web.UI;

namespace SistemaProductos
{
    public partial class Proveedores : System.Web.UI.Page
    {
        CN_Proveedor cnProv = new CN_Proveedor();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UsuarioId"] == null) Response.Redirect("~/Login.aspx");
            if (!IsPostBack) CargarGrid();
        }

        private void CargarGrid()
        {
            gvProveedores.DataSource = cnProv.ListarConCantidadProductos();
            gvProveedores.DataBind();
        }

        protected void btnNuevoProv_Click(object sender, EventArgs e)
        {
            pnlFormulario.Visible = true;
            lblTituloForm.Text = "Nuevo Proveedor";
            txtNombreProv.Text = "";
            hdnProvId.Value = "";
        }

        protected void btnGuardarProv_Click(object sender, EventArgs e)
        {
            try
            {
                string nombre = txtNombreProv.Text.Trim();
                string id = hdnProvId.Value;

                if (string.IsNullOrEmpty(id))
                {
                    cnProv.Insertar(nombre);
                }
                else
                {
                    cnProv.Actualizar(id, nombre);
                }

                pnlFormulario.Visible = false;
                CargarGrid();
                ScriptManager.RegisterStartupScript(this, GetType(), "success",
                    "showMessage('Proveedor guardado correctamente.', 'success')", true);
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "error",
                    $"showMessage('{ex.Message.Replace("'", "\\'")}', 'error')", true);
            }
        }

        protected void btnCancelarProv_Click(object sender, EventArgs e)
        {
            pnlFormulario.Visible = false;
        }

        protected void gvProveedores_RowCommand(object sender, System.Web.UI.WebControls.GridViewCommandEventArgs e)
        {
            string id = e.CommandArgument.ToString();

            if (e.CommandName == "Editar")
            {
                var prov = cnProv.ObtenerPorId(id);
                txtNombreProv.Text = prov.prov_nombre;
                hdnProvId.Value = id;
                lblTituloForm.Text = "Editar Proveedor";
                pnlFormulario.Visible = true;
            }
            else if (e.CommandName == "ToggleEstado")
            {
                var prov = cnProv.ObtenerPorId(id);
                if (prov.prov_estado == "A")
                {
                    // Confirmación con SweetAlert y luego establecer el HiddenField y hacer clic en el botón oculto
                    string script = $@"
                        Swal.fire({{
                            title: '¿Desactivar proveedor?',
                            text: 'Los productos asociados quedarán sin proveedor.',
                            icon: 'warning',
                            showCancelButton: true,
                            confirmButtonText: 'Sí, desactivar',
                            cancelButtonText: 'Cancelar'
                        }}).then((result) => {{
                            if (result.isConfirmed) {{
                                document.getElementById('{hdnIdProveedorDesactivar.ClientID}').value = '{id}';
                                document.getElementById('{btnConfirmarDesactivar.ClientID}').click();
                            }}
                        }});";
                    ScriptManager.RegisterStartupScript(this, GetType(), "confirmDesactivar", script, true);
                }
                else
                {
                    try
                    {
                        cnProv.Activar(id);
                        CargarGrid();
                        ScriptManager.RegisterStartupScript(this, GetType(), "success",
                            "showMessage('Proveedor activado correctamente.', 'success')", true);
                    }
                    catch (Exception ex)
                    {
                        ScriptManager.RegisterStartupScript(this, GetType(), "error",
                            $"showMessage('{ex.Message.Replace("'", "\\'")}', 'error')", true);
                    }
                }
            }
        }

        protected void btnConfirmarDesactivar_Click(object sender, EventArgs e)
        {
            string provId = hdnIdProveedorDesactivar.Value;
            if (!string.IsNullOrEmpty(provId))
            {
                try
                {
                    cnProv.Desactivar(provId);
                    CargarGrid();
                    ScriptManager.RegisterStartupScript(this, GetType(), "success",
                        "showMessage('Proveedor desactivado. Productos actualizados.', 'success')", true);
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