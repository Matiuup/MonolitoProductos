using CapaDatos.Modelos;
using CapaDatos.Repositorios;
using System;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Mail;

namespace CapaNegocio.Seguridad
{
    public class CN_Seguridad
    {
        private readonly OTPRepository _otpRepo = new OTPRepository();
        private readonly UsuarioRepository _userRepo = new UsuarioRepository();

        public string GenerarOTP(string usuarioId)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var rand = new Random();
            string otp = new string(new char[8].Select(c => chars[rand.Next(chars.Length)]).ToArray());

            _otpRepo.Insertar(new OTPRegistro
            {
                otp_usu_id = usuarioId,
                otp_codigo = otp,
                otp_fecha_expiracion = DateTime.Now.AddMinutes(15),
                otp_tipo = "autenticacion"
            });
            return otp;
        }

        public bool ValidarOTP(string usuarioId, string codigo) => _otpRepo.ValidarOTP(usuarioId, codigo);

        public string GenerarCodigoRecuperacion(string usuarioId)
        {
            var rand = new Random();
            string codigo = rand.Next(100000, 999999).ToString();
            _otpRepo.Insertar(new OTPRegistro
            {
                otp_usu_id = usuarioId,
                otp_codigo = codigo,
                otp_fecha_expiracion = DateTime.Now.AddMinutes(15),
                otp_tipo = "recuperacion"
            });

            var user = _userRepo.ObtenerPorId(usuarioId);
            if (user != null)
            {
                user.usu_token_recuperacion = codigo;
                user.usu_token_expiracion = DateTime.Now.AddMinutes(15);
                _userRepo.Actualizar(user);
            }
            return codigo;
        }

        public DataSet GenerarTokenRecuperacion(string correo)
        {
            var user = _userRepo.ObtenerPorCorreo(correo);
            DataSet ds = new DataSet();
            DataTable dt = new DataTable();
            dt.Columns.Add("StatusCode", typeof(int));
            dt.Columns.Add("Mensaje");
            dt.Columns.Add("Token");
            dt.Columns.Add("UsuarioId");
            if (user == null)
            {
                dt.Rows.Add(0, "Correo no registrado", "", "");
            }
            else
            {
                string token = GenerarCodigoRecuperacion(user.Id);
                dt.Rows.Add(1, "Token generado exitosamente", token, user.Id);
            }
            ds.Tables.Add(dt);
            return ds;
        }

        public void EnviarCorreoOTP(string correoDestino, string otp)
        {
            using (SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587))
            {
                smtp.Credentials = new NetworkCredential("matiyjosue8@gmail.com", "hrnv ofzh fcou rtbo");
                smtp.EnableSsl = true;
                using (MailMessage mail = new MailMessage())
                {
                    mail.From = new MailAddress("matiyjosue8@gmail.com", "Sistema Seguridad");
                    mail.To.Add(correoDestino);
                    mail.Subject = "Código OTP de verificación";
                    mail.Body = $"Tu código OTP es: {otp}. Válido por 15 minutos.";
                    smtp.Send(mail);
                }
            }
        }

        public void EnviarCorreoRecuperacion(string correoDestino, string token)
        {
            using (SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587))
            {
                smtp.Credentials = new NetworkCredential("matiyjosue8@gmail.com", "hrnv ofzh fcou rtbo");
                smtp.EnableSsl = true;
                using (MailMessage mail = new MailMessage())
                {
                    mail.From = new MailAddress("matiyjosue8@gmail.com", "Sistema Seguridad");
                    mail.To.Add(correoDestino);
                    mail.Subject = "Token de recuperación de contraseña";
                    mail.Body = $"Tu código de recuperación es: {token}. Válido por 15 minutos.";
                    smtp.Send(mail);
                }
            }
        }
    }
}