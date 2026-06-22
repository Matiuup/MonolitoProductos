using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace CapaDatos.Modelos
{
    public class ImagenUsuario
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string img_usuario_id { get; set; }
        public string img_tipo { get; set; } = "perfil";
        public string img_nombre_archivo { get; set; }
        public string img_formato { get; set; }
        public byte[] img_data { get; set; }
        public DateTime img_fecha_subida { get; set; } = DateTime.Now;
        public bool img_activa { get; set; } = true;
        public string img_estado { get; set; } = "A";
    }
}