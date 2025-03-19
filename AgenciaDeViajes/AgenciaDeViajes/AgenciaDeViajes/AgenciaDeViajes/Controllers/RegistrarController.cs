using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AgenciaDeViajes.Controllers
{
    public class RegistrarController : Controller
    {
        [HttpGet]
        public ActionResult Registar()
        {
            return View();
        }
    }
}