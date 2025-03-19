using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entidades;

namespace Datos
{
    public class UsuarioDatos
    {
        public bool ValidarUsuario(UsuariosExistentes usuario,string strconexion)
        {
            using (SqlConnection con = new SqlConnection(strconexion))
            {
                con.Open();
                string query = "SELECT COUNT(*) FROM Usuarios WHERE Usuario = @usuario AND Password = @password";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@usuario", usuario.Usuario);
                cmd.Parameters.AddWithValue("@password", usuario.Password);

                int count = (int)cmd.ExecuteScalar();

                return count > 0;
            }
        }
    }
}
