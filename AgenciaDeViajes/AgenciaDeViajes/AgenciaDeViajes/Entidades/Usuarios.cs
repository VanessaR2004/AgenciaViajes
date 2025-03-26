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
        public int TipoIdentificacion { get; set; }
        public string Identifiacion { get; set; }
        public string Direccion { get; set; }
        public string Celular { get; set; }
        public string Nombre { get; set; }
        public string UsuarioNuevo { get; set; }

        public string PasswordNuevo { get; set; }
        public string Email { get; set; }
    }
    public class TiposIdentificacion
    {
        public int IdenId { get; set; }
        public string Nombre { get; set; }
    }
    public class RecuperarContrasena
    {

        public string Email { get; set; }
        public string Login { get; set; }
    }
    public class CambiarContrasena
    {
        public string PasswordConfirmar { get; set; }
        public string PasswordNuevo { get; set; }
        public string Login { get; set; }

    }
    public class RegistroIngreso
    {

        public int  UsuarioId { get; set; }
        public DateTime FechaIngreso { get; set; } = DateTime.Now;
        public string Navegador { get; set; }
    }

}
