using CapaDatos.Modelos;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CapaDatos.Repositorios
{
    public class ImagenProductoRepository
    {
        private readonly IMongoCollection<ImagenProducto> _imagenes;

        public ImagenProductoRepository()
        {
            var db = ConexionMongo.ObtenerBaseDatos();
            _imagenes = db.GetCollection<ImagenProducto>("imagenes_producto");
        }

        // Insertar una imagen y devolver el Id generado
        public string Insertar(ImagenProducto imagen)
        {
            _imagenes.InsertOne(imagen);
            return imagen.Id;
        }

        // Eliminación lógica (cambia estado a "I")
        public void Eliminar(string imagenId)
        {
            var filter = Builders<ImagenProducto>.Filter.Eq(i => i.Id, imagenId);
            var update = Builders<ImagenProducto>.Update.Set(i => i.img_estado, "I");
            _imagenes.UpdateOne(filter, update);
        }

        // Obtener todas las imágenes activas de un producto, ordenadas por principal descendente
        public List<ImagenProducto> ObtenerPorProducto(string productoId)
        {
            var filter = Builders<ImagenProducto>.Filter.Where(i =>
                i.ProductoId == productoId && i.img_estado == "A");
            var sort = Builders<ImagenProducto>.Sort.Descending(i => i.img_principal);
            return _imagenes.Find(filter).Sort(sort).ToList();
        }

        public ImagenProducto ObtenerPorId(string id)
        {
            if (!ObjectId.TryParse(id, out ObjectId objectId))
                return null;
            return _imagenes.Find(i => i.Id == id).FirstOrDefault();
        }
    }
}