using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace CapaDatos.Modelos
{
    public class Usuario
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string usu_cedula { get; set; }
        public string usu_nombres { get; set; }
        public string usu_apellidos { get; set; }
        public string usu_direccion { get; set; }
        public string usu_celular { get; set; }
        public string usu_correo { get; set; }
        public DateTime? usu_fecha_creacion { get; set; } = DateTime.Now;
        public DateTime? usu_fecha_cumple { get; set; }
        public string usu_nick { get; set; }
        public string usu_contraseña { get; set; }
        public int usu_intentos { get; set; } = 0;
        public DateTime? usu_fecha_ultimo_intento { get; set; }
        public DateTime? usu_fecha_bloqueo { get; set; }
        public string usu_estado { get; set; } = "A";
        public string usu_token_recuperacion { get; set; }
        public DateTime? usu_token_expiracion { get; set; }
        public string usu_secret_2fa { get; set; }
        public bool usu_2fa_habilitado { get; set; } = false;
        public string TipoUsuarioId { get; set; }
    }
}