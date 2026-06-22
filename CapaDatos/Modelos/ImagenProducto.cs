using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CapaDatos.Modelos
{
    public class ImagenProducto
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        // Referencia al producto (ObjectId como string)
        public string ProductoId { get; set; }

        public string img_ruta { get; set; }
        public string img_nombre_archivo { get; set; }
        public bool img_principal { get; set; }
        public string img_estado { get; set; } = "A";   // "A" = Activo, "I" = Inactivo
    }
}