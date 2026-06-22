using CapaDatos.Modelos;
using CapaDatos.Repositorios;
using System;

namespace CapaNegocio.Seguridad
{
    public class CN_Imagen
    {
        private readonly ImagenUsuarioRepository _repo = new ImagenUsuarioRepository();

        public void GuardarImagen(string usuarioId, byte[] imagen, string formato, string nombre)
        {
            _repo.Guardar(new ImagenUsuario
            {
                img_usuario_id = usuarioId,
                img_data = imagen,
                img_formato = formato,
                img_nombre_archivo = nombre,
                img_activa = true
            });
        }

        public byte[] ObtenerImagenActiva(string usuarioId) => _repo.ObtenerActiva(usuarioId);
    }
}