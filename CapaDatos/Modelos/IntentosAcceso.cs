using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace CapaDatos.Modelos
{
    public class IntentosAcceso
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string int_usu_id { get; set; }
        public DateTime int_fecha { get; set; } = DateTime.Now;
        public string int_ip { get; set; }
        public bool int_exitoso { get; set; }
        public string int_estado { get; set; } = "A";
    }
}