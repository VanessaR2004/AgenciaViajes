using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Datos;
using Entidades;

namespace Negocio
{
    public class UsuarioNegocio
    {
        UsuarioDatos usuarioDatos = new UsuarioDatos();

        public bool ValidarLogin(UsuariosExistentes usuario,string strconexion)
        {
            return usuarioDatos.ValidarUsuario(usuario,strconexion);
        }
    }
}
