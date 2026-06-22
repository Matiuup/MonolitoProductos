using CapaDatos.Modelos;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Linq;

namespace CapaDatos.Repositorios
{
    public class CategoriaRepository
    {
        private readonly IMongoCollection<Categoria> _categorias;

        public CategoriaRepository()
        {
            // Obtenemos la base de datos desde el singleton de conexión
            var db = ConexionMongo.ObtenerBaseDatos();
            // Accedemos a la colección de categorías
            _categorias = db.GetCollection<Categoria>("categorias");
        }

        // Obtener todas las categorías activas
        public List<Categoria> ObtenerActivas()
        {
            return _categorias.Find(c => c.Estado == "A").ToList();
        }

        // Obtener todas (activas e inactivas)
        public List<Categoria> ObtenerTodas()
        {
            return _categorias.Find(_ => true).ToList();
        }

        // Obtener por ID
        public Categoria ObtenerPorId(string id)
        {
            return _categorias.Find(c => c.Id == id).FirstOrDefault();
        }

        // Insertar una nueva categoría
        public void Insertar(Categoria categoria)
        {
            _categorias.InsertOne(categoria);
        }

        // Actualizar categoría
        public void Actualizar(string id, string nombre, string descripcion)
        {
            var update = Builders<Categoria>.Update
                .Set(c => c.Nombre, nombre)
                .Set(c => c.Descripcion, descripcion);
            _categorias.UpdateOne(c => c.Id == id, update);
        }

        // Desactivar (eliminación lógica)
        public void Desactivar(string id)
        {
            var update = Builders<Categoria>.Update.Set(c => c.Estado, "I");
            _categorias.UpdateOne(c => c.Id == id, update);
        }

        // Activar
        public void Activar(string id)
        {
            var update = Builders<Categoria>.Update.Set(c => c.Estado, "A");
            _categorias.UpdateOne(c => c.Id == id, update);
        }

        // Buscar por nombre exacto (útil para validaciones)
        public Categoria ObtenerPorNombre(string nombre)
        {
            return _categorias.Find(c => c.Nombre == nombre && c.Estado == "A").FirstOrDefault();
        }
    }
}