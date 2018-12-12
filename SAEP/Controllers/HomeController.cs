using SAEP.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SAEP.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
        
        [HttpPost]
        public ActionResult Autorizar(SAEP.Models.Usuario usuario)
        {
            using (SAEPEntities db = new SAEPEntities())
            {
                var usuarioDetalle = db.Usuario.Where(x => x.correo == usuario.correo && x.contraseña == usuario.contraseña).FirstOrDefault();
                if (usuarioDetalle == null)
                {
                    usuario.ErrorLogin = "Correo o contraseña invalida.";
                    return View("Index", usuario);
                }
                else
                {
                    Session["usuarioNombre"] = usuarioDetalle.nombre;
                    Session["usuarioMatricula"] = usuarioDetalle.matricula;
                    Session["usuarioRol"] = usuarioDetalle.id_rol;

                    if (usuarioDetalle.id_rol == 1)
                    {
                        return RedirectToAction("Index", "Director");
                    }
                    else
                    {
                        return RedirectToAction("Index", "Coordinator");
                    }
                }
            }
        }

        public ActionResult Salir()
        {
            Session.Abandon();
            return RedirectToAction("Index", "Home");
        }
    }
}