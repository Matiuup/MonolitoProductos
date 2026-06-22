<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Categorias.aspx.cs" Inherits="SistemaProductos.Categorias" MasterPageFile="~/Site.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <script src="https://cdn.jsdelivr.net/npm/sweetalert2@11"></script>
    <style>
        :root {
            --primary: #2563eb;
            --primary-dark: #1d4ed8;
            --accent: #06b6d4;
            --bg: #f8fafc;
            --card: #ffffff;
            --text: #1e293b;
            --text-muted: #64748b;
            --border: #e2e8f0;
        }
        body { background: var(--bg); font-family: 'Inter', sans-serif; }
        .page-header {
            display: flex;
            align-items: center;
            justify-content: space-between;
            margin-bottom: 1.5rem;
        }
        .page-header h2 {
            font-weight: 700;
            color: var(--text);
        }
        .card {
            background: var(--card);
            border-radius: 20px;
            box-shadow: 0 10px 30px -5px rgba(0,0,0,0.05);
            transition: all 0.2s ease;
            border: 1px solid rgba(0,0,0,0.03);
        }
        .form-card {
            padding: 2rem;
            margin-bottom: 2rem;
            animation: fadeInUp 0.4s ease;
        }
        @keyframes fadeInUp {
            from { opacity: 0; transform: translateY(15px); }
            to { opacity: 1; transform: translateY(0); }
        }
        .btn-primary-custom {
            background: var(--primary);
            border: none;
            padding: 0.6rem 1.8rem;
            font-weight: 600;
            border-radius: 12px;
            color: white;
            transition: all 0.2s;
            box-shadow: 0 4px 10px rgba(37,99,235,0.2);
        }
        .btn-primary-custom:hover {
            background: var(--primary-dark);
            transform: translateY(-2px);
            box-shadow: 0 8px 20px rgba(37,99,235,0.3);
        }
        .btn-success-custom {
            background: #10b981;
            border: none;
            padding: 0.6rem 1.8rem;
            font-weight: 600;
            border-radius: 12px;
            color: white;
            transition: all 0.2s;
            box-shadow: 0 4px 10px rgba(16,185,129,0.2);
        }
        .btn-success-custom:hover {
            background: #059669;
            transform: translateY(-2px);
        }
        .btn-outline-custom {
            border: 2px solid var(--border);
            background: white;
            color: var(--text);
            font-weight: 500;
            padding: 0.5rem 1.5rem;
            border-radius: 12px;
            transition: all 0.2s;
        }
        .btn-outline-custom:hover {
            background: #f1f5f9;
            border-color: var(--primary);
            color: var(--primary);
        }
        .table-responsive {
            border-radius: 16px;
            overflow: hidden;
        }
        .table {
            margin-bottom: 0;
            background: white;
        }
        .table th {
            background: #f8fafc;
            border-bottom: 2px solid var(--border);
            font-weight: 600;
            color: var(--text);
            padding: 1rem;
        }
        .table td {
            padding: 1rem;
            vertical-align: middle;
        }
        .badge-status {
            padding: 0.4em 0.8em;
            border-radius: 50px;
            font-weight: 500;
            font-size: 0.8rem;
        }
        .action-btns .btn {
            margin-right: 5px;
            border-radius: 8px;
            font-weight: 500;
        }
    </style>

    <div class="container-fluid">
        <div class="page-header">
            <h2><i class="fas fa-tags me-2"></i>Categorías</h2>
            <asp:Button ID="btnNuevo" runat="server" Text="Nueva Categoría" CssClass="btn-primary-custom" OnClick="btnNuevo_Click" />
        </div>

        <asp:Panel ID="pnlFormulario" runat="server" Visible="false" CssClass="card form-card">
            <h5 class="mb-4"><asp:Label ID="lblTituloForm" runat="server" Text="Nueva Categoría" /></h5>
            <div class="row">
                <div class="col-md-6 mb-3">
                    <label class="form-label fw-semibold">Nombre</label>
                    <asp:TextBox ID="txtNombreCat" runat="server" CssClass="form-control" placeholder="Nombre de la categoría" />
                </div>
                <div class="col-md-6 mb-3">
                    <label class="form-label fw-semibold">Descripción</label>
                    <asp:TextBox ID="txtDescripcionCat" runat="server" CssClass="form-control" placeholder="Descripción (opcional)" />
                </div>
            </div>
            <div class="d-flex gap-2 mt-2">
                <asp:Button ID="btnGuardarCat" runat="server" Text="Guardar" CssClass="btn-success-custom" OnClick="btnGuardarCat_Click" />
                <asp:Button ID="btnCancelarCat" runat="server" Text="Cancelar" CssClass="btn-outline-custom" OnClick="btnCancelarCat_Click" />
            </div>
            <asp:HiddenField ID="hdnCatId" runat="server" />
        </asp:Panel>

        <div class="card table-responsive">
            <asp:GridView ID="gvCategorias" runat="server" CssClass="table" AutoGenerateColumns="false"
                DataKeyNames="Id" OnRowCommand="gvCategorias_RowCommand" GridLines="None">
                <Columns>
                    <asp:BoundField DataField="Nombre" HeaderText="Nombre" />
                    <asp:BoundField DataField="Descripcion" HeaderText="Descripción" />
                    <asp:TemplateField HeaderText="Estado">
                        <ItemTemplate>
                            <span class='badge-status <%# Eval("Estado").ToString() == "A" ? "bg-success" : "bg-danger" %>'>
                                <%# Eval("Estado").ToString() == "A" ? "Activo" : "Inactivo" %>
                            </span>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Acciones">
                        <ItemTemplate>
                            <div class="action-btns">
                                <asp:Button ID="btnEditarCat" runat="server" Text="Editar" CssClass="btn btn-sm btn-outline-warning" CommandName="Editar" CommandArgument='<%# Eval("Id") %>' />
                                <asp:Button ID="btnDesactivarCat" runat="server" Text='<%# Eval("Estado").ToString() == "A" ? "Desactivar" : "Activar" %>'
                                    CssClass='<%# "btn btn-sm " + (Eval("Estado").ToString() == "A" ? "btn-outline-danger" : "btn-outline-success") %>'
                                    CommandName="ToggleEstado" CommandArgument='<%# Eval("Id") %>' />
                            </div>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
        </div>
    </div>

    <script>
        function showMessage(title, icon) {
            Swal.fire({ title: title, icon: icon, toast: true, position: 'top-end', showConfirmButton: false, timer: 3000 });
        }
    </script>
</asp:Content>