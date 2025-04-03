using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Datos;
using Entidades;

namespace Negocio
{
    public class UsuarioNegocio
    {
        UsuarioDatos usuarioDatos = new UsuarioDatos();

        public bool ValidarLoginTemporal(UsuariosExistentes usuario,string strconexion)
        {
            string str_cod_error = string.Empty;
            string str_error_mensaje = string.Empty;
            DataTable usuarios = usuarioDatos.ValidarUsuariTemporal(usuario, 1, strconexion,ref str_cod_error,ref str_error_mensaje);
            bool temporal=Convert.ToBoolean(usuarios.Rows[0]["EsContrasenaTemporal"]);

             return temporal;
        }
        public DataTable ValidarLogin(UsuariosExistentes usuario, string strconexion)
        {
            string str_cod_error = string.Empty;
            string str_error_mensaje = string.Empty;
            DataTable usuarios = usuarioDatos.ValidarUsuariTemporal(usuario, 2, strconexion, ref str_cod_error, ref str_error_mensaje);
            return usuarios;
        }
        public int RegistroIngreso(RegistroIngreso usuario, string strconexion)
        {
            string str_cod_error = string.Empty;
            string str_error_mensaje = string.Empty;
            int usuarios = usuarioDatos.RegistroLogin(usuario, strconexion, ref str_cod_error, ref str_error_mensaje);
            return usuarios;
        }
        public int RegistroNuevo(UsuariosNuevos usuariosN, string strconexion)
        {
            string str_cod_error = string.Empty;
            string str_error_mensaje = string.Empty;
            int usuarios = usuarioDatos.RegistroClientesNuevos(usuariosN,  strconexion, ref str_cod_error, ref str_error_mensaje);
           
            return usuarios;
        }
        public bool CambioClave(CambiarContrasena cambioContrasena, string strconexion)
        {
            string str_cod_error = string.Empty;
            string str_error_mensaje = string.Empty;
            bool usuarios = usuarioDatos.CambioContrasenaTem(cambioContrasena, strconexion, ref str_cod_error, ref str_error_mensaje);

            return usuarios;
        }
        public bool RecuperarContrasena(RecuperarContrasena recuperar,string strconexion)
        {
            string str_cod_error = string.Empty;
            string str_error_mensaje = string.Empty;
            string nuevaContrasena = GenerarContrasena();
            try
            {
                using (SqlConnection con = new SqlConnection(strconexion))
                {
                    con.Open();
                    string query = "UPDATE usuarios SET usu_passwordNuevo = @Password,EsContrasenaTemporal = 1 WHERE usu_login = @Login";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@Password", nuevaContrasena);
                        cmd.Parameters.AddWithValue("@Login", recuperar.Login);

                        int filasAfectadas = cmd.ExecuteNonQuery();

                        if (filasAfectadas > 0)
                        {
                            // 2️⃣ Enviar la nueva contraseña al correo del usuario
                            return EnviarCorreo(recuperar.Email, nuevaContrasena);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
            return false;
            //int usuarios = usuarioDatos.RegistroClientesNuevos(recuperar, ref str_cod_error, ref str_error_mensaje);

            // return usuarios;
        }
        public int Emailbienescrito(string email)
        {

            string patron = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";

            if (Regex.IsMatch(email, patron))
            {
                return 1; // El correo es válido
            }

            return 0;


        }
        private string GenerarContrasena()
        {
            const string caracteres = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789@#$!";
            StringBuilder resultado = new StringBuilder();
            Random random = new Random();

            for (int i = 0; i < 10; i++)
            {
                resultado.Append(caracteres[random.Next(caracteres.Length)]);
            }

            return resultado.ToString();
        }

        private bool EnviarCorreo(string emailDestino, string nuevaContrasena)
        {
            try
            {
                MailMessage mail = new MailMessage();
                mail.From = new MailAddress("lrodriguezb10@ucentral.edu.co");
                mail.To.Add(emailDestino);
                mail.Subject = "Recuperación de Contraseña";
                mail.Body = $"Tu nueva contraseña es: {nuevaContrasena}\nPor favor, cambia tu contraseña después de iniciar sesión.";
                mail.IsBodyHtml = false;

                SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);
                smtp.Credentials = new NetworkCredential("Vannerodriguez556@gmail.com", "vdoo jsuz oytk lgpi");
                smtp.EnableSsl = true;
                smtp.Send(mail);

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al enviar el correo: " + ex.Message);
                return false;
            }
        }

        public List<TiposIdentificacion> TipoIdentificacioneslist(string strconexion)
        {
            string str_cod_error = string.Empty;
            string str_error_mensaje = string.Empty;
            List<TiposIdentificacion>  lista1 = usuarioDatos.ListaTiposIdentificacion(strconexion, ref str_cod_error, ref str_error_mensaje);
            

            return lista1;
        }
        public List<TiposClases> TipoClaseslist(string strconexion)
        {
            string str_cod_error = string.Empty;
            string str_error_mensaje = string.Empty;
            List<TiposClases> lista1 = usuarioDatos.ListaClasesVuelos(strconexion, ref str_cod_error, ref str_error_mensaje);


            return lista1;
        }
        public List<TipoViaje> TipoViajelist(string strconexion)
        {
            string str_cod_error = string.Empty;
            string str_error_mensaje = string.Empty;
            List<TipoViaje> lista1 = usuarioDatos.ListaTipoVuelos(strconexion, ref str_cod_error, ref str_error_mensaje);


            return lista1;
        }
        public List<Destino> Destino(string strconexion)
        {
            string str_cod_error = string.Empty;
            string str_error_mensaje = string.Empty;
            List<Destino> lista1 = usuarioDatos.ListaDestinoVuelos(strconexion, ref str_cod_error, ref str_error_mensaje);


            return lista1;
        }
        public List<OpcionUsuario> OpcionesAdmin(string strconexion)
        {
            string str_cod_error = string.Empty;
            string str_error_mensaje = string.Empty;
            List<OpcionUsuario> lista1 = usuarioDatos.OpcionesAdmin(strconexion, "1", ref str_cod_error, ref str_error_mensaje);


            return lista1;
        }
        public bool  EsAdmin(string strconexion, string login)
        {
            string str_cod_error = string.Empty;
            string str_error_mensaje = string.Empty;
            bool admin = usuarioDatos.EsAdmin(strconexion, login , ref str_cod_error, ref str_error_mensaje);


            return admin;
        }
        public int crearReserva(string strconexion, ReservasNuevas reservasNuevas,string login)
        {
            string str_cod_error = string.Empty;
            string str_error_mensaje = string.Empty;
            int reserva = usuarioDatos.RegistrarReserva(strconexion, reservasNuevas, login, ref str_cod_error, ref str_error_mensaje);


            return reserva;
        }
        


    }
}
