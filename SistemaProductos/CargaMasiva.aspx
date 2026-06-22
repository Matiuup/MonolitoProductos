<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CargaMasiva.aspx.cs" Inherits="SistemaProductos.CargaMasiva" MasterPageFile="~/Site.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
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

        .carga-wrapper { font-family: 'Inter','Segoe UI',sans-serif; color: var(--text); padding-bottom: 32px; }

        /* HERO */
        .carga-hero {
            background: linear-gradient(120deg, #2563eb 0%, #1e40af 55%, #06b6d4 130%);
            border-radius: 22px;
            padding: 26px 30px;
            display: flex;
            align-items: center;
            gap: 16px;
            margin-bottom: 22px;
            color: #fff;
            box-shadow: 0 18px 40px -18px rgba(37,99,235,.6);
        }
        .carga-hero .hero-icon {
            width: 56px; height: 56px; border-radius: 16px;
            background: rgba(255,255,255,.18);
            display: flex; align-items: center; justify-content: center;
            font-size: 24px; backdrop-filter: blur(6px);
        }
        .carga-hero h2 { font-size: 26px; font-weight: 800; margin: 0; letter-spacing: -.5px; }
        .carga-hero p { margin: 2px 0 0; font-size: 13.5px; opacity: .85; }

        .panel {
            background: #fff;
            border: 1px solid var(--border);
            border-radius: 20px;
            box-shadow: 0 6px 24px -14px rgba(0,0,0,.12);
        }

        /* ZONA DE CARGA */
        .upload-card { padding: 28px; margin-bottom: 22px; }
        .upload-zone {
            border: 2px dashed #cbd5e1;
            border-radius: 16px;
            padding: 36px 24px;
            text-align: center;
            background: #f8fafc;
            transition: all .25s;
            cursor: pointer;
        }
        .upload-zone:hover, .upload-zone.dragover { border-color: var(--primary); background: #eff6ff; }
        .upload-zone .up-icon {
            width: 64px; height: 64px; border-radius: 18px; margin: 0 auto 14px;
            background: linear-gradient(135deg, var(--primary), var(--accent));
            display: flex; align-items: center; justify-content: center;
            font-size: 26px; color: #fff;
            box-shadow: 0 10px 24px -8px rgba(37,99,235,.55);
        }
        .upload-zone h5 { font-weight: 700; font-size: 16px; margin: 0 0 4px; }
        .upload-zone p { color: var(--text-muted); font-size: 13px; margin: 0; }
        .upload-zone .file-name { margin-top: 12px; font-weight: 600; color: var(--primary); font-size: 13.5px; }

        /* Ocultamos el FileUpload nativo pero conservamos el control */
        .upload-card .hidden-file input[type=file] { display: none; }

        /* Formatos / instrucciones */
        .format-badges { display: flex; gap: 8px; justify-content: center; margin-top: 16px; flex-wrap: wrap; }
        .format-chip {
            display: inline-flex; align-items: center; gap: 6px;
            background: #fff; border: 1px solid var(--border); color: var(--text-muted);
            padding: 6px 14px; border-radius: 50px; font-size: 12px; font-weight: 600;
        }
        .format-chip i { color: var(--success); }

        /* BOTONES */
        .btn-primary-custom {
            background: var(--primary); border: none; padding: 11px 24px; font-weight: 600;
            border-radius: 12px; color: #fff; transition: all .2s; box-shadow: 0 4px 12px rgba(37,99,235,.25);
        }
        .btn-primary-custom:hover { background: var(--primary-dark); transform: translateY(-1px); }
        .btn-success-custom {
            background: var(--success); border: none; padding: 11px 24px; font-weight: 600;
            border-radius: 12px; color: #fff; transition: all .2s; box-shadow: 0 4px 12px rgba(16,185,129,.25);
        }
        .btn-success-custom:hover { background: #059669; transform: translateY(-1px); }

        /* RESUMEN contadores */
        .summary-grid { display: flex; gap: 14px; flex-wrap: wrap; margin-bottom: 18px; }
        .summary-item {
            flex: 1; min-width: 140px; padding: 16px 18px; border-radius: 14px;
            display: flex; align-items: center; gap: 12px; border: 1px solid var(--border);
        }
        .summary-item .s-icon { width: 40px; height: 40px; border-radius: 11px; display: flex; align-items: center; justify-content: center; font-size: 16px; }
        .summary-item.total .s-icon { background: #eff6ff; color: var(--primary); }
        .summary-item.nuevo .s-icon { background: #f0fdf4; color: var(--success); }
        .summary-item.error .s-icon { background: #fef2f2; color: var(--danger); }
        .summary-item label { font-size: 11.5px; color: var(--text-muted); text-transform: uppercase; font-weight: 600; letter-spacing: .3px; display: block; }
        .summary-item .s-val { font-size: 20px; font-weight: 800; }

        /* TABLA */
        .preview-card { padding: 22px; }
        .preview-head { display: flex; align-items: center; gap: 10px; margin-bottom: 18px; }
        .preview-head .badge-icon { width: 36px; height: 36px; border-radius: 10px; background: #ecfeff; color: var(--accent); display: flex; align-items: center; justify-content: center; }
        .preview-head h5 { margin: 0; font-weight: 700; font-size: 17px; }
        .preview-table { max-height: 420px; overflow-y: auto; border-radius: 14px; border: 1px solid var(--border); }
        .table { margin-bottom: 0; }
        .table thead th {
            background: #f8fafc; border: none; border-bottom: 2px solid var(--border);
            font-weight: 700; font-size: 11.5px; text-transform: uppercase; letter-spacing: .4px;
            color: var(--text-muted); padding: 12px 16px; position: sticky; top: 0; z-index: 1;
        }
        .table tbody td { padding: 11px 16px; vertical-align: middle; border-bottom: 1px solid var(--border); font-size: 13.5px; }
        .table tbody tr:hover { background: #f8fbff; }

        .badge-estado { padding: 4px 12px; border-radius: 50px; font-weight: 600; font-size: 11.5px; display: inline-flex; align-items: center; gap: 5px; }
        .badge-estado::before { content: ''; width: 6px; height: 6px; border-radius: 50%; background: currentColor; }
        .badge-estado.nuevo { background: #f0fdf4; color: var(--success); }
        .badge-estado.error { background: #fef2f2; color: var(--danger); }
        .badge-estado.existe { background: #fffbeb; color: var(--warning); }
    </style>

    <div class="carga-wrapper">
        <!-- HERO -->
        <div class="carga-hero">
            <div class="hero-icon"><i class="fas fa-file-arrow-up"></i></div>
            <div>
                <h2>Carga Masiva de Productos</h2>
                <p>Importa múltiples productos desde un archivo Excel o CSV</p>
            </div>
        </div>

        <!-- ZONA DE CARGA -->
        <div class="panel upload-card">
            <div class="upload-zone" id="dropZone" onclick="document.getElementById('<%= fuArchivo.ClientID %>').click();">
                <div class="up-icon"><i class="fas fa-cloud-arrow-up"></i></div>
                <h5>Arrastra tu archivo aquí o haz clic para seleccionar</h5>
                <p>Sube tu listado de productos para previsualizarlo antes de confirmar</p>
                <div class="hidden-file">
                    <asp:FileUpload ID="fuArchivo" runat="server" CssClass="form-control" accept=".xlsx,.xls,.csv" />
                </div>
                <div class="file-name" id="fileName"></div>
                <div class="format-badges">
                    <span class="format-chip"><i class="fas fa-check-circle"></i> XLSX</span>
                    <span class="format-chip"><i class="fas fa-check-circle"></i> XLS</span>
                    <span class="format-chip"><i class="fas fa-check-circle"></i> CSV</span>
                </div>
            </div>

            <div class="d-flex justify-content-center gap-2 mt-4">
                <asp:Button ID="btnPrevisualizar" runat="server" Text="Previsualizar" CssClass="btn-primary-custom" OnClick="btnPrevisualizar_Click" />
                <asp:Button ID="btnConfirmar" runat="server" Text="Confirmar carga" CssClass="btn-success-custom" Visible="false" OnClick="btnConfirmar_Click" />
            </div>
        </div>

        <!-- PREVIEW -->
        <asp:Panel ID="pnlPreview" runat="server" Visible="false" CssClass="panel preview-card">
            <div class="preview-head">
                <span class="badge-icon"><i class="fas fa-table-list"></i></span>
                <h5>Vista previa de la importación</h5>
            </div>

            <asp:Label ID="lblResumen" runat="server" CssClass="d-block mb-3 text-muted" />

            <div class="preview-table">
                <br /> 
                <br /> 
                <asp:GridView ID="gvPreview" runat="server" CssClass="table" AutoGenerateColumns="false" GridLines="None">
                    <Columns>
                        <asp:BoundField DataField="Codigo" HeaderText="Código" />
                        <asp:BoundField DataField="Nombre" HeaderText="Nombre" />
                        <asp:BoundField DataField="Precio" HeaderText="Precio" DataFormatString="{0:C}" />
                        <asp:BoundField DataField="Stock" HeaderText="Stock" />
                        <asp:BoundField DataField="Categoria" HeaderText="Categoría" />
                        <asp:BoundField DataField="Proveedor" HeaderText="Proveedor" />
                        <asp:TemplateField HeaderText="Estado">
                            <ItemTemplate>
                                <span class='badge-estado <%# Eval("Estado").ToString().StartsWith("Error") ? "error" : (Eval("Estado").ToString() == "Nuevo" ? "nuevo" : "existe") %>'>
                                    <%# Eval("Estado") %>
                                </span>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
            </div>
        </asp:Panel>
    </div>

    <script>
        // Mostrar nombre de archivo seleccionado y soporte drag & drop visual
        (function () {
            var fileInput = document.getElementById('<%= fuArchivo.ClientID %>');
            var dropZone = document.getElementById('dropZone');
            var fileNameLabel = document.getElementById('fileName');

            if (fileInput) {
                fileInput.addEventListener('change', function () {
                    fileNameLabel.textContent = this.value ? this.value.split('\\').pop() : '';
                });
            }
            if (dropZone) {
                ['dragenter', 'dragover'].forEach(function (ev) {
                    dropZone.addEventListener(ev, function (e) { e.preventDefault(); dropZone.classList.add('dragover'); });
                });
                ['dragleave', 'drop'].forEach(function (ev) {
                    dropZone.addEventListener(ev, function (e) { e.preventDefault(); dropZone.classList.remove('dragover'); });
                });
                dropZone.addEventListener('drop', function (e) {
                    if (e.dataTransfer.files.length) {
                        fileInput.files = e.dataTransfer.files;
                        fileNameLabel.textContent = e.dataTransfer.files[0].name;
                    }
                });
            }
        })();
    </script>

</asp:Content>