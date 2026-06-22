using CapaDatos.Modelos;
using CapaDatos.Repositorios;
using ExcelDataReader;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;

namespace CapaNegocio
{
    public class CN_Producto
    {
        private readonly ProductoRepository _repo = new ProductoRepository();
        private readonly CN_Categoria _cnCat = new CN_Categoria();
        private readonly CN_Proveedor _cnProv = new CN_Proveedor();

        public List<object> ListarActivosConCategoria()
        {
            int total;
            return _repo.BuscarPaginado(null, null, null, null, "A", null, 1, int.MaxValue, out total);
        }

        public Producto ObtenerPorId(string proId)
        {
            return _repo.ObtenerPorId(proId);
        }

        // Insertar producto, opcionalmente con ruta de imagen
        public string Insertar(string nombre, string descripcion, decimal precio, int stock,
                               string categoriaId, string proveedorId, string rutaImagen = null)
        {
            ValidarProducto(nombre, precio, stock, categoriaId);

            var prod = new Producto
            {
                pro_nombre = nombre,
                pro_descripcion = descripcion,
                pro_precio = precio,
                pro_stock = stock,
                CategoriaId = categoriaId,
                ProveedorId = string.IsNullOrEmpty(proveedorId) ? null : proveedorId,
                pro_ruta_imagen = rutaImagen,
                pro_estado = "A"
            };
            return _repo.Insertar(prod);
        }

        public void Actualizar(string proId, string nombre, string descripcion, decimal precio, int stock,
                               string categoriaId, string proveedorId)
        {
            ValidarProducto(nombre, precio, stock, categoriaId);

            var prod = _repo.ObtenerPorId(proId);
            if (prod == null) throw new Exception("Producto no encontrado.");

            prod.pro_nombre = nombre;
            prod.pro_descripcion = descripcion;
            prod.pro_precio = precio;
            prod.pro_stock = stock;
            prod.CategoriaId = categoriaId;
            prod.ProveedorId = string.IsNullOrEmpty(proveedorId) ? null : proveedorId;

            _repo.Actualizar(prod);
        }

        public void Desactivar(string proId) => _repo.Desactivar(proId);
        public void Activar(string proId) => _repo.Activar(proId);

        private void ValidarProducto(string nombre, decimal precio, int stock, string categoriaId)
        {
            if (string.IsNullOrWhiteSpace(nombre))
                throw new Exception("El nombre del producto es obligatorio.");
            if (precio <= 0)
                throw new Exception("El precio debe ser mayor a cero.");
            if (stock < 0)
                throw new Exception("El stock no puede ser negativo.");

            var cat = _cnCat.ObtenerPorId(categoriaId);
            if (cat == null || cat.Estado != "A")
                throw new Exception("La categoría seleccionada no es válida o está inactiva.");
        }

        public List<object> BuscarPaginado(string nombre, string categoriaId, decimal? precioMin, decimal? precioMax,
                                            string estado, string proveedorId, int pagina, int tamañoPagina, out int total)
        {
            return _repo.BuscarPaginado(nombre, categoriaId, precioMin, precioMax, estado, proveedorId, pagina, tamañoPagina, out total);
        }

        public bool ActualizarRutaImagen(string proId, string ruta)
        {
            try
            {
                _repo.ActualizarRutaImagen(proId, ruta);
                return true;
            }
            catch { return false; }
        }

        // ------------------------------------------
        // Métodos para carga masiva (Excel)
        // ------------------------------------------
        public class FilaCargaProducto
        {
            public string Codigo { get; set; }
            public string Nombre { get; set; }
            public string Descripcion { get; set; }
            public decimal Precio { get; set; }
            public int Stock { get; set; }
            public string Categoria { get; set; }
            public string Proveedor { get; set; }
            public string Estado { get; set; }
        }

