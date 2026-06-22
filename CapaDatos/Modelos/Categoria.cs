using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CapaDatos.Modelos
{
    public class Categoria
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }  // MongoDB asigna automáticamente un ObjectId

        [BsonElement("cat_nombre")]
        public string Nombre { get; set; }

        [BsonElement("cat_descripcion")]
        public string Descripcion { get; set; }

        [BsonElement("cat_estado")]
        public string Estado { get; set; } = "A"; // "A" = Activo, "I" = Inactivo
    }
}