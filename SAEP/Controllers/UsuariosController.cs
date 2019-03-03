using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SAEP.Models;

namespace SAEP.Controllers
{
    public class UsuariosController : Controller
    {
        private SAEPEntities db = new SAEPEntities();

        // GET: Usuarios
        public async Task<ActionResult> Index()
        {
            var usuario = db.Usuario.Include(u => u.c_Programa_Edu).Include(u => u.c_Rol);
            return View(await usuario.ToListAsync());
        }

        // GET: Usuarios/Details/5
        public async Task<ActionResult> Details(int? id)
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
            return View(usuario);
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
        public async Task<ActionResult> Create([Bind(Include = "matricula,nombre,correo,contraseña,id_rol,telefono,id_pro_edu,estado")] Usuario usuario)
        {
            if (ModelState.IsValid)
            {
                db.Usuario.Add(usuario);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.id_pro_edu = new SelectList(db.c_Programa_Edu, "id_pro_edu", "nombre", usuario.id_pro_edu);
            ViewBag.id_rol = new SelectList(db.c_Rol, "id_rol", "descripcion", usuario.id_rol);
            return View(usuario);
        }

        // GET: Usuarios/Edit/5
        public async Task<ActionResult> Edit(int? id)
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
        public async Task<ActionResult> Edit([Bind(Include = "matricula,nombre,correo,contraseña,id_rol,telefono,id_pro_edu,estado")] Usuario usuario)
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

        // GET: Usuarios/Delete/5
        public async Task<ActionResult> Delete(int? id)
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
            return View(usuario);
        }

        // POST: Usuarios/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Usuario usuario = await db.Usuario.FindAsync(id);
            db.Usuario.Remove(usuario);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
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
