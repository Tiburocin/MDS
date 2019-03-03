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
                else if (usuarioDetalle.estado == false)
                {
                    usuario.ErrorLogin = "La cuenta está deshabilitada.";
                    return View("Index", usuario);
                }
                else
                {
                    //Variable de sesion.
                    Session.RemoveAll();
                    Session.Clear();
                    Session.Add("usuario", usuarioDetalle);

                    if (usuarioDetalle.id_rol == 1)
                    {
                        return RedirectToAction("Index", "Director");
                    }
                    else if(usuarioDetalle.id_rol == 2)
                    {
                        return RedirectToAction("Index", "Coordinator");
                    }
                    else 
                    {
                        return RedirectToAction("Index", "Admin");
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