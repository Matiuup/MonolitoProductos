using CapaDatos.Modelos;
using MongoDB.Driver;
using System.Linq;

namespace CapaDatos.Repositorios
{
    public class ImagenUsuarioRepository
    {
        private readonly IMongoCollection<ImagenUsuario> _imagenes;

        public ImagenUsuarioRepository()
        {
            var db = ConexionMongo.ObtenerBaseDatos();
            _imagenes = db.GetCollection<ImagenUsuario>("imagenes_usuario");
        }

        public void Guardar(ImagenUsuario imagen)
        {
            var filter = Builders<ImagenUsuario>.Filter.Where(i => i.img_usuario_id == imagen.img_usuario_id && i.img_activa);
            var update = Builders<ImagenUsuario>.Update.Set(i => i.img_activa, false);
            _imagenes.UpdateMany(filter, update);
            _imagenes.InsertOne(imagen);
        }

        public byte[] ObtenerActiva(string usuarioId)
        {
            var img = _imagenes.Find(i => i.img_usuario_id == usuarioId && i.img_activa).FirstOrDefault();
            return img?.img_data;
        }
    }
}