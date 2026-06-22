using CapaDatos.Modelos;
using MongoDB.Driver;
using System;

namespace CapaDatos.Repositorios
{
    public class OTPRepository
    {
        private readonly IMongoCollection<OTPRegistro> _otp;

        public OTPRepository()
        {
            var db = ConexionMongo.ObtenerBaseDatos();
            _otp = db.GetCollection<OTPRegistro>("otp_registro");
        }

        public void Insertar(OTPRegistro otp) => _otp.InsertOne(otp);

        public bool ValidarOTP(string usuarioId, string codigo, string tipo = null)
        {
            var filter = Builders<OTPRegistro>.Filter.Where(o =>
                o.otp_usu_id == usuarioId &&
                o.otp_codigo == codigo &&
                o.otp_fecha_expiracion > DateTime.Now &&
                o.otp_usado == false
            );
            if (!string.IsNullOrEmpty(tipo))
                filter &= Builders<OTPRegistro>.Filter.Eq(o => o.otp_tipo, tipo);

            var otp = _otp.Find(filter).FirstOrDefault();
            if (otp != null)
            {
                var update = Builders<OTPRegistro>.Update.Set(o => o.otp_usado, true);
                _otp.UpdateOne(o => o.Id == otp.Id, update);
                return true;
            }
            return false;
        }
    }
}