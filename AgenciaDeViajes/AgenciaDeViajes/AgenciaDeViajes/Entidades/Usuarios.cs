using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entidades
{
    public class UsuariosExistentes
    {
        public string Usuario { get; set; }
        public string Password { get; set; }
    }
    public class UsuariosNuevos
    {
        public string TipoIdentificacion { get; set; }
        public string NoIdentifiacion { get; set; }
        public string Direccion { get; set; }
        public string NoCelular { get; set; }
        public string Nombre { get; set; }
        public string UsuarioNuevo { get; set; }
        public string PasswordNuevo { get; set; }
    }
}
