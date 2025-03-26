using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
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
        public string strconexion = ConfigurationManager.ConnectionStrings["Conexionbd"].ConnectionString;

        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]

        public ActionResult Login(UsuariosExistentes userExistentes, string bandera)
        {
            string strconexion = ConfigurationManager.ConnectionStrings["Conexionbd"].ConnectionString;

            if (bandera == "login")
            {
                // Validar usuario
                UsuariosExistentes jsonusuariosexistentes = new UsuariosExistentes
                {
                    Usuario = userExistentes.Usuario,
                    Password = userExistentes.Password
                };
                bool loginValidoTemporal = usuarioNegocio.ValidarLoginTemporal(jsonusuariosexistentes, strconexion);
                if (loginValidoTemporal)
                {
                    Session["UsuarioLogin"] = jsonusuariosexistentes.Usuario;
                    return RedirectToAction("CambiarContrasena", "Home");
                }
                else 
                {
                    DataTable loginValido = usuarioNegocio.ValidarLogin(jsonusuariosexistentes, strconexion);
                    if (loginValido.Rows.Count > 0)
                    {
                        int UsuarioId = Convert.ToInt32( loginValido.Rows[0]["usu_id"]);
                        // Si el login es exitoso, redirigir a otra vista
                        RegistroIngreso registroIngreso = new RegistroIngreso
                        {
                            UsuarioId = UsuarioId,
                            FechaIngreso = DateTime.Now,
                           // IP = HttpContext.Connection.RemoteIpAddress?.ToString(),
                            Navegador = Request.Headers["User-Agent"].ToString()

                        };
                        int registroingreso = usuarioNegocio.RegistroIngreso(registroIngreso, strconexion);
                        return RedirectToAction("Reservas", "Home"); // Cambia "Reservas" por la vista correcta
                    }
                    else
                    {
                        ViewBag.Mensaje = "Credenciales incorrectas, vuelva a intentar";
                        return View(); // Asegura que retorne la vista en caso de error
                    }
                }
                
            }
            else if (bandera == "recuperar")
            {
                return RedirectToAction("RecuperarContrasena", "Home");
            }
            else if (bandera == "registrar")
            {
                return RedirectToAction("Registrar", "Home");
            }

            // Agrega un return por defecto para evitar errores
            return View("Login"); // O la vista que desees mostrar en caso de una bandera inválida
        }

        [HttpGet]
        public ActionResult Registrar()
        {
            string strconexion = ConfigurationManager.ConnectionStrings["Conexionbd"].ConnectionString;
            List<TiposIdentificacion> tiposIdentificacion = usuarioNegocio.TipoIdentificacioneslist(strconexion);
            ViewBag.TiposIdentificacion = tiposIdentificacion;
            return View();
        }
        [HttpPost]

        public ActionResult Registrar(UsuariosNuevos usernuevos)
        {
            string strconexion = ConfigurationManager.ConnectionStrings["Conexionbd"].ConnectionString;
            List<TiposIdentificacion> tiposIdentificacion = usuarioNegocio.TipoIdentificacioneslist(strconexion);
            ViewBag.TiposIdentificacion = tiposIdentificacion;

            int email = usuarioNegocio.Emailbienescrito(usernuevos.Email);
            if (email != 1)
            {
                ViewBag.Mensaje = "El correo ingresado es incorrecto, vuelva a intentar";
                return View(usernuevos); // Mantiene la vista actual si el email es incorrecto
            }

            int registronuevo = usuarioNegocio.RegistroNuevo(usernuevos, strconexion);
            if (registronuevo == 1) // Registro exitoso
            {
                TempData["MensajeExito"] = "¡Registro exitoso! Ahora puedes iniciar sesión.";
                return RedirectToAction("Login"); // Redirige a la acción "Login"
            }

            ViewBag.Mensaje = "Error, Registro duplicado, intente de nuevo";
            return View(usernuevos);
        }

        [HttpGet]
        public ActionResult RecuperarContrasena()
        {
            return View();
        }
        [HttpPost]

        public ActionResult RecuperarContrasena (RecuperarContrasena recuperar)
        {
            string strconexion = ConfigurationManager.ConnectionStrings["Conexionbd"].ConnectionString;
            int email = usuarioNegocio.Emailbienescrito(recuperar.Email);
            if (email != 1)
            {
                ViewBag.Mensaje = "El correo ingresado es incorrecto, vuelva a intentar";
                return View(recuperar);
            }
            else 
            {
                bool recuperarcontrasena = usuarioNegocio.RecuperarContrasena(recuperar, strconexion);
                if (recuperarcontrasena)
                {
                    TempData["MensajeExito"] = "Se ha enviado una nueva contraseña a tu correo.";
                    return RedirectToAction("Login");
 
                }
                else
                {
                    ViewBag.Mensaje = "No se encontró el correo en la base de datos.";
                }

                return View(recuperar);
            }
        }
        [HttpGet]
        public ActionResult CambiarContrasena()
        {
            string usuario = Session["UsuarioLogin"].ToString(); // Obtener ID del usuario

            // Pasar el ID a la vista
            ViewBag.UsuarioLogin= usuario;

            return View();
        }
        [HttpPost]
        public ActionResult CambiarContrasena(CambiarContrasena cambiarContrasena)
        {
            string usuarioLogin = Session["UsuarioLogin"] as string;
            if (cambiarContrasena.PasswordNuevo == cambiarContrasena.PasswordConfirmar)
            {
                cambiarContrasena.Login = usuarioLogin;
                bool cambioClave = usuarioNegocio.CambioClave(cambiarContrasena, strconexion);
                TempData["MensajeExito"] = "Se ha cambiado la clave exitosamente.";
                return RedirectToAction("Login");
            }
            else 
            {
                ViewBag.Mensaje = "Las contraseñas no son iguales, valide y vuelva a intentar ";
            }
                return View();
        }

        [HttpGet]
        public ActionResult Reservas()
        {
            List<TiposClases> tiposClases = usuarioNegocio.TipoClaseslist(strconexion);
            ViewBag.Clases = tiposClases;
            List<TipoViaje> tiposViaje = usuarioNegocio.TipoViajelist(strconexion);
            ViewBag.TipoViaje = tiposViaje;
            List<Destino> destino = usuarioNegocio.Destino(strconexion);
            ViewBag.Destino = destino;

            return View();
        }
        [HttpPost]
        public ActionResult Reservas(ReservasNuevas reservasNuevas,int confirmacion)
        {
            List<TiposClases> tiposClases = usuarioNegocio.TipoClaseslist(strconexion);
            ViewBag.Clases = tiposClases;
            List<TipoViaje> tiposViaje = usuarioNegocio.TipoViajelist(strconexion);
            ViewBag.TipoViaje = tiposViaje;
            List<Destino> destino = usuarioNegocio.Destino(strconexion);
            ViewBag.Destino = destino;
            if (confirmacion == 1)
            {

            }
            else 
            {
                TempData["MensajeExito"] = "La reserva se ha cancelado exitosamente";
                return RedirectToAction("Login");
            }


                return View();
        }

    }
}