using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Entidades;
using Negocio;

namespace AgenciaDeViajes.Controllers
{
    public class HomeController : Controller
    {

        UsuarioNegocio usuarioNegocio = new UsuarioNegocio();

        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]

        public ActionResult Login(string usuario, string password, string bandera)
        {
            string strconexion = ConfigurationManager.ConnectionStrings["Conexionbd"].ConnectionString;
            if (bandera == "login")
            {
                // Validar usuario

                UsuariosExistentes jsonusuariosexistentes = new UsuariosExistentes
                {
                    Usuario = usuario,
                    Password = password
                };
                bool loginValido = usuarioNegocio.ValidarLogin(jsonusuariosexistentes, strconexion);

                if (loginValido)
                {
                    return RedirectToAction("Dashboard", "Home");
                }
                else
                {
                    ViewBag.MensajeError = "Usuario o contraseña incorrectos";
                }
            }
            else if (bandera == "recuperar")
            {
                // Lógica para recuperar contraseña
                return RedirectToAction("RecuperarContrasena", "Home");
            }
            else if (bandera == "registrar")
            {
                // Lógica para recuperar contraseña
                return RedirectToAction("Registar", "Home");
            }

            return View();
        }
        [HttpGet]
        public ActionResult Registar()
        {
            return View();
        }
        public ActionResult RecuperarContrasena()
        {
            return View();
        }
    }
}