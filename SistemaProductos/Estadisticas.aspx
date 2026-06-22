<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Estadisticas.aspx.cs" Inherits="SistemaProductos.Estadisticas" MasterPageFile="~/Site.Master" %>
<%@ Register Assembly="System.Web.DataVisualization, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35"
    Namespace="System.Web.UI.DataVisualization.Charting" TagPrefix="asp" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <style>
        :root {
            --azul: #2563eb;
            --verde: #10b981;
            --naranja: #f59e0b;
            --rojo: #ef4444;
            --cyan: #06b6d4;
            --texto: #1e293b;
            --texto-sec: #64748b;
            --borde: #eef2f7;
        }

        .stats-wrapper { font-family: 'Inter', 'Segoe UI', sans-serif; padding: 8px 4px 32px; }

        .page-head {
            display: flex;
            align-items: center;
            gap: 16px;
            margin-bottom: 28px;
        }
        .page-head .head-icon {
            width: 52px; height: 52px;
            border-radius: 14px;
            background: linear-gradient(135deg, var(--azul), var(--cyan));
            display: flex; align-items: center; justify-content: center;
            color: #fff; font-size: 22px;
            box-shadow: 0 8px 20px -6px rgba(37,99,235,.5);
        }
        .page-head h2 { font-size: 26px; font-weight: 700; color: var(--texto); margin: 0; }
        .page-head p { font-size: 14px; color: var(--texto-sec); margin: 0; }

        /* Tarjetas KPI */
        .kpi-grid {
            display: grid;
            grid-template-columns: repeat(3, 1fr);
            gap: 18px;
            margin-bottom: 24px;
        }
        .kpi-card {
            background: #fff;
            border: 1px solid var(--borde);
            border-radius: 18px;
            padding: 20px;
            display: flex;
            align-items: center;
            gap: 16px;
            transition: all .25s ease;
        }
        .kpi-card:hover { transform: translateY(-4px); box-shadow: 0 16px 32px -16px rgba(0,0,0,.18); }
        .kpi-icon {
            width: 52px; height: 52px; border-radius: 14px; flex-shrink: 0;
            display: flex; align-items: center; justify-content: center; font-size: 20px;
        }
        .kpi-icon.rojo { background: #fef2f2; color: var(--rojo); }
        .kpi-icon.verde { background: #f0fdf4; color: var(--verde); }
        .kpi-icon.naranja { background: #fffbeb; color: var(--naranja); }
        .kpi-card label { font-size: 12px; color: var(--texto-sec); font-weight: 600; text-transform: uppercase; letter-spacing: .4px; display: block; margin-bottom: 4px; }
        .kpi-card .kpi-value { font-size: 17px; font-weight: 700; color: var(--texto); }

        /* Tarjetas de gráficos */
        .charts-grid {
            display: grid;
            grid-template-columns: repeat(2, 1fr);
            gap: 18px;
            margin-bottom: 24px;
        }
        .panel {
            background: #fff;
            border: 1px solid var(--borde);
            border-radius: 20px;
            padding: 22px;
            box-shadow: 0 4px 18px -10px rgba(0,0,0,.08);
        }
        .panel-head {
            display: flex; align-items: center; gap: 10px;
            margin-bottom: 16px; padding-bottom: 14px;
            border-bottom: 1px solid var(--borde);
        }
        .panel-head .dot { width: 36px; height: 36px; border-radius: 10px; display: flex; align-items: center; justify-content: center; font-size: 16px; }
        .panel-head .dot.azul { background: #eff6ff; color: var(--azul); }
        .panel-head .dot.verde { background: #f0fdf4; color: var(--verde); }
        .panel-head .dot.cyan { background: #ecfeff; color: var(--cyan); }
        .panel-head h5 { font-size: 16px; font-weight: 700; color: var(--texto); margin: 0; }
        .chart-box { display: flex; justify-content: center; overflow: hidden; }
        .chart-box img {
            max-width: 100%;
            height: auto;
        }
        /* Carrusel */
        .carousel-inner img {
            height: 320px;
            object-fit: contain;
            background: #f8fafc;
            border-radius: 14px;
        }
        .carousel-caption {
            background: rgba(15,23,42,.7);
            border-radius: 10px;
            padding: 8px 16px;
            left: 15%; right: 15%; bottom: 18px;
        }
        .carousel-caption h5 { font-size: 16px; font-weight: 600; margin: 0; }
        .carousel-control-prev-icon, .carousel-control-next-icon {
            background-color: rgba(37,99,235,.85);
            border-radius: 50%;
            padding: 14px;
            background-size: 50%;
        }

        @media (max-width: 992px) {
            .charts-grid { grid-template-columns: 1fr; }
            .kpi-grid { grid-template-columns: 1fr; }
        }
    </style>

    <div class="stats-wrapper">

        <div class="page-head">
            <div class="head-icon"><i class="fas fa-chart-pie"></i></div>
            <div>
                <h2>Estadísticas</h2>
                <p>Resumen visual del inventario y productos destacados</p>
            </div>
        </div>

        <!-- KPIs -->
        <div class="kpi-grid">
            <div class="kpi-card">
                <div class="kpi-icon rojo"><i class="fas fa-arrow-trend-up"></i></div>
                <div>
                    <label>Producto más caro</label>
                    <asp:Label ID="lblProductoCaro" runat="server" CssClass="kpi-value" />
                </div>
            </div>
            <div class="kpi-card">
                <div class="kpi-icon verde"><i class="fas fa-arrow-trend-down"></i></div>
                <div>
                    <label>Producto más barato</label>
                    <asp:Label ID="lblProductoBarato" runat="server" CssClass="kpi-value" />
                </div>
            </div>
            <div class="kpi-card">
                <div class="kpi-icon naranja"><i class="fas fa-cubes"></i></div>
                <div>
                    <label>Mayor stock</label>
                    <asp:Label ID="lblProductoStock" runat="server" CssClass="kpi-value" />
                </div>
            </div>
        </div>

        <!-- Gráficos -->
        <div class="charts-grid">
            <div class="panel">
                <div class="panel-head">
                    <div class="dot azul"><i class="fas fa-chart-pie"></i></div>
                    <h5>Productos por categoría</h5>
                </div>
                <div class="chart-box">
                    <asp:Chart ID="chartCategorias" runat="server" Width="520px" Height="400px"
    Palette="BrightPastel" AntiAliasing="All" TextAntiAliasingQuality="High"
    ImageType="Png" RenderType="ImageTag" BackColor="Transparent">
    <Series>
        <asp:Series Name="Categorias" ChartType="Pie"
            IsValueShownAsLabel="true" LabelFormat="{0} ({1}%)"
            Font="Segoe UI, 10pt" BorderWidth="1" BorderColor="White">
            <SmartLabelStyle Enabled="true" />
        </asp:Series>
    </Series>
    <ChartAreas>
        <asp:ChartArea Name="MainArea" BackColor="Transparent" BorderColor="Transparent">
            <Area3DStyle Enable3D="false" />
        </asp:ChartArea>
    </ChartAreas>
    <Legends>
        <asp:Legend Alignment="Center" Docking="Bottom" Font="Segoe UI, 9pt" BackColor="Transparent" />
    </Legends>
</asp:Chart>
                </div>
            </div>

            <div class="panel">
                <div class="panel-head">
                    <div class="dot verde"><i class="fas fa-chart-bar"></i></div>
                    <h5>Proveedores con más productos</h5>
                </div>
                <div class="chart-box">
                    <asp:Chart ID="chartProveedores" runat="server" Width="520px" Height="400px"
    Palette="SeaGreen" AntiAliasing="All" TextAntiAliasingQuality="High"
    ImageType="Png" RenderType="ImageTag" BackColor="Transparent">
    <Series>
        <asp:Series Name="Proveedores" ChartType="Bar"
            IsValueShownAsLabel="true" Font="Segoe UI, 9pt"
            BorderWidth="0" Color="#10b981">
            <SmartLabelStyle Enabled="true" />
        </asp:Series>
    </Series>
    <ChartAreas>
        <asp:ChartArea Name="MainArea" BackColor="Transparent" BorderColor="Transparent">
            <AxisX LineColor="#cbd5e1" LabelStyle-Font="Segoe UI, 9pt">
                <MajorGrid Enabled="false" />
            </AxisX>
            <AxisY Title="Cantidad" TitleFont="Segoe UI, 9pt" LineColor="#cbd5e1" LabelStyle-Font="Segoe UI, 9pt">
                <MajorGrid LineColor="#eef2f7" />
            </AxisY>
        </asp:ChartArea>
    </ChartAreas>
    <Legends>
        <asp:Legend Enabled="false" />
    </Legends>
</asp:Chart>
                </div>
            </div>
        </div>

        <!-- Carrusel -->
        <div class="panel">
            <div class="panel-head">
                <div class="dot cyan"><i class="fas fa-images"></i></div>
                <h5>Productos Destacados</h5>
            </div>
            <div id="carouselDestacados" class="carousel slide" data-bs-ride="carousel">
                <div class="carousel-inner">
                    <asp:Repeater ID="rptCarrusel" runat="server">
                        <ItemTemplate>
                            <div class='carousel-item <%# Container.ItemIndex == 0 ? "active" : "" %>'>
                                <img src='<%# ResolveUrl(Eval("pro_ruta_imagen")?.ToString() ?? "Images/no-image.png") %>' class="d-block w-100" />
                                <div class="carousel-caption d-none d-md-block">
                                    <h5><%# Eval("pro_nombre") %></h5>
                                </div>
                            </div>
                        </ItemTemplate>
                    </asp:Repeater>
                </div>
                <button class="carousel-control-prev" type="button" data-bs-target="#carouselDestacados" data-bs-slide="prev">
                    <span class="carousel-control-prev-icon"></span>
                </button>
                <button class="carousel-control-next" type="button" data-bs-target="#carouselDestacados" data-bs-slide="next">
                    <span class="carousel-control-next-icon"></span>
                </button>
            </div>
        </div>

    </div>
    <script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/js/bootstrap.bundle.min.js"></script>
</asp:Content>