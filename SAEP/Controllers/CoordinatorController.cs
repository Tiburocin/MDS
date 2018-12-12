using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;
using SAEP.Models;
using System.Web.UI;

namespace Test.Controllers
{
    public class CoordinatorController : Controller
    {
        private SAEPEntities db = new SAEPEntities();

        // GET: Coordinator
        public ActionResult Index()
        {
            try
            {
                db.Configuration.LazyLoadingEnabled = false;
                //System.Diagnostics.Debug.WriteLine("Hey:   " + Session["usuarioMatricula"]);
                var eventos = db.Evento.Where(x => x.matricula_co == Convert.ToInt32(Session["usuarioMatricula"]));

                eventos = db.Evento.Include(u => u.c_Estado_Evento).Include(u => u.c_Tipo_Evento).Include(u => u.Usuario);
                return View(MostrarEventos(Convert.ToInt32(Session["usuarioMatricula"])));

            }
            catch (Exception ex)
            {
                return View();
                throw;
            }
        }

        //crear el evento 
        public ActionResult Create()
        {
            //Llenamos una lista para poder ocuparla despues en la vista.
                ViewBag.id_tipo = new SelectList(db.c_Tipo_Evento, "id_tipo", "descripcion");

                return View();
        }

        // POST: Usuario/Create
        [HttpPost]
        public async Task<ActionResult> Create([Bind(Include = "codigo,matricula_co,id_estado,ponente,titulo,fecha,lugar,asesor,id_tipo,abstracto")] Evento evento)
        {
            // TODO: Add insert logic here
            if (ModelState.IsValid)
            {
                evento.matricula_co = Convert.ToInt32(Session["usuarioMatricula"]);
                evento.codigo = GenerarCodigo(Convert.ToInt32(evento.matricula_co));
                evento.id_estado = 1;
                db.Configuration.LazyLoadingEnabled = false;


                db.Evento.Add(evento);
                await db.SaveChangesAsync();


                ViewBag.id_tipo = new SelectList(db.c_Tipo_Evento, "id_tipo", "descripcion", evento.id_tipo);

                return RedirectToAction("Index");

            }
            return View(evento);
        }

        private string GenerarCodigo(int matricula)
        {
            string codigo = "";
            try
            {
                // TODO: Add insert logic here

                db.Configuration.LazyLoadingEnabled = false;

                Usuario usuario = new Usuario();
                c_Programa_Edu programa_Edu = new c_Programa_Edu();
                c_Direccion direccion = new c_Direccion();

                usuario = db.Usuario.Find(matricula);
                programa_Edu = db.c_Programa_Edu.Find(usuario.id_pro_edu);
                direccion = db.c_Direccion.Find(programa_Edu.id_direccion);


                var cuenta = db.Evento.Count(x => x.matricula_co == usuario.matricula);

                codigo = $"{direccion.siglas}/{programa_Edu.siglas}{cuenta + 1}/{DateTime.Now.Year}";

            }
            catch (Exception ex)
            {
                return ex.ToString();
            }

            return codigo;
        }
        
        // GET: Usuario/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            db.Configuration.LazyLoadingEnabled = false;
            Evento evento = await db.Evento.FindAsync(id);
            evento.id_estado = 2;
            db.Entry(evento).State = EntityState.Modified;
            await db.SaveChangesAsync();
            return RedirectToAction("Index","Coordinator");

        }

        
        public ActionResult Invitacion(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var evento = db.Evento.Find(id);
            System.Diagnostics.Debug.WriteLine("Hey:   " + id);
           
            FileStream fs = new FileStream(@"C:/Users/Juan/Documents/Invitación.pdf", FileMode.Create);
            Document document = new Document(iTextSharp.text.PageSize.LETTER, 0, 0, 0, 0);
            PdfWriter writer = PdfWriter.GetInstance(document, fs);
            
            document.Open();
            BaseFont bf = BaseFont.CreateFont(BaseFont.TIMES_ROMAN, BaseFont.CP1252,false);
                     
            document.Add(new Paragraph("Universidad Politécnica de Tulancingo"));
            
            iTextSharp.text.Image png1 = iTextSharp.text.Image.GetInstance("C:/Users/Juan/Documents/logo.png");
            png1.ScalePercent(40f);
            document.Add(png1);

            document.Add(new Paragraph($"{evento.codigo}"));
            document.Add(new Paragraph($"{evento.ponente}"));
            document.Add(new Paragraph($"{evento.titulo}"));
            document.Add(new Paragraph($"{evento.c_Tipo_Evento.descripcion}"));
            document.Add(new Paragraph($"{evento.lugar}"));
            document.Add(new Paragraph($"{evento.fecha}"));
            document.Add(new Paragraph($"{evento.asesor}"));
            document.Add(new Paragraph($"{evento.abstracto}"));
            
            iTextSharp.text.Image png2 = iTextSharp.text.Image.GetInstance("C:/Users/Juan/Documents/QR-UPT.png");
            png2.ScalePercent(50f);
            png2.Alignment = Element.ALIGN_RIGHT;
            document.Add(png2);
            document.Close();

            return RedirectToAction("Index", "Coordinator");
        }

        public List<Evento> MostrarEventos(int? matricula)
        {
            using (var db = new SAEPEntities())
            {
                db.Configuration.LazyLoadingEnabled = false;

                List<Evento> eventos = db.Evento.Where(x => x.matricula_co == matricula).ToList();

                foreach (var item in eventos)
                {
                    Usuario usuario = db.Usuario.Find(item.matricula_co);
                    c_Estado_Evento estado = db.c_Estado_Evento.Find(item.id_estado);
                    c_Tipo_Evento tipo = db.c_Tipo_Evento.Find(item.id_tipo);

                    item.fecha.ToShortTimeString();
                    item.Usuario.nombre = usuario.nombre;
                    item.c_Estado_Evento.descripcion = estado.descripcion;
                    item.c_Tipo_Evento.descripcion = tipo.descripcion;
                }
                return eventos;
            }
        }
    }
}