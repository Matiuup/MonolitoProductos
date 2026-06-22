using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CapaDatos.Modelos
{
    public class Producto
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public string pro_codigo { get; set; }         // Código único para carga masiva
        public string pro_nombre { get; set; }
        public string pro_descripcion { get; set; }
        public decimal pro_precio { get; set; }
        public int pro_stock { get; set; }

        // Referencia a la categoría (ObjectId como string)
        public string CategoriaId { get; set; }

        // Referencia al proveedor (ObjectId como string, nullable)
        public string ProveedorId { get; set; }         // <-- ¡IMPORTANTE! Para la relación con Proveedor

        public string pro_estado { get; set; } = "A";   // "A" = Activo, "I" = Inactivo
        public string pro_ruta_imagen { get; set; }     // Ruta relativa de la imagen principal
    }
}