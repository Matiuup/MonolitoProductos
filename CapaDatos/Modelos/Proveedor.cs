using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CapaDatos.Modelos
{
    public class Proveedor
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public string prov_nombre { get; set; }

        // "A" = Activo, "I" = Inactivo
        public string prov_estado { get; set; } = "A";
    }
}