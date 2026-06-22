using CapaNegocio;
using System;
using System.Data;
using System.Linq;
using System.Web.UI;
using System.Web.UI.DataVisualization.Charting;

namespace SistemaProductos
{
    public partial class Estadisticas : System.Web.UI.Page
    {
        CN_Producto cnProd = new CN_Producto();
        CN_Proveedor cnProv = new CN_Proveedor();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UsuarioId"] == null) Response.Redirect("~/Login.aspx");
            if (!IsPostBack)
            {
                CargarGraficos();
                CargarTarjetas();
                CargarCarrusel();
            }
        }

        private void CargarGraficos()
        {
            // Mejorar calidad de los gráficos
            AplicarCalidad(chartCategorias);
            AplicarCalidad(chartProveedores);

            // Gráfico de pastel: Productos por categoría
            DataTable dtCat = cnProd.ObtenerCantidadPorCategoria();
            chartCategorias.Series["Categorias"].Points.DataBind(
                dtCat.AsEnumerable(), "cat_nombre", "Cantidad", "");

            // Gráfico de barras: Top proveedores con más productos activos
            var topProv = cnProv.ObtenerTopProveedoresConMasProductos();
            foreach (var item in topProv)
            {
                chartProveedores.Series["Proveedores"].Points.AddXY(
                    item.prov_nombre, item.Cantidad);
            }
        }

        private void AplicarCalidad(Chart chart)
        {
            chart.AntiAliasing = AntiAliasingStyles.All;
            chart.TextAntiAliasingQuality = TextAntiAliasingQuality.High;
            chart.ImageType = ChartImageType.Png;
            chart.RenderType = RenderType.ImageTag;
        }

        private void CargarTarjetas()
        {
            // Producto más caro
            var caros = cnProd.ObtenerTop5Caros();
            if (caros.Any())
                lblProductoCaro.Text = $"{caros[0].pro_nombre} ({caros[0].pro_precio:C})";

            // Producto más barato
            var baratos = cnProd.ObtenerTop5Baratos();
            if (baratos.Any())
                lblProductoBarato.Text = $"{baratos[0].pro_nombre} ({baratos[0].pro_precio:C})";

            // Producto con mayor stock
            var stock = cnProd.ObtenerTop5MasStock();
            if (stock.Any())
                lblProductoStock.Text = $"{stock[0].pro_nombre} ({stock[0].pro_stock} unidades)";
        }

        private void CargarCarrusel()
        {
            var productos = cnProd.ObtenerProductosConImagen();
            rptCarrusel.DataSource = productos;
            rptCarrusel.DataBind();
        }
    }
}