        public List<FilaCargaProducto> PrevisualizarCarga(Stream fileStream)
        {
            var resultado = new List<FilaCargaProducto>();
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

            using (var reader = ExcelReaderFactory.CreateReader(fileStream))
            {
                var ds = reader.AsDataSet();
                var tabla = ds.Tables[0];

                for (int i = 1; i < tabla.Rows.Count; i++)
                {
                    var row = tabla.Rows[i];
                    if (row.IsNull(0) && row.IsNull(1)) break;

                    var fila = new FilaCargaProducto();
                    try
                    {
                        fila.Codigo = row[0]?.ToString().Trim();
                        fila.Nombre = row[1]?.ToString().Trim();
                        fila.Descripcion = row[2]?.ToString().Trim();
                        fila.Precio = Convert.ToDecimal(row[3]);
                        fila.Stock = Convert.ToInt32(row[4]);
                        fila.Categoria = row[5]?.ToString().Trim();
                        fila.Proveedor = row[6]?.ToString().Trim();
                    }
                    catch
                    {
                        fila.Estado = "Error: formato inválido en una columna";
                        resultado.Add(fila);
                        continue;
                    }

                    if (string.IsNullOrEmpty(fila.Codigo))
                        fila.Estado = "Error: código vacío";
                    else if (string.IsNullOrEmpty(fila.Nombre))
                        fila.Estado = "Error: nombre vacío";
                    else if (fila.Precio <= 0)
                        fila.Estado = "Error: precio debe ser mayor a 0";
                    else if (fila.Stock < 0)
                        fila.Estado = "Error: stock negativo";
                    else if (!string.IsNullOrEmpty(fila.Categoria) && _cnCat.ObtenerPorNombre(fila.Categoria) == null)
                        fila.Estado = "Error: categoría '" + fila.Categoria + "' no existe";
                    else
                    {
                        var existente = _repo.ObtenerPorCodigo(fila.Codigo);
                        fila.Estado = existente != null ? "Actualizar" : "Nuevo";
                    }

                    resultado.Add(fila);
                }
            }
            return resultado;
        }

        public int EjecutarCargaMasiva(List<FilaCargaProducto> filas)
        {
            int procesados = 0;
            var productosLote = new List<Producto>();

            foreach (var fila in filas)
            {
                if (fila.Estado.StartsWith("Error")) continue;

                string catId = _cnCat.ObtenerPorNombre(fila.Categoria)?.Id;
                if (string.IsNullOrEmpty(catId)) continue;

                string provId = null;
                if (!string.IsNullOrEmpty(fila.Proveedor))
                    provId = _cnProv.ObtenerOCrear(fila.Proveedor);

                productosLote.Add(new Producto
                {
                    pro_codigo = fila.Codigo,
                    pro_nombre = fila.Nombre,
                    pro_descripcion = fila.Descripcion,
                    pro_precio = fila.Precio,
                    pro_stock = fila.Stock,
                    CategoriaId = catId,
                    ProveedorId = provId,
                    pro_estado = "A"
                });
                procesados++;
            }

            _repo.InsertarLote(productosLote);
            return procesados;
        }

        // ------------------------------------------
        // Datos para gráficos y tarjetas
        // ------------------------------------------
        public DataTable ObtenerCantidadPorCategoria()
        {
            var dic = _repo.ObtenerCantidadPorCategoria();
            DataTable dt = new DataTable();
            dt.Columns.Add("cat_nombre");
            dt.Columns.Add("Cantidad", typeof(int));
            foreach (var kvp in dic)
                dt.Rows.Add(kvp.Key, kvp.Value);
            return dt;
        }

        public List<Producto> ObtenerProductosConImagen() => _repo.ObtenerProductosConImagen();
        public List<Producto> ObtenerTop5Caros() => _repo.ObtenerTopPrecio(true);
        public List<Producto> ObtenerTop5Baratos() => _repo.ObtenerTopPrecio(false);
        public List<Producto> ObtenerTop5MasStock() => _repo.ObtenerTopMasStock();
    }
}