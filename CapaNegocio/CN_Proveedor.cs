using CapaDatos.Modelos;
using CapaDatos.Repositorios;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CapaNegocio
{
    public class CN_Proveedor
    {
        private readonly ProveedorRepository _repo = new ProveedorRepository();

        // Lista los proveedores activos
        public List<Proveedor> ListarActivas()
        {
            return _repo.ObtenerActivos();
        }

        // Lista todos los proveedores (activos e inactivos)
        public List<Proveedor> ListarTodas()
        {
            return _repo.ObtenerTodos();
        }

        // Obtiene un proveedor por su Id (string)
        public Proveedor ObtenerPorId(string id)
        {
            return _repo.ObtenerPorId(id);
        }

        // Inserta un nuevo proveedor. Retorna el Id generado (string)
        public string Insertar(string nombre)
        {
            if (string.IsNullOrWhiteSpace(nombre))
                throw new Exception("El nombre del proveedor es obligatorio.");

            // Validar duplicado activo
            if (_repo.ObtenerPorNombre(nombre) != null)
                throw new Exception("Ya existe un proveedor activo con ese nombre.");

            var prov = new Proveedor
            {
                prov_nombre = nombre,
                prov_estado = "A"
            };
            return _repo.Insertar(prov);
        }

        // Actualiza el nombre de un proveedor existente
        public void Actualizar(string id, string nombre)
        {
            if (string.IsNullOrWhiteSpace(nombre))
                throw new Exception("El nombre del proveedor es obligatorio.");

            var prov = _repo.ObtenerPorId(id);
            if (prov == null) throw new Exception("Proveedor no encontrado.");

            // Validar que no exista otro proveedor activo con el mismo nombre
            var duplicado = _repo.ObtenerPorNombre(nombre);
            if (duplicado != null && duplicado.Id != id)
                throw new Exception("Ya existe otro proveedor activo con ese nombre.");

            prov.prov_nombre = nombre;
            _repo.Actualizar(prov);
        }

        // Desactiva un proveedor y deja sus productos activos sin proveedor (null)
        public void Desactivar(string id)
        {
            var prov = _repo.ObtenerPorId(id);
            if (prov == null) throw new Exception("Proveedor no encontrado.");
            _repo.Desactivar(id);
        }

        // Activa un proveedor (cambia estado a 'A')
        public void Activar(string id)
        {
            var prov = _repo.ObtenerPorId(id);
            if (prov == null) throw new Exception("Proveedor no encontrado.");
            _repo.Activar(id);
        }

        // Busca un proveedor por nombre exacto. Si no existe, lo crea. Retorna el Id. Útil para carga masiva.
        public string ObtenerOCrear(string nombre)
        {
            if (string.IsNullOrWhiteSpace(nombre))
                return null; // Sin proveedor
            return _repo.ObtenerOCrear(nombre);
        }

        // Lista todos los proveedores con la cantidad de productos activos asociados
        public List<object> ListarConCantidadProductos()
        {
            var datos = _repo.ObtenerConCantidadProductos();
            // Convertir a objetos anónimos para la GridView (misma estructura que antes)
            return datos.Select(d => (object)new
            {
                prov_id = d.prov_id,
                prov_nombre = d.prov_nombre,
                prov_estado = d.prov_estado,
                CantidadProductos = d.CantidadProductos
            }).ToList();
        }

        // Top 5 proveedores con más productos activos (para estadísticas)
        public List<ProveedorConCantidad> ObtenerTopProveedoresConMasProductos()
        {
            var datos = _repo.ObtenerTopProveedoresConMasProductos();
            return datos.Select(d => new ProveedorConCantidad
            {
                prov_nombre = d.prov_nombre,
                Cantidad = d.CantidadProductos
            }).ToList();
        }

        // Clase auxiliar para resultados (mantiene compatibilidad)
        public class ProveedorConCantidad
        {
            public string prov_nombre { get; set; }
            public int Cantidad { get; set; }
        }
    }
}