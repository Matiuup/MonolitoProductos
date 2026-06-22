using CapaDatos.Modelos;
using CapaDatos.Repositorios;
using System;
using System.Collections.Generic;

namespace CapaNegocio
{
    public class CN_Categoria
    {
        private readonly CategoriaRepository _repo = new CategoriaRepository();

        public List<Categoria> ListarActivas() => _repo.ObtenerActivas();
        public List<Categoria> ListarTodas() => _repo.ObtenerTodas();
        public Categoria ObtenerPorId(string id) => _repo.ObtenerPorId(id);

        public void Insertar(string nombre, string descripcion)
        {
            if (string.IsNullOrWhiteSpace(nombre))
                throw new Exception("El nombre es obligatorio.");
            if (_repo.ObtenerPorNombre(nombre) != null)
                throw new Exception("Ya existe una categoría activa con ese nombre.");

            _repo.Insertar(new Categoria
            {
                Nombre = nombre,
                Descripcion = descripcion,
                Estado = "A"
            });
        }

        public void Actualizar(string id, string nombre, string descripcion)
        {
            if (string.IsNullOrWhiteSpace(nombre))
                throw new Exception("El nombre es obligatorio.");
            _repo.Actualizar(id, nombre, descripcion);
        }

        public void Desactivar(string id)
        {
            // Aquí luego validaremos que no tenga productos activos (lo adaptaremos más adelante)
            _repo.Desactivar(id);
        }

        public void Activar(string id) => _repo.Activar(id);
        public Categoria ObtenerPorNombre(string nombre) => _repo.ObtenerPorNombre(nombre);
    }
}