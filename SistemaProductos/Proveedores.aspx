<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Proveedores.aspx.cs" Inherits="SistemaProductos.Proveedores" MasterPageFile="~/Site.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <script src="https://cdn.jsdelivr.net/npm/sweetalert2@11"></script>
    <style>
        :root {
            --primary: #2563eb;
            --primary-dark: #1d4ed8;
            --success: #10b981;
            --danger: #ef4444;
            --text: #1e293b;
            --text-muted: #64748b;
            --border: #eef2f7;
        }

        .prov-wrapper { font-family: 'Inter','Segoe UI',sans-serif; color: var(--text); padding-bottom: 32px; }

        .prov-hero {
            background: linear-gradient(120deg, #7c3aed 0%, #6d28d9 55%, #8b5cf6 130%);
            border-radius: 22px;
            padding: 26px 30px;
            display: flex;
            align-items: center;
            justify-content: space-between;
            margin-bottom: 22px;
            color: #fff;
            box-shadow: 0 18px 40px -18px rgba(124,58,237,.6);
            flex-wrap: wrap;
            gap: 16px;
        }
        .prov-hero .hero-left { display: flex; align-items: center; gap: 16px; }
        .prov-hero .hero-icon {
            width: 56px; height: 56px; border-radius: 16px;
            background: rgba(255,255,255,.18);
            display: flex; align-items: center; justify-content: center;
            font-size: 24px; backdrop-filter: blur(6px);
        }
        .prov-hero h2 { font-size: 26px; font-weight: 800; margin: 0; letter-spacing: -.5px; }
        .prov-hero p { margin: 2px 0 0; font-size: 13.5px; opacity: .85; }
        .btn-hero {
            background: #fff; color: #7c3aed !important;
            border: none; padding: 11px 22px; font-weight: 700; font-size: 14px;
            border-radius: 12px; transition: all .2s;
            box-shadow: 0 6px 16px -4px rgba(0,0,0,.25);
            text-decoration: none;
        }
        .btn-hero:hover { transform: translateY(-2px); box-shadow: 0 10px 22px -6px rgba(0,0,0,.35); color: #6d28d9 !important; }

        .panel {
            background: #fff;
            border: 1px solid var(--border);
            border-radius: 20px;
            box-shadow: 0 6px 24px -14px rgba(0,0,0,.12);
        }
        .form-card { padding: 28px; margin-bottom: 22px; animation: fadeInUp .35s ease; }
        @keyframes fadeInUp { from { opacity:0; transform: translateY(14px);} to { opacity:1; transform: translateY(0);} }

        .form-label.fw-semibold { font-size: 12.5px; color: var(--text-muted); text-transform: uppercase; letter-spacing: .3px; }
        .form-control, .form-select {
            border: 1.5px solid var(--border); border-radius: 11px; font-size: 14px; padding: 9px 12px;
            transition: all .18s;
        }
        .form-control:focus { border-color: #7c3aed; box-shadow: 0 0 0 3px rgba(124,58,237,.12); }

        .btn-primary-custom {
            background: #7c3aed; border: none; padding: 9px 20px; font-weight: 600;
            border-radius: 11px; color: #fff; transition: all .2s; box-shadow: 0 4px 12px rgba(124,58,237,.25);
        }
        .btn-primary-custom:hover { background: #6d28d9; transform: translateY(-1px); }
        .btn-success-custom {
            background: var(--success); border: none; padding: 10px 22px; font-weight: 600;
            border-radius: 11px; color: #fff; transition: all .2s; box-shadow: 0 4px 12px rgba(16,185,129,.25);
        }
        .btn-success-custom:hover { background: #059669; transform: translateY(-1px); }
        .btn-outline-custom {
            border: 1.5px solid var(--border); background: #fff; color: var(--text); font-weight: 500;
            padding: 8px 18px; border-radius: 11px; transition: all .2s;
        }
        .btn-outline-custom:hover { background: #f8fafc; border-color: #7c3aed; color: #7c3aed; }

        .table-card { padding: 6px; overflow: hidden; }
        .table { margin-bottom: 0; background: #fff; }
        .table thead th {
            background: #f8fafc; border: none; border-bottom: 2px solid var(--border);
            font-weight: 700; font-size: 12px; text-transform: uppercase; letter-spacing: .4px;
            color: var(--text-muted); padding: 14px 16px;
        }
        .table tbody td { padding: 12px 16px; vertical-align: middle; border-bottom: 1px solid var(--border); font-size: 14px; }
        .table tbody tr { transition: background .15s; }
        .table tbody tr:hover { background: #f8fbff; }
        .table tbody tr:last-child td { border-bottom: none; }

        .action-btns .btn { margin-right: 6px; border-radius: 9px; font-weight: 600; font-size: 12.5px; padding: 5px 12px; }

        .form-section-title { font-size: 18px; font-weight: 700; display: flex; align-items: center; gap: 10px; margin-bottom: 22px; }
        .form-section-title .badge-icon {
            width: 36px; height: 36px; border-radius: 10px; background: #f3e8ff; color: #7c3aed;
            display: flex; align-items: center; justify-content: center; font-size: 15px;
        }
    </style>

    <div class="prov-wrapper">
        <div class="prov-hero">
            <div class="hero-left">
                <div class="hero-icon"><i class="fas fa-truck"></i></div>
                <div>
                    <h2>Gestión de Proveedores</h2>
                    <p>Administra los proveedores del sistema</p>
                </div>
            </div>
            <asp:Button ID="btnNuevoProv" runat="server" Text="+ Nuevo Proveedor" CssClass="btn-hero" OnClick="btnNuevoProv_Click" />
        </div>

        <!-- Panel de formulario -->
        <asp:Panel ID="pnlFormulario" runat="server" Visible="false" CssClass="panel form-card">
            <div class="form-section-title">
                <span class="badge-icon"><i class="fas fa-pen-to-square"></i></span>
                <asp:Label ID="lblTituloForm" runat="server" Text="Nuevo Proveedor" />
            </div>
            <div class="mb-3">
                <label class="form-label fw-semibold">Nombre del Proveedor *</label>
                <asp:TextBox ID="txtNombreProv" runat="server" CssClass="form-control" placeholder="Ingrese el nombre" />
            </div>
            <div class="d-flex gap-2 mt-4">
                <asp:Button ID="btnGuardarProv" runat="server" Text="Guardar" CssClass="btn-success-custom" OnClick="btnGuardarProv_Click" />
                <asp:Button ID="btnCancelarProv" runat="server" Text="Cancelar" CssClass="btn-outline-custom" OnClick="btnCancelarProv_Click" />
            </div>
            <asp:HiddenField ID="hdnProvId" runat="server" />
        </asp:Panel>

        <!-- Tabla de proveedores -->
        <div class="panel table-card">
            <div class="table-responsive">
                <asp:GridView ID="gvProveedores" runat="server" CssClass="table" AutoGenerateColumns="false"
                    DataKeyNames="prov_id" OnRowCommand="gvProveedores_RowCommand" GridLines="None">
                    <Columns>
                        <asp:BoundField DataField="prov_nombre" HeaderText="Nombre" />
                        <asp:TemplateField HeaderText="Estado">
                            <ItemTemplate>
                                <asp:Label ID="lblEstadoProv" runat="server"
                                    Text='<%# Eval("prov_estado").ToString() == "A" ? "Activo" : "Inactivo" %>'
                                    CssClass='<%# Eval("prov_estado").ToString() == "A" ? "text-success fw-bold" : "text-danger fw-bold" %>' />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="CantidadProductos" HeaderText="Productos Asociados" ItemStyle-HorizontalAlign="Center" />
                        <asp:TemplateField HeaderText="Acciones">
                            <ItemTemplate>
                                <div class="action-btns">
                                    <asp:Button ID="btnEditarProv" runat="server" Text="Editar" CssClass="btn btn-sm btn-outline-warning"
                                        CommandName="Editar" CommandArgument='<%# Eval("prov_id") %>' />
                                    <asp:Button ID="btnToggleEstadoProv" runat="server"
                                        Text='<%# Eval("prov_estado").ToString() == "A" ? "Desactivar" : "Activar" %>'
                                        CssClass='<%# "btn btn-sm " + (Eval("prov_estado").ToString() == "A" ? "btn-outline-danger" : "btn-outline-success") %>'
                                        CommandName="ToggleEstado" CommandArgument='<%# Eval("prov_id") %>' />
                                </div>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
            </div>
        </div>
    </div>

    <!-- Botón oculto para confirmar desactivación vía postback -->
    <asp:HiddenField ID="hdnIdProveedorDesactivar" runat="server" />
<asp:Button ID="Button1" runat="server" OnClick="btnConfirmarDesactivar_Click" style="display:none;" />
    <asp:Button ID="btnConfirmarDesactivar" runat="server" OnClick="btnConfirmarDesactivar_Click" style="display:none;" />

    <script>
        function showMessage(title, icon) {
            Swal.fire({ title: title, icon: icon, toast: true, position: 'top-end', showConfirmButton: false, timer: 3000 });
        }
    </script>
</asp:Content>