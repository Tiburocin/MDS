using SAEP.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;

namespace SAEP.Controllers
{
    public class AdminController : Controller
    {
        private SAEPEntities db = new SAEPEntities();


        // GET: Admin
        public ActionResult Index(string sortOrder)
        {
            ViewBag.NameSortParm1 = String.IsNullOrEmpty(sortOrder) ? "nombre" : "";
            var usuario = from s in db.Usuario select s;
            usuario = db.Usuario.Include(u => u.c_Programa_Edu).Include(u => u.c_Rol);

            switch (sortOrder)
            {
                case "nombre":
                    usuario = usuario.OrderBy(s => s.nombre);
                    break;
            }

            return View(usuario.ToList());
        }

        public ActionResult CreateDir()
        {
            ViewBag.id_pro_edu = new SelectList(db.c_Programa_Edu, "id_pro_edu", "nombre");
            ViewBag.id_rol = new SelectList(db.c_Rol, "id_rol", "descripcion");
            return View();
        }

        // POST: Usuarios/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateDir([Bind(Include = "matricula,nombre,correo,contraseña,id_rol,telefono,id_pro_edu")] Usuario usuario)
        {

            if (ModelState.IsValid)
            {
                usuario.id_rol = 1;
                usuario.estado = true;

                if (Existe(usuario))
                {

                }
                else
                {
                    db.Usuario.Add(usuario);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }

            }

            ViewBag.id_pro_edu = new SelectList(db.c_Programa_Edu, "id_pro_edu", "nombre", usuario.id_pro_edu);
            ViewBag.id_rol = new SelectList(db.c_Rol, "id_rol", "descripcion", usuario.id_rol);
            return View(usuario);
        }


        public ActionResult Edit(int? matricula)
        {
            if (matricula == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Usuario usuario = db.Usuario.Find(matricula);
            if (usuario.estado == false)
            {
                usuario.estado = true;
            }
            else if (usuario.estado == true)
            {
                usuario.estado = false;
            }
            else
            {
                usuario.estado = true;
            }

            db.Entry(usuario).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Index", "Admin");

        }
        public bool Existe(Usuario us)
        {
            Usuario user = db.Usuario.Find(us.matricula);
            
            if (user != null)
            {
                TempData["msg"] = "<script>alert('El Usuario ya existe');</script>";
                //ScriptManager.RegisterStartupScript(this, this.GetType(), "ModalView", "<script> $('#Modal1').modal('show');</script>", false);
                return true;
            }

            return false;
        }
    }
}