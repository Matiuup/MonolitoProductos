using CapaDatos.Modelos;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CapaDatos.Repositorios
{
    public class ProductoRepository
    {
        private readonly IMongoCollection<Producto> _productos;

        public ProductoRepository()
        {
            var db = ConexionMongo.ObtenerBaseDatos();
            _productos = db.GetCollection<Producto>("productos");
        }

        // Obtener producto por Id
        public Producto ObtenerPorId(string id)
        {
            if (!ObjectId.TryParse(id, out _)) return null;
            return _productos.Find(p => p.Id == id).FirstOrDefault();
        }

        // Insertar un producto (retorna el Id generado)
        public string Insertar(Producto producto)
        {
            _productos.InsertOne(producto);
            return producto.Id;
        }

        // Actualizar producto completo
        public void Actualizar(Producto producto)
        {
            _productos.ReplaceOne(p => p.Id == producto.Id, producto);
        }

        // Actualizar solo la ruta de la imagen principal
        public void ActualizarRutaImagen(string id, string ruta)
        {
            var filter = Builders<Producto>.Filter.Eq(p => p.Id, id);
            var update = Builders<Producto>.Update.Set(p => p.pro_ruta_imagen, ruta);
            _productos.UpdateOne(filter, update);
        }

        // Desactivar / Activar
        public void Desactivar(string id)
        {
            var filter = Builders<Producto>.Filter.Eq(p => p.Id, id);
            var update = Builders<Producto>.Update.Set(p => p.pro_estado, "I");
            _productos.UpdateOne(filter, update);
        }

        public void Activar(string id)
        {
            var filter = Builders<Producto>.Filter.Eq(p => p.Id, id);
            var update = Builders<Producto>.Update.Set(p => p.pro_estado, "A");
            _productos.UpdateOne(filter, update);
        }

        // Búsqueda paginada con filtros combinados y $lookup a categorías y proveedores

        public List<object> BuscarPaginado(string nombre, string categoriaId, decimal? precioMin, decimal? precioMax,
                                    string estado, string proveedorId, int pagina, int tamañoPagina, out int total)
        {
            var filterBuilder = Builders<Producto>.Filter;
            var filters = new List<FilterDefinition<Producto>>();

            if (estado == "A" || estado == "I")
                filters.Add(filterBuilder.Eq(p => p.pro_estado, estado));

            if (!string.IsNullOrWhiteSpace(nombre))
                filters.Add(filterBuilder.Regex(p => p.pro_nombre, new BsonRegularExpression(nombre, "i")));

            if (!string.IsNullOrEmpty(categoriaId) && ObjectId.TryParse(categoriaId, out _))
                filters.Add(filterBuilder.Eq(p => p.CategoriaId, categoriaId));

            if (!string.IsNullOrEmpty(proveedorId) && ObjectId.TryParse(proveedorId, out _))
                filters.Add(filterBuilder.Eq(p => p.ProveedorId, proveedorId));

            if (precioMin.HasValue)
                filters.Add(filterBuilder.Gte(p => p.pro_precio, precioMin.Value));
            if (precioMax.HasValue)
                filters.Add(filterBuilder.Lte(p => p.pro_precio, precioMax.Value));

            var combinedFilter = filters.Count > 0 ? filterBuilder.And(filters) : filterBuilder.Empty;

            // Total de registros
            total = (int)_productos.CountDocuments(combinedFilter);

            int skip = (pagina - 1) * tamañoPagina;

            // Construir la agregación sin Render, usando AppendStage para los $lookup y $project
            var resultados = _productos.Aggregate()
                .Match(combinedFilter)                // aquí el driver serializa el filtro automáticamente
                .SortBy(p => p.pro_nombre)
                .Skip(skip)
                .Limit(tamañoPagina)
                // Lookup a categorías usando $toObjectId
                .AppendStage<BsonDocument>(new BsonDocument("$lookup", new BsonDocument
                {
            { "from", "categorias" },
            { "let", new BsonDocument("catId", new BsonDocument("$toObjectId", "$CategoriaId")) },
            { "pipeline", new BsonArray
                {
                    new BsonDocument("$match", new BsonDocument("$expr",
                        new BsonDocument("$eq", new BsonArray { "$_id", "$$catId" })))
                }
            },
            { "as", "categoria_info" }
                }))
                // Lookup a proveedores
                .AppendStage<BsonDocument>(new BsonDocument("$lookup", new BsonDocument
                {
            { "from", "proveedores" },
            { "let", new BsonDocument("provId", new BsonDocument("$toObjectId", "$ProveedorId")) },
            { "pipeline", new BsonArray
                {
                    new BsonDocument("$match", new BsonDocument("$expr",
                        new BsonDocument("$eq", new BsonArray { "$_id", "$$provId" })))
                }
            },
            { "as", "proveedor_info" }
                }))
                // Proyectar los campos finales
                .AppendStage<BsonDocument>(new BsonDocument("$project", new BsonDocument
                {
            { "_id", 1 },
            { "pro_nombre", 1 },
            { "pro_precio", 1 },
            { "pro_stock", 1 },
            { "pro_ruta_imagen", 1 },
            { "pro_estado", 1 },
            { "Categoria", new BsonDocument("$ifNull", new BsonArray
                {
                    new BsonDocument("$arrayElemAt", new BsonArray { "$categoria_info.cat_nombre", 0 }),
                    BsonNull.Value
                })
            },
            { "Proveedor", new BsonDocument("$ifNull", new BsonArray
                {
                    new BsonDocument("$arrayElemAt", new BsonArray { "$proveedor_info.prov_nombre", 0 }),
                    "Sin proveedor"
                })
            }
                }))
                .ToList();

            // Mapear a lista de objetos anónimos para la grilla
            var lista = new List<object>();
            foreach (var doc in resultados)
            {
                lista.Add(new
                {
                    pro_id = doc["_id"].AsObjectId.ToString(),
                    pro_nombre = doc["pro_nombre"].AsString,
                    pro_precio = doc["pro_precio"].AsDecimal,
                    pro_stock = doc["pro_stock"].AsInt32,
                    pro_ruta_imagen = doc.Contains("pro_ruta_imagen") && !doc["pro_ruta_imagen"].IsBsonNull ? doc["pro_ruta_imagen"].AsString : null,
                    pro_estado = doc["pro_estado"].AsString,
                    Categoria = doc["Categoria"] != BsonNull.Value ? doc["Categoria"].AsString : "",
                    Proveedor = doc["Proveedor"].AsString
                });
            }
            return lista;
        }

        // Obtener productos activos con imagen (para destacados)
        public List<Producto> ObtenerProductosConImagen()
        {
            var filter = Builders<Producto>.Filter.Where(p => p.pro_estado == "A" && p.pro_ruta_imagen != null);
            return _productos.Find(filter).Limit(5).ToList();
        }

        // Cantidad de productos por categoría (con nombre)
        public Dictionary<string, int> ObtenerCantidadPorCategoria()
        {
            var pipeline = new[]
            {
        new BsonDocument("$match", new BsonDocument("pro_estado", "A")),
        new BsonDocument("$group", new BsonDocument
        {
            { "_id", "$CategoriaId" },
            { "count", new BsonDocument("$sum", 1) }
        }),
        // Convertir el string CategoriaId a ObjectId antes del lookup
        new BsonDocument("$addFields", new BsonDocument
        {
            { "catObjId", new BsonDocument("$toObjectId", "$_id") }
        }),
        new BsonDocument("$lookup", new BsonDocument
        {
            { "from", "categorias" },
            { "localField", "catObjId" },
            { "foreignField", "_id" },
            { "as", "cat" }
        }),
        new BsonDocument("$project", new BsonDocument
        {
            { "nombre", new BsonDocument("$ifNull", new BsonArray
                {
                    new BsonDocument("$arrayElemAt", new BsonArray { "$cat.cat_nombre", 0 }),
                    "Sin categoría"
                })
            },
            { "count", 1 }
        })
    };

            var aggResult = _productos.Aggregate<BsonDocument>(pipeline).ToList();
            var dic = new Dictionary<string, int>();
            foreach (var doc in aggResult)
            {
                string nombreCat = doc["nombre"].AsString;
                int count = doc["count"].AsInt32;
                dic[nombreCat] = count;
            }
            return dic;
        }

        // Top 5 productos por precio
        public List<Producto> ObtenerTopPrecio(bool caros = true)
        {
            var filter = Builders<Producto>.Filter.Eq(p => p.pro_estado, "A");
            var sort = caros ? Builders<Producto>.Sort.Descending(p => p.pro_precio) : Builders<Producto>.Sort.Ascending(p => p.pro_precio);
            return _productos.Find(filter).Sort(sort).Limit(5).ToList();
        }

        public List<Producto> ObtenerTopMasStock()
        {
            var filter = Builders<Producto>.Filter.Eq(p => p.pro_estado, "A");
            var sort = Builders<Producto>.Sort.Descending(p => p.pro_stock);
            return _productos.Find(filter).Sort(sort).Limit(5).ToList();
        }

        // Producto por código
        public Producto ObtenerPorCodigo(string codigo)
        {
            return _productos.Find(p => p.pro_codigo == codigo).FirstOrDefault();
        }

        // Carga masiva (upsert)
        public void InsertarLote(List<Producto> productos)
        {
            var bulkOps = new List<WriteModel<Producto>>();
            foreach (var prod in productos)
            {
                var filter = Builders<Producto>.Filter.Eq(p => p.pro_codigo, prod.pro_codigo);
                var update = Builders<Producto>.Update
                    .Set(p => p.pro_nombre, prod.pro_nombre)
                    .Set(p => p.pro_descripcion, prod.pro_descripcion)
                    .Set(p => p.pro_precio, prod.pro_precio)
                    .Set(p => p.pro_stock, prod.pro_stock)
                    .Set(p => p.CategoriaId, prod.CategoriaId)
                    .Set(p => p.ProveedorId, prod.ProveedorId)
                    .Set(p => p.pro_estado, prod.pro_estado);
                var upsert = new UpdateOneModel<Producto>(filter, update) { IsUpsert = true };
                bulkOps.Add(upsert);
            }
            if (bulkOps.Count > 0)
                _productos.BulkWrite(bulkOps);
        }
    }
}