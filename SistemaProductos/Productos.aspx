<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Productos.aspx.cs" Inherits="SistemaProductos.Productos" MasterPageFile="~/Site.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <asp:ScriptManager ID="sm" runat="server" />
    <script src="https://cdn.jsdelivr.net/npm/sweetalert2@11"></script>

    <style>
        :root {
            --primary: #2563eb;
            --primary-dark: #1d4ed8;
            --accent: #06b6d4;
            --success: #10b981;
            --danger: #ef4444;
            --warning: #f59e0b;
            --text: #1e293b;
            --text-muted: #64748b;
            --border: #eef2f7;
        }

        .prod-wrapper { font-family: 'Inter','Segoe UI',sans-serif; color: var(--text); padding-bottom: 32px; }

        /* HEADER tipo banner */
        .prod-hero {
            background: linear-gradient(120deg, #2563eb 0%, #1e40af 55%, #06b6d4 130%);
            border-radius: 22px;
            padding: 26px 30px;
            display: flex;
            align-items: center;
            justify-content: space-between;
            margin-bottom: 22px;
            color: #fff;
            box-shadow: 0 18px 40px -18px rgba(37,99,235,.6);
            flex-wrap: wrap;
            gap: 16px;
        }
        .prod-hero .hero-left { display: flex; align-items: center; gap: 16px; }
        .prod-hero .hero-icon {
            width: 56px; height: 56px; border-radius: 16px;
            background: rgba(255,255,255,.18);
            display: flex; align-items: center; justify-content: center;
            font-size: 24px; backdrop-filter: blur(6px);
        }
        .prod-hero h2 { font-size: 26px; font-weight: 800; margin: 0; letter-spacing: -.5px; }
        .prod-hero p { margin: 2px 0 0; font-size: 13.5px; opacity: .85; }
        .btn-hero {
            background: #fff; color: var(--primary) !important;
            border: none; padding: 11px 22px; font-weight: 700; font-size: 14px;
            border-radius: 12px; transition: all .2s;
            box-shadow: 0 6px 16px -4px rgba(0,0,0,.25);
        }
        .btn-hero:hover { transform: translateY(-2px); box-shadow: 0 10px 22px -6px rgba(0,0,0,.35); color: var(--primary-dark) !important; }

        /* TARJETAS base */
        .panel {
            background: #fff;
            border: 1px solid var(--border);
            border-radius: 20px;
            box-shadow: 0 6px 24px -14px rgba(0,0,0,.12);
        }
        .filter-card { padding: 20px 22px; margin-bottom: 22px; }
        .form-card { padding: 28px; margin-bottom: 22px; animation: fadeInUp .35s ease; }
        @keyframes fadeInUp { from { opacity:0; transform: translateY(14px);} to { opacity:1; transform: translateY(0);} }

        .form-label.fw-semibold { font-size: 12.5px; color: var(--text-muted); text-transform: uppercase; letter-spacing: .3px; }
        .form-control, .form-select {
            border: 1.5px solid var(--border); border-radius: 11px; font-size: 14px; padding: 9px 12px;
            transition: all .18s;
        }
        .form-control:focus, .form-select:focus {
            border-color: var(--primary); box-shadow: 0 0 0 3px rgba(37,99,235,.12);
        }

        /* BOTONES */
        .btn-primary-custom {
            background: var(--primary); border: none; padding: 9px 20px; font-weight: 600;
            border-radius: 11px; color: #fff; transition: all .2s; box-shadow: 0 4px 12px rgba(37,99,235,.25);
        }
        .btn-primary-custom:hover { background: var(--primary-dark); transform: translateY(-1px); }
        .btn-success-custom {
            background: var(--success); border: none; padding: 10px 22px; font-weight: 600;
            border-radius: 11px; color: #fff; transition: all .2s; box-shadow: 0 4px 12px rgba(16,185,129,.25);
        }
        .btn-success-custom:hover { background: #059669; transform: translateY(-1px); }
        .btn-outline-custom {
            border: 1.5px solid var(--border); background: #fff; color: var(--text); font-weight: 500;
            padding: 8px 18px; border-radius: 11px; transition: all .2s;
        }
        .btn-outline-custom:hover { background: #f8fafc; border-color: var(--primary); color: var(--primary); }

        .search-box { position: relative; }
        .search-box input { padding-right: 2.4rem; }
        .search-box .clear-btn {
            position: absolute; right: 10px; top: 50%; transform: translateY(-50%);
            background: none; border: none; color: #94a3b8; cursor: pointer;
        }

        .form-section-title { font-size: 18px; font-weight: 700; display: flex; align-items: center; gap: 10px; margin-bottom: 22px; }
        .form-section-title .badge-icon {
            width: 36px; height: 36px; border-radius: 10px; background: #eff6ff; color: var(--primary);
            display: flex; align-items: center; justify-content: center; font-size: 15px;
        }

        /* TABLA */
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
        .prod-img-cell img { width: 48px; height: 48px; border-radius: 10px; object-fit: cover; border: 1px solid var(--border); }

        .action-btns .btn { margin-right: 6px; border-radius: 9px; font-weight: 600; font-size: 12.5px; padding: 5px 12px; }

        /* PAGINACION */
        .pagination .page-link {
            border-radius: 9px; margin: 0 3px; border: 1px solid var(--border); color: var(--text); font-weight: 600; font-size: 13.5px;
        }
        .pagination .page-link:hover { background: #f1f5f9; color: var(--primary); }
        .pagination .active .page-link { background: var(--primary); border-color: var(--primary); color: #fff; }

        /* CARRUSEL */
        .carousel-inner img { height: 200px; object-fit: contain; background: #f8fafc; border-radius: 12px; }
        .carousel-thumb {
            width: 60px; height: 60px; object-fit: cover; border-radius: 9px;
            border: 2px solid transparent; cursor: pointer; transition: all .2s;
        }
        .carousel-thumb:hover { border-color: var(--primary); }
        .carousel-thumb.active { border-color: var(--primary); box-shadow: 0 0 0 2px rgba(37,99,235,.3); }
        .carousel-control-prev-icon, .carousel-control-next-icon { background-color: rgba(37,99,235,.85); border-radius: 50%; padding: 12px; background-size: 50%; }
    </style>

    <div class="prod-wrapper">
        <!-- HERO HEADER -->
        <div class="prod-hero">
            <div class="hero-left">
                <div class="hero-icon"><i class="fas fa-boxes-stacked"></i></div>
                <div>
                    <h2>Gestión de Productos</h2>
                    <p>Administra el catálogo, inventario e imágenes de tus productos</p>
                </div>
            </div>
            <asp:Button ID="btnNuevoProd" runat="server" Text="+ Nuevo Producto" CssClass="btn-hero" OnClick="btnNuevoProd_Click" />
        </div>

        <asp:UpdatePanel ID="upPrincipal" runat="server" UpdateMode="Always">
            <ContentTemplate>
                <!-- FILTROS -->
                <div class="panel filter-card">
                    <div class="row g-3 align-items-end">
                        <div class="col-md-3">
                            <label class="form-label fw-semibold">Nombre</label>
                            <div class="search-box">
                                <asp:TextBox ID="txtFiltroNombre" runat="server" CssClass="form-control" placeholder="Buscar producto..."
                                    AutoPostBack="true" OnTextChanged="txtFiltroNombre_TextChanged" />
                                <button type="button" class="clear-btn" onclick="clearSearch()" title="Limpiar">
                                    <i class="fas fa-times"></i>
                                </button>
                            </div>
                        </div>
                        <div class="col-md-2">
                            <label class="form-label fw-semibold">Categoría</label>
                            <asp:DropDownList ID="ddlFiltroCategoria" runat="server" CssClass="form-select"
                                AutoPostBack="true" OnSelectedIndexChanged="ddlFiltroCategoria_SelectedIndexChanged" />
                        </div>
                        <div class="col-md-2">
                            <label class="form-label fw-semibold">Proveedor</label>
                            <asp:DropDownList ID="ddlFiltroProveedor" runat="server" CssClass="form-select"
                                AutoPostBack="true" OnSelectedIndexChanged="ddlFiltroProveedor_SelectedIndexChanged" />
                        </div>
                        <div class="col-md-2">
                            <label class="form-label fw-semibold">Estado</label>
                            <asp:DropDownList ID="ddlFiltroEstado" runat="server" CssClass="form-select"
                                AutoPostBack="true" OnSelectedIndexChanged="ddlFiltroEstado_SelectedIndexChanged">
                                <asp:ListItem Text="Activos" Value="A" Selected="True" />
                                <asp:ListItem Text="Inactivos" Value="I" />
                                <asp:ListItem Text="Todos" Value="Todos" />
                            </asp:DropDownList>
                        </div>
                        <div class="col-md-1">
                            <label class="form-label fw-semibold">Desde</label>
                            <asp:TextBox ID="txtPrecioMin" runat="server" CssClass="form-control" TextMode="Number" placeholder="0" />
                        </div>
                        <div class="col-md-1">
                            <label class="form-label fw-semibold">Hasta</label>
                            <asp:TextBox ID="txtPrecioMax" runat="server" CssClass="form-control" TextMode="Number" placeholder="9999" />
                        </div>
                        <div class="col-md-1">
                            <asp:Button ID="btnBuscar" runat="server" Text="Filtrar" CssClass="btn-primary-custom w-100" OnClick="btnBuscar_Click" />
                        </div>
                    </div>
                </div>

                <!-- FORMULARIO -->
                <asp:Panel ID="pnlForm" runat="server" Visible="false" CssClass="panel form-card">
                    <div class="form-section-title">
                        <span class="badge-icon"><i class="fas fa-pen-to-square"></i></span>
                        <asp:Label ID="lblTituloForm" runat="server" Text="Nuevo Producto" />
                    </div>
                    <div class="row">
                        <div class="col-md-6 mb-3">
                            <label class="form-label fw-semibold">Nombre *</label>
                            <asp:TextBox ID="txtNombreProd" runat="server" CssClass="form-control" placeholder="Nombre del producto" />
                        </div>
                        <div class="col-md-6 mb-3">
                            <label class="form-label fw-semibold">Categoría *</label>
                            <asp:DropDownList ID="ddlCategoriaProd" runat="server" CssClass="form-select" />
                        </div>
                        <div class="col-md-4 mb-3">
                            <label class="form-label fw-semibold">Precio *</label>
                            <asp:TextBox ID="txtPrecioProd" runat="server" CssClass="form-control" TextMode="Number" placeholder="0.00" />
                        </div>
                        <div class="col-md-4 mb-3">
                            <label class="form-label fw-semibold">Stock *</label>
                            <asp:TextBox ID="txtStockProd" runat="server" CssClass="form-control" TextMode="Number" placeholder="0" />
                        </div>
                        <div class="col-md-4 mb-3">
                            <label class="form-label fw-semibold">Proveedor</label>
                            <asp:DropDownList ID="ddlProveedorProd" runat="server" CssClass="form-select" />
                        </div>
                        <div class="col-md-6 mb-3">
                            <label class="form-label fw-semibold">Imagen principal (JPG/PNG, máx. 2MB)</label>
                            <asp:FileUpload ID="fuImagenProd" runat="server" CssClass="form-control" accept=".jpg,.jpeg,.png" />
                            <asp:Button ID="btnPrevisualizarImg" runat="server" Text="Previsualizar" CssClass="btn-outline-custom mt-2" OnClick="btnPrevisualizarImg_Click" />
                            <asp:Image ID="imgPreviewProd" runat="server" CssClass="preview-img mt-2" Visible="false" Style="max-height:150px; object-fit:contain; border-radius:10px;" />
                        </div>
                        <div class="col-12 mb-3">
                            <label class="form-label fw-semibold">Descripción</label>
                            <asp:TextBox ID="txtDescripcionProd" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="3" placeholder="Descripción del producto" />
                        </div>
                    </div>

                    <!-- Imágenes múltiples (carrusel) -->
                    <asp:UpdatePanel ID="upCarrusel" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="true">
                        <ContentTemplate>
                            <asp:Button ID="btnSubirMasImagenes" runat="server" Text="Subir más imágenes" CssClass="btn-outline-custom mb-3" OnClick="btnSubirMasImagenes_Click" />
                            <asp:Panel ID="pnlCarrusel" runat="server" Visible="false" CssClass="mt-3 pt-3 border-top">
                                <h6 class="fw-bold mb-3"><i class="fas fa-images me-2"></i>Imágenes adicionales</h6>
                                <div id="carouselProducto" class="carousel slide mb-3" data-bs-ride="carousel" style="max-width:400px;">
                                    <div class="carousel-inner">
                                        <asp:Repeater ID="rptImagenes" runat="server">
    <ItemTemplate>
        <div class='carousel-item <%# Container.ItemIndex == 0 ? "active" : "" %>'>
            <img src='<%# ObtenerUrlImagenCarrusel(Eval("img_ruta")) %>' class="d-block w-100" style="height:200px; object-fit:contain; background:#f8fafc; border-radius:12px;" />
        </div>
    </ItemTemplate>
</asp:Repeater>
                                    </div>
                                    <button class="carousel-control-prev" type="button" data-bs-target="#carouselProducto" data-bs-slide="prev">
                                        <span class="carousel-control-prev-icon"></span>
                                    </button>
                                    <button class="carousel-control-next" type="button" data-bs-target="#carouselProducto" data-bs-slide="next">
                                        <span class="carousel-control-next-icon"></span>
                                    </button>
                                </div>
                                <div class="d-flex flex-wrap gap-2">
                                    <asp:Repeater ID="rptThumbs" runat="server">
                                        <ItemTemplate>
                                           <asp:ImageButton ID="btnThumb" runat="server" ImageUrl='<%# ObtenerUrlImagenCarrusel(Eval("img_ruta")) %>'
    CssClass='carousel-thumb <%# Container.ItemIndex == 0 ? "active" : "" %>'
    OnClick="btnThumb_Click" CommandArgument='<%# Eval("Id") %>' />
                                        </ItemTemplate>
                                    </asp:Repeater>
                                </div>
                                <div class="mt-3 d-flex gap-2 align-items-center">
                                    <asp:FileUpload ID="fuImagenCarrusel" runat="server" CssClass="form-control form-control-sm" accept=".jpg,.jpeg,.png" style="max-width:250px;" />
                                    <asp:Button ID="btnAgregarImagen" runat="server" Text="Agregar" CssClass="btn btn-sm btn-outline-primary" OnClick="btnAgregarImagen_Click" />
                                    <asp:Button ID="btnEliminarImagen" runat="server" Text="Eliminar seleccionada" CssClass="btn btn-sm btn-outline-danger" OnClick="btnEliminarImagen_Click" />
                                </div>
                            </asp:Panel>
                            <asp:Label ID="lblSinImagenes" runat="server" CssClass="text-muted mt-3" Visible="false">No hay imágenes adicionales.</asp:Label>
                        </ContentTemplate>
                        <Triggers>
                            <asp:PostBackTrigger ControlID="btnAgregarImagen" />
                            <asp:PostBackTrigger ControlID="btnEliminarImagen" />
                        </Triggers>
                    </asp:UpdatePanel>

                    <div class="d-flex gap-2 mt-4">
                        <asp:Button ID="btnGuardarProd" runat="server" Text="Guardar producto" CssClass="btn-success-custom" OnClick="btnGuardarProd_Click" />
                        <asp:Button ID="btnCancelarProd" runat="server" Text="Cancelar" CssClass="btn-outline-custom" OnClick="btnCancelarProd_Click" />
                    </div>
                    <asp:HiddenField ID="hdnProdId" runat="server" />
                    <asp:HiddenField ID="hdnImagenSeleccionada" runat="server" />
                </asp:Panel>

                <!-- TABLA -->
                <div class="panel table-card">
                    <div class="table-responsive">
                        <asp:GridView ID="gvProductos" runat="server" CssClass="table" AutoGenerateColumns="false"
                            DataKeyNames="pro_id" AllowPaging="false" OnRowCommand="gvProductos_RowCommand" GridLines="None">
                            <Columns>
                                <asp:TemplateField HeaderText="Imagen">
                                    <ItemTemplate>
                                        <div class="prod-img-cell">
                                            <asp:Image ID="imgProd" runat="server" ImageUrl='<%# ObtenerUrlImagen(Eval("pro_ruta_imagen")) %>' />
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:BoundField DataField="pro_nombre" HeaderText="Nombre" />
                                <asp:BoundField DataField="Categoria" HeaderText="Categoría" />
                                <asp:BoundField DataField="Proveedor" HeaderText="Proveedor" />
                                <asp:BoundField DataField="pro_precio" HeaderText="Precio" DataFormatString="{0:C}" />
                                <asp:BoundField DataField="pro_stock" HeaderText="Stock" />
                                <asp:TemplateField HeaderText="Acciones">
                                    <ItemTemplate>
                                        <div class="action-btns">
                                            <asp:Button ID="btnEditarProd" runat="server" Text="Editar" CssClass="btn btn-sm btn-outline-warning"
                                                CommandName="Editar" CommandArgument='<%# Eval("pro_id") %>' />
                                            <asp:Button ID="btnDesactivarProd" runat="server"
                                                Text='<%# Eval("pro_estado").ToString() == "A" ? "Desactivar" : "Activar" %>'
                                                CssClass='<%# "btn btn-sm " + (Eval("pro_estado").ToString() == "A" ? "btn-outline-danger" : "btn-outline-success") %>'
                                                CommandName="ToggleEstado" CommandArgument='<%# Eval("pro_id") %>' />
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                        </asp:GridView>
                    </div>

                    <!-- PAGINACION MEJORADA -->
                    <nav aria-label="Page navigation" class="mt-3 mb-2 d-flex justify-content-center align-items-center gap-3 flex-wrap">
                        <ul class="pagination mb-0">
                            <li class="page-item">
                                <asp:LinkButton ID="lnkPrimera" runat="server" CssClass="page-link" OnClick="lnkPrimera_Click" Text="««" />
                            </li>
                            <li class="page-item">
                                <asp:LinkButton ID="lnkAnterior" runat="server" CssClass="page-link" OnClick="lnkAnterior_Click" Text="«" />
                            </li>
                            <asp:Repeater ID="rptPaginas" runat="server">
                                <ItemTemplate>
                                    <li class='page-item <%# ((SistemaProductos.Productos)Container.Page).ObtenerClasePagina(Convert.ToInt32(Eval("Value"))) %>'>
                                        <asp:LinkButton ID="lnkPagina" runat="server" CssClass="page-link"
                                            Text='<%# Eval("Text") %>' OnClick="lnkPagina_Click" CommandArgument='<%# Eval("Value") %>' />
                                    </li>
                                </ItemTemplate>
                            </asp:Repeater>
                            <li class="page-item">
                                <asp:LinkButton ID="lnkSiguiente" runat="server" CssClass="page-link" OnClick="lnkSiguiente_Click" Text="»" />
                            </li>
                            <li class="page-item">
                                <asp:LinkButton ID="lnkUltima" runat="server" CssClass="page-link" OnClick="lnkUltima_Click" Text="»»" />
                            </li>
                        </ul>
                        <div class="d-flex align-items-center gap-2">
                            <span class="text-muted small">Tamaño:</span>
                            <asp:DropDownList ID="ddlPageSize" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlPageSize_Changed" CssClass="form-select form-select-sm w-auto">
                                <asp:ListItem Text="10" Value="10" />
                                <asp:ListItem Text="25" Value="25" />
                                <asp:ListItem Text="50" Value="50" />
                            </asp:DropDownList>
                        </div>
                    </nav>
                </div>
            </ContentTemplate>
            <Triggers>
                <asp:PostBackTrigger ControlID="btnPrevisualizarImg" />
                <asp:PostBackTrigger ControlID="btnGuardarProd" />
            </Triggers>
        </asp:UpdatePanel>
    </div>

    <script>
        function clearSearch() {
            document.getElementById('<%= txtFiltroNombre.ClientID %>').value = '';
            __doPostBack('<%= txtFiltroNombre.UniqueID %>', '');
        }
        function showMessage(title, icon) {
            Swal.fire({ title: title, icon: icon, toast: true, position: 'top-end', showConfirmButton: false, timer: 3000 });
        }
    </script>
    <script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/js/bootstrap.bundle.min.js"></script>
</asp:Content>