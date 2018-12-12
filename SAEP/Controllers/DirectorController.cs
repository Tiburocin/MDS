using SAEP.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace SAEP.Controllers
{
    public class DirectorController : Controller
    {
        private SAEPEntities db = new SAEPEntities();

        // GET: Usuarios
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

        // GET: Usuarios/Create
        public ActionResult Create()
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
        public ActionResult Create([Bind(Include = "matricula,nombre,correo,contraseña,id_rol,telefono,id_pro_edu")] Usuario usuario)
        {
            if (ModelState.IsValid)
            {
                usuario.id_rol = 2;
                db.Usuario.Add(usuario);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.id_pro_edu = new SelectList(db.c_Programa_Edu, "id_pro_edu", "nombre", usuario.id_pro_edu);
            ViewBag.id_rol = new SelectList(db.c_Rol, "id_rol", "descripcion", usuario.id_rol);
            return View(usuario);
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
                db.Usuario.Add(usuario);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.id_pro_edu = new SelectList(db.c_Programa_Edu, "id_pro_edu", "nombre", usuario.id_pro_edu);
            ViewBag.id_rol = new SelectList(db.c_Rol, "id_rol", "descripcion", usuario.id_rol);
            return View(usuario);
        }

        // GET: Usuarios/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Usuario usuario = db.Usuario.Find(id);
            if (usuario == null)
            {
                return HttpNotFound();
            }
            return View(usuario);
        }

        // POST: Usuarios/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Usuario usuario = db.Usuario.Find(id);
            db.Usuario.Remove(usuario);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        //Evento
        [HttpGet]
        // GET: Usuario
        public ActionResult Lista_Eventos(string sortOrder)
        {
            //EventService eventos = new EventService();

            ViewBag.NameSortParm1 = String.IsNullOrEmpty(sortOrder) ? "estado" : "";
            ViewBag.NameSortParm2 = String.IsNullOrEmpty(sortOrder) ? "tipo" : "";
            ViewBag.NameSortParm3 = String.IsNullOrEmpty(sortOrder) ? "matricula" : "";
            var evento = from s in db.Evento select s;

            switch (sortOrder)
            {
                case "estado":
                    evento = evento.OrderByDescending(s => s.id_estado);
                    break;
                case "tipo":
                    evento = evento.OrderByDescending(s => s.id_tipo);
                    break;
                case "matricula":
                    evento = evento.OrderByDescending(s => s.matricula_co);
                    break;
            }

            return View(evento.ToList());
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}