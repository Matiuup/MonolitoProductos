using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CapaDatos.Modelos
{
    public class TipoUsuario
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string tusu_nombre { get; set; }
        public string tusu_descripcion { get; set; }
        public string tusu_estado { get; set; } = "A";
    }
}