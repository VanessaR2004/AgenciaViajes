using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.DynamicData;
using System.Web.Mvc;
using Entidades;
using Negocio;
using Newtonsoft.Json;

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
                    Session["UsuarioLogin"] = jsonusuariosexistentes.Usuario;
                    DataTable loginValido = usuarioNegocio.ValidarLogin(jsonusuariosexistentes, strconexion);
                    if (loginValido.Rows.Count > 0)
                    {
                        int UsuarioId = Convert.ToInt32(loginValido.Rows[0]["usu_id"]);
                        // Si el login es exitoso, redirigir a otra vista
                        RegistroIngreso registroIngreso = new RegistroIngreso
                        {
                            UsuarioId = UsuarioId,
                            FechaIngreso = DateTime.Now,
                            // IP = HttpContext.Connection.RemoteIpAddress?.ToString(),
                            Navegador = Request.Headers["User-Agent"].ToString()

                        };
                        int registroingreso = usuarioNegocio.RegistroIngreso(registroIngreso, strconexion);
                        bool EsAdmin = usuarioNegocio.EsAdmin(strconexion, jsonusuariosexistentes.Usuario);

                        if (EsAdmin)
                        {
                            return RedirectToAction("ReservasAdministradr", "Home");
                        }
                        else
                        {
                            return RedirectToAction("Reservas", "Home");
                        }

                        // return RedirectToAction("Reservas", "Home"); // Cambia "Reservas" por la vista correcta
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

        public ActionResult RecuperarContrasena(RecuperarContrasena recuperar)
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
            ViewBag.UsuarioLogin = usuario;

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
        public ActionResult Reservas(ReservasNuevas reservasNuevas, string accion)//int confirmacion)
        {
            List<TiposClases> tiposClases = usuarioNegocio.TipoClaseslist(strconexion);
            ViewBag.Clases = tiposClases;
            List<TipoViaje> tiposViaje = usuarioNegocio.TipoViajelist(strconexion);
            ViewBag.TipoViaje = tiposViaje;
            List<Destino> destino = usuarioNegocio.Destino(strconexion);
            ViewBag.Destino = destino;
            string usuarioLogin = Session["UsuarioLogin"] as string;
            decimal valorClase = 0;


            switch (reservasNuevas.Clase)
            {
                case 1: // Económica
                    valorClase = 100000;
                    break;
                case 2: // Ejecutiva
                    valorClase = 170000;
                    break;
                case 3: // Primera clase
                    valorClase = 300000;
                    break;
            }

            decimal valorBase = reservasNuevas.TipoViaje == 1 ? 150000 : 250000;
            reservasNuevas.ValorTotalPagar = valorBase + valorClase;

            if (accion == "reservar")
            {
                reservasNuevas.ReservaConfirmada = 1; // Confirmar reserva
            }
            else if (accion == "cancelar")
            {

                reservasNuevas.ReservaConfirmada = 0; // Cancelar reserva
            }
            if (reservasNuevas.ReservaConfirmada == 1)
            {

                int guardarreseva = usuarioNegocio.crearReserva(strconexion, reservasNuevas, usuarioLogin);
                TempData["ResumenReserva"] = JsonConvert.SerializeObject(reservasNuevas);
                return RedirectToAction("ResumenCompra");


            }
            else
            {
                TempData["MensajeExito"] = "La reserva se ha cancelado exitosamente";
                return RedirectToAction("Login");
            }

        }
        [HttpGet]
        public ActionResult ResumenCompra()
        {
            string usuarioLogin = Session["UsuarioLogin"] as string;
            DataTable informacionReserva = usuarioNegocio.InformacionReserva(strconexion, usuarioLogin);
            var reservas = new List<ResumenCompra>();
            foreach (DataRow row in informacionReserva.Rows)
            {
                reservas.Add(new ResumenCompra
                {

                    FechaIda = row["Rev_FechaVueloIda"].ToString(),
                    FechaVuelta = row["Rev_FechaVueloVuelta"].ToString(),
                    TipoViaje = row["Nombre"].ToString(),
                    Hora = row["Rev_Horario"].ToString(),
                    Destino = row["Des_Nombre"].ToString(),
                    ValorTotalPagar = row["ValorTotalPagar"].ToString()
                });
            }
            return View(reservas);
        }
        public ActionResult ReservasAdministradr()
        {
            string usuarioLogin = Session["UsuarioLogin"] as string;
            DataTable informacionReservaAdmin = usuarioNegocio.InformacionReservaAdmin(strconexion);
            var reservas = new List<ResumenCompra>();
            foreach (DataRow row in informacionReservaAdmin.Rows)
            {
                reservas.Add(new ResumenCompra
                {
                    Id_Reserva = Convert.ToInt32( row["Rev_Id"]),
                    FechaIda = row["Rev_FechaVueloIda"].ToString(),
                    FechaVuelta = row["Rev_FechaVueloVuelta"].ToString(),
                    TipoViaje = row["Nombre"].ToString(),
                    Hora = row["Rev_Horario"].ToString(),
                    Destino = row["Des_Nombre"].ToString(),
                    NombreUsuario = row["usu_nombre"].ToString(),
                    ValorTotalPagar = row["ValorTotalPagar"].ToString()
                });
            }
            return View(reservas);
        }

    }
}