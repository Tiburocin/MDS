using SAEP.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;

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

        // GET: Usuarios/Edit/5
        public async Task<ActionResult> Editar(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Usuario usuario = await db.Usuario.FindAsync(id);
            if (usuario == null)
            {
                return HttpNotFound();
            }
            ViewBag.id_pro_edu = new SelectList(db.c_Programa_Edu, "id_pro_edu", "nombre", usuario.id_pro_edu);
            ViewBag.id_rol = new SelectList(db.c_Rol, "id_rol", "descripcion", usuario.id_rol);
            return View(usuario);
        }

        // POST: Usuarios/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Editar([Bind(Include = "matricula,nombre,correo,contraseña,id_rol,telefono,id_pro_edu,estado")] Usuario usuario)
        {
            if (ModelState.IsValid)
            {
                db.Entry(usuario).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.id_pro_edu = new SelectList(db.c_Programa_Edu, "id_pro_edu", "nombre", usuario.id_pro_edu);
            ViewBag.id_rol = new SelectList(db.c_Rol, "id_rol", "descripcion", usuario.id_rol);
            return View(usuario);
        }


        //public ActionResult CreateDir()
        //{
        //    ViewBag.id_pro_edu = new SelectList(db.c_Programa_Edu, "id_pro_edu", "nombre");
        //    ViewBag.id_rol = new SelectList(db.c_Rol, "id_rol", "descripcion");
        //    return View();
        //}

        //// POST: Usuarios/Create
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //// more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult CreateDir([Bind(Include = "matricula,nombre,correo,contraseña,id_rol,telefono,id_pro_edu")] Usuario usuario)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        usuario.id_rol = 1;
        //        usuario.estado = true;
        //        db.Usuario.Add(usuario);
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }

        //    ViewBag.id_pro_edu = new SelectList(db.c_Programa_Edu, "id_pro_edu", "nombre", usuario.id_pro_edu);
        //    ViewBag.id_rol = new SelectList(db.c_Rol, "id_rol", "descripcion", usuario.id_rol);
        //    return View(usuario);
        //}


        public ActionResult Edit(int? matricula)
        {
            if (matricula == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Usuario usuario = db.Usuario.Find(matricula);

            if (usuario.estado == false )
            {
                usuario.estado = true;
            }
            else if (usuario.estado == true){
                usuario.estado = false;
            }
            else
            {
                usuario.estado = true;
            }

            db.Entry(usuario).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Index", "Director");

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

        public bool Existe(Usuario us)
        {
            Usuario user = db.Usuario.Find(us.matricula);
            Usuario coor = db.Usuario.Find(us.id_pro_edu);

            if (coor != null && coor.estado == true)
            {
                TempData["msg"] = "<script>alert('Ya existe un coordinador para este programa');</script>";
                return true;
            }
            else if (user != null && user.estado == true)
            {
                TempData["msg"] = "<script>alert('El Usuario ya existe');</script>";
                return true;
            }
           
            return false;
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