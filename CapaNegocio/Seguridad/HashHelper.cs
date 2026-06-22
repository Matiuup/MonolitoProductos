using System.Security.Cryptography;
using System.Text;

namespace CapaNegocio.Seguridad
{
    public static class HashHelper
    {
        public static string HashPassword(string clave)
        {
            using (SHA256 sha = SHA256.Create())
            {
                // IMPORTANTE: la sal debe ser exactamente "cl@ve_2026" como estaba en SQL
                byte[] bytes = sha.ComputeHash(Encoding.UTF8.GetBytes("cl@ve_2026" + clave));
                StringBuilder sb = new StringBuilder();
                foreach (byte b in bytes) sb.Append(b.ToString("x2"));
                return sb.ToString();
            }
        }
    }
}