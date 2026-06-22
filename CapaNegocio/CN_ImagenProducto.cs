using CapaDatos.Modelos;
using CapaDatos.Repositorios;
using System;
using System.Collections.Generic;

namespace CapaNegocio
{
    public class CN_ImagenProducto
    {
        private readonly ImagenProductoRepository _repo = new ImagenProductoRepository();

        // Insertar una nueva imagen para un producto (retorna el Id string)
        public string Insertar(string productoId, string ruta, string nombreArchivo = null, bool esPrincipal = false)
        {
            var img = new ImagenProducto
            {
                ProductoId = productoId,
                img_ruta = ruta,
                img_nombre_archivo = nombreArchivo,
                img_principal = esPrincipal,
                img_estado = "A"
            };
            return _repo.Insertar(img);
        }

        // Eliminación lógica
        public void Eliminar(string imagenId)
        {
            var img = _repo.ObtenerPorId(imagenId); // vamos a necesitar este método, lo añadimos
            if (img == null) throw new Exception("Imagen no encontrada.");
            _repo.Eliminar(imagenId);
        }

        // Obtener todas las imágenes activas de un producto
        public List<ImagenProducto> ObtenerPorProducto(string productoId)
        {
            return _repo.ObtenerPorProducto(productoId);
        }

        // Obtener una imagen por Id (para validar existencia)
        public ImagenProducto ObtenerPorId(string imagenId)
        {
            var repo = new ImagenProductoRepository();
            // Necesitamos un método ObtenerPorId en el repositorio
            return repo.ObtenerPorId(imagenId);
        }
    }
}