using CapaDatos.Modelos;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CapaDatos.Repositorios
{
    public class ProveedorRepository
    {
        private readonly IMongoCollection<Proveedor> _proveedores;
        private readonly IMongoCollection<Producto> _productos; // Referencia a la colección de productos

        public ProveedorRepository()
        {
            var db = ConexionMongo.ObtenerBaseDatos();
            _proveedores = db.GetCollection<Proveedor>("proveedores");
            _productos = db.GetCollection<Producto>("productos");
        }

        // Obtener todos los proveedores activos (prov_estado = "A")
        public List<Proveedor> ObtenerActivos()
        {
            return _proveedores.Find(p => p.prov_estado == "A")
                               .SortBy(p => p.prov_nombre)
                               .ToList();
        }

        // Obtener todos los proveedores sin filtrar por estado
        public List<Proveedor> ObtenerTodos()
        {
            return _proveedores.Find(Builders<Proveedor>.Filter.Empty)
                               .SortBy(p => p.prov_nombre)
                               .ToList();
        }

        // Obtener un proveedor por su Id (string ObjectId)
        public Proveedor ObtenerPorId(string id)
        {
            if (!ObjectId.TryParse(id, out ObjectId objectId))
                return null;

            return _proveedores.Find(p => p.Id == id).FirstOrDefault();
        }

        // Insertar un nuevo proveedor, retorna el Id generado
        public string Insertar(Proveedor prov)
        {
            _proveedores.InsertOne(prov);
            return prov.Id; // MongoDB asignará el Id automáticamente si está vacío
        }

        // Actualizar un proveedor existente (reemplazo completo)
        public void Actualizar(Proveedor prov)
        {
            _proveedores.ReplaceOne(p => p.Id == prov.Id, prov);
        }

        // Desactivar proveedor: cambia estado a "I" y quita el proveedor de todos sus productos activos
        public void Desactivar(string id)
        {
            // 1. Actualizar el proveedor
            var filter = Builders<Proveedor>.Filter.Eq(p => p.Id, id);
            var update = Builders<Proveedor>.Update.Set(p => p.prov_estado, "I");
            _proveedores.UpdateOne(filter, update);

            // 2. Quitar el proveedor de los productos activos asociados
            var productFilter = Builders<Producto>.Filter.Eq(p => p.ProveedorId, id);
            var productUpdate = Builders<Producto>.Update.Set(p => p.ProveedorId, null);
            _productos.UpdateMany(productFilter, productUpdate);
        }

        // Activar proveedor (cambia estado a "A")
        public void Activar(string id)
        {
            var filter = Builders<Proveedor>.Filter.Eq(p => p.Id, id);
            var update = Builders<Proveedor>.Update.Set(p => p.prov_estado, "A");
            _proveedores.UpdateOne(filter, update);
        }

        // Buscar proveedor por nombre exacto (para validar duplicados)
        public Proveedor ObtenerPorNombre(string nombre)
        {
            return _proveedores.Find(p => p.prov_nombre == nombre && p.prov_estado == "A")
                               .FirstOrDefault();
        }

        // Obtener todos los proveedores con la cantidad de productos activos asociados
        public List<ProveedorConCantidadProjection> ObtenerConCantidadProductos()
        {
            var pipeline = new[]
            {
        // Convertir el _id (ObjectId) a string para compararlo con ProveedorId (string) en productos
        new BsonDocument("$addFields", new BsonDocument
        {
            { "provIdStr", new BsonDocument("$toString", "$_id") }
        }),
        new BsonDocument("$lookup", new BsonDocument
        {
            { "from", "productos" },
            { "localField", "provIdStr" },
            { "foreignField", "ProveedorId" },
            { "as", "productosAsociados" }
        }),
        new BsonDocument("$project", new BsonDocument
        {
            { "_id", 1 },
            { "prov_nombre", 1 },
            { "prov_estado", 1 },
            { "CantidadProductos", new BsonDocument("$size", new BsonDocument("$filter", new BsonDocument
                {
                    { "input", "$productosAsociados" },
                    { "as", "p" },
                    { "cond", new BsonDocument("$eq", new BsonArray { "$$p.pro_estado", "A" }) }
                }))
            }
        }),
        new BsonDocument("$sort", new BsonDocument("prov_nombre", 1))
    };

            return _proveedores.Aggregate<ProveedorConCantidadProjection>(pipeline).ToList();
        }

        // Top 5 proveedores activos con más productos activos
        public List<ProveedorConCantidadProjection> ObtenerTopProveedoresConMasProductos()
        {
            var pipeline = new[]
            {
        new BsonDocument("$match", new BsonDocument("prov_estado", "A")),
        new BsonDocument("$addFields", new BsonDocument
        {
            { "provIdStr", new BsonDocument("$toString", "$_id") }
        }),
        new BsonDocument("$lookup", new BsonDocument
        {
            { "from", "productos" },
            { "localField", "provIdStr" },
            { "foreignField", "ProveedorId" },
            { "as", "productosAsociados" }
        }),
        new BsonDocument("$project", new BsonDocument
        {
            { "_id", 1 },
            { "prov_nombre", 1 },
            { "prov_estado", 1 },
            { "CantidadProductos", new BsonDocument("$size", new BsonDocument("$filter", new BsonDocument
                {
                    { "input", "$productosAsociados" },
                    { "as", "p" },
                    { "cond", new BsonDocument("$eq", new BsonArray { "$$p.pro_estado", "A" }) }
                }))
            }
        }),
        new BsonDocument("$sort", new BsonDocument("CantidadProductos", -1)),
        new BsonDocument("$limit", 5)
    };

            return _proveedores.Aggregate<ProveedorConCantidadProjection>(pipeline).ToList();
        }

        // Buscar proveedor por nombre exacto (activo). Si no existe, lo crea y devuelve su Id
        public string ObtenerOCrear(string nombre)
        {
            var existente = ObtenerPorNombre(nombre);
            if (existente != null)
                return existente.Id;

            var nuevo = new Proveedor { prov_nombre = nombre, prov_estado = "A" };
            Insertar(nuevo);
            return nuevo.Id;
        }
    }

    // Clase auxiliar para los resultados de agregación con cantidad de productos
    public class ProveedorConCantidadProjection
    {
        // El ObjectId se mapea desde _id
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string prov_id { get; set; }

        public string prov_nombre { get; set; }
        public string prov_estado { get; set; }
        public int CantidadProductos { get; set; }
    }
}