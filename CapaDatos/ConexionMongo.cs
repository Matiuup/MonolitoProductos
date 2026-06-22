using MongoDB.Driver;

namespace CapaDatos
{
    public static class ConexionMongo
    {
        private static readonly IMongoDatabase _database;

        static ConexionMongo()
        {
            // Cambia la cadena de conexión según tu configuración
            var client = new MongoClient("mongodb://localhost:27017");
            _database = client.GetDatabase("Monolito4B"); // Nombre de tu BD
        }

        public static IMongoDatabase ObtenerBaseDatos()
        {
            return _database;
        }
    }
}