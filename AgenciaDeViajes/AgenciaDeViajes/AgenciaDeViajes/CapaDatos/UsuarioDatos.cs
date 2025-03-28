using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entidades;

namespace Datos
{
    public class UsuarioDatos
    {
        public DataTable ValidarUsuariTemporal(UsuariosExistentes usuario, int opcion, string strconexion,
            ref string str_cod_error, ref string str_error_mensaje)
        {
            using (SqlConnection con = new SqlConnection(strconexion))
            {
                SqlCommand cmd = new SqlCommand();
                SqlDataAdapter da = new SqlDataAdapter();
                DataTable dt = new DataTable();
                try
                {
                    cmd = new SqlCommand("Sp_Consulta_Usuarios", con);
                    cmd.Parameters.Add("@buscar", SqlDbType.Int).Value = opcion;
                    cmd.Parameters.Add("@login", SqlDbType.VarChar,100).Value = usuario.Usuario;
                    cmd.Parameters.Add("@password", SqlDbType.VarChar,100).Value = usuario.Password;

                    cmd.CommandType = CommandType.StoredProcedure;
                    da.SelectCommand = cmd;
                    da.Fill(dt);
                    return dt;
                }
                catch (Exception ex)
                {
                    str_cod_error = "PrE12";
                    str_cod_error = "Error: " + ex.Message;
                }
                finally
                {
                    cmd.Dispose();
                }
                return dt;


                //return count > 0;
            }



        }
        public int RegistroLogin(RegistroIngreso registrousuario,  string strconexion,
            ref string str_cod_error, ref string str_error_mensaje)
        {
            int estado = 0;
            using (SqlConnection con = new SqlConnection(strconexion))
            {
                SqlCommand cmd = new SqlCommand();
                SqlDataAdapter da = new SqlDataAdapter();
                DataTable dt = new DataTable();
                try
                {
                    cmd = new SqlCommand("Sp_Registro_Ingreso", con);
                   
                   cmd.Parameters.Add("@usu_id", SqlDbType.VarChar, 100).Value = registrousuario.UsuarioId;
                   cmd.Parameters.Add("@FechaIngreso", SqlDbType.DateTime).Value = registrousuario.FechaIngreso;
                   cmd.Parameters.Add("@Navegador", SqlDbType.VarChar, 100).Value = registrousuario.Navegador;
                    //cmd.Parameters.Add("@password", SqlDbType.VarChar, 100).Value = usuario.Password;

                    cmd.CommandType = CommandType.StoredProcedure;
                    da.SelectCommand = cmd;
                    da.Fill(dt);
                    estado = 1;
                    return estado;
                }
                catch (Exception ex)
                {
                    str_cod_error = "PrE12";
                    str_cod_error = "Error: " + ex.Message;
                    estado = 2;
                }
                finally
                {
                    cmd.Dispose();
                }
                return estado;


                //return count > 0;
            }



        }
        public bool CambioContrasenaTem(CambiarContrasena cambioC, string strconexion,
            ref string str_cod_error, ref string str_error_mensaje)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(strconexion))
                {
                    con.Open();
                    string query = "UPDATE usuarios SET usu_passwordNuevo = @Password,EsContrasenaTemporal = 0 WHERE usu_login = @Login";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@Password", cambioC.PasswordConfirmar);
                        cmd.Parameters.AddWithValue("@Login", cambioC.Login);

                        int filasAfectadas = cmd.ExecuteNonQuery();

                        if (filasAfectadas > 0)
                        {
                            // 2️⃣ Enviar la nueva contraseña al correo del usuario
                            return true; //EnviarCorreo(recuperar.Email, nuevaContrasena);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
            return false;



        }
        public int RegistroClientesNuevos(UsuariosNuevos usuariosN, string strconexion,
           ref string str_cod_error, ref string str_error_mensaje)
        {
            using (SqlConnection con = new SqlConnection(strconexion))
            {
                SqlCommand cmd = new SqlCommand();
                SqlDataAdapter da = new SqlDataAdapter();
                DataTable dt = new DataTable();
                int cod_error = 0;
                try
                {
                    cmd = new SqlCommand("Sp_Insert_User", con);
                    cmd.Parameters.Add("@TipoIdentificacion", SqlDbType.Int).Value = usuariosN.TipoIdentificacion;
                    cmd.Parameters.Add("@identificacion", SqlDbType.VarChar, 100).Value = usuariosN.Identifiacion;
                    cmd.Parameters.Add("@nombreCompleto", SqlDbType.VarChar, 100).Value = usuariosN.Nombre;
                    cmd.Parameters.Add("@Direccion", SqlDbType.VarChar, 100).Value = usuariosN.Direccion;
                    cmd.Parameters.Add("@Celular", SqlDbType.VarChar, 100).Value = usuariosN.Celular;
                    cmd.Parameters.Add("@login", SqlDbType.VarChar, 100).Value = usuariosN.UsuarioNuevo;
                    cmd.Parameters.Add("@contraseña", SqlDbType.VarChar, 100).Value = usuariosN.PasswordNuevo;
                    cmd.Parameters.Add("@Correo", SqlDbType.VarChar, 100).Value = usuariosN.Email;



                    cmd.CommandType = CommandType.StoredProcedure;
                    da.SelectCommand = cmd;
                    da.Fill(dt);
                    if (dt.Rows.Count > 0 && dt.Columns.Contains("ErrorMensaje"))
                    {
                        str_error_mensaje = dt.Rows[0]["ErrorMensaje"].ToString();

                        // Verificar si el mensaje indica un campo duplicado
                        if (str_error_mensaje.Contains("duplicate"))
                        {
                            str_cod_error = "DUPLICATE_ENTRY";
                            cod_error = -1;
                            return cod_error; // Código específico para duplicados
                        }

                    }
                    else
                    {
                        cod_error = 1;

                    }
                    return cod_error;
                    //return 1;



                }
                catch (Exception ex)
                {
                    str_cod_error = "PrE12";
                    str_cod_error = "Error: " + ex.Message;
                    return 0;
                }
                finally
                {
                    cmd.Dispose();
                }



                //return count > 0;
            }



        }


        public List<TiposIdentificacion> ListaTiposIdentificacion(string strconexion,
         ref string str_cod_error, ref string str_error_mensaje)
        {
            List<TiposIdentificacion> lista = new List<TiposIdentificacion>();
            using (SqlConnection con = new SqlConnection(strconexion))
            {
                try
                {
                    con.Open();
                    string query = "SELECT iden_id, nombre FROM TiposIdentificacion";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            lista.Add(new TiposIdentificacion
                            {
                                IdenId = reader.GetInt32(0),
                                Nombre = reader.GetString(1)
                            });
                        }
                    }
                    return lista;
                }
                catch (Exception ex)
                {
                    // Manejo de errores
                    Console.WriteLine("Error: " + ex.Message);
                    return null;
                }
            }
        }
        public List<TiposClases> ListaClasesVuelos(string strconexion,
         ref string str_cod_error, ref string str_error_mensaje)
        {
            List<TiposClases> lista = new List<TiposClases>();
            using (SqlConnection con = new SqlConnection(strconexion))
            {
                try
                {
                    con.Open();
                    string query = "SELECT CLas_Id, Nombre FROM ClasesVuelo";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            lista.Add(new TiposClases
                            {
                                CLas_Id = reader.GetInt32(0),
                                Nombre = reader.GetString(1)
                            });
                        }
                    }
                    return lista;
                }
                catch (Exception ex)
                {
                    // Manejo de errores
                    Console.WriteLine("Error: " + ex.Message);
                    return null;
                }
            }
        }
        public List<TipoViaje> ListaTipoVuelos(string strconexion,
 ref string str_cod_error, ref string str_error_mensaje)
        {
            List<TipoViaje> lista = new List<TipoViaje>();
            using (SqlConnection con = new SqlConnection(strconexion))
            {
                try
                {
                    con.Open();
                    string query = "SELECT Via_id, Via_nombre FROM TipoVuelo";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            lista.Add(new TipoViaje
                            {
                                Via_id = reader.GetInt32(0),
                                Via_nombre = reader.GetString(1)
                            });
                        }
                    }
                    return lista;
                }
                catch (Exception ex)
                {
                    // Manejo de errores
                    Console.WriteLine("Error: " + ex.Message);
                    return null;
                }
            }
        }
        public List<Destino> ListaDestinoVuelos(string strconexion,
 ref string str_cod_error, ref string str_error_mensaje)
        {
            List<Destino> lista = new List<Destino>();
            using (SqlConnection con = new SqlConnection(strconexion))
            {
                try
                {
                    con.Open();
                    string query = "SELECT Des_id, Des_nombre FROM Destinos";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            lista.Add(new Destino
                            {
                                Des_id = reader.GetInt32(0),
                                Des_nombre = reader.GetString(1)
                            });
                        }
                    }
                    return lista;
                }
                catch (Exception ex)
                {
                    // Manejo de errores
                    Console.WriteLine("Error: " + ex.Message);
                    return null;
                }
            }
        }
    }
}

