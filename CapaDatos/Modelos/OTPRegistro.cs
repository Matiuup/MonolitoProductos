using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace CapaDatos.Modelos
{
    public class OTPRegistro
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string otp_usu_id { get; set; }
        public string otp_codigo { get; set; }
        public DateTime otp_fecha_generacion { get; set; } = DateTime.Now;
        public DateTime otp_fecha_expiracion { get; set; }
        public string otp_tipo { get; set; }
        public bool otp_usado { get; set; } = false;
        public string otp_estado { get; set; } = "A";
    }
}