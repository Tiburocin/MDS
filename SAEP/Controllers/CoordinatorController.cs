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
                SAEP.Models.Usuario user = (SAEP.Models.Usuario)Session["usuario"];
                db.Configuration.LazyLoadingEnabled = false;
                //System.Diagnostics.Debug.WriteLine("Hey:   " + Session["usuarioMatricula"]);
                var eventos = db.Evento.Where(x => x.matricula_co == user.matricula);

                eventos = db.Evento.Include(u => u.c_Estado_Evento).Include(u => u.c_Tipo_Evento).Include(u => u.Usuario);
                return View(MostrarEventos(user.matricula));

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
                SAEP.Models.Usuario user = (SAEP.Models.Usuario)Session["usuario"];
                evento.matricula_co = user.matricula;
                evento.codigo = GenerarCodigo(Convert.ToInt32(evento.matricula_co),evento.id_tipo,evento.fecha);
                evento.id_estado = 1;
                db.Configuration.LazyLoadingEnabled = false;

                if (evento.fecha > System.DateTime.Now)
                {
                    db.Evento.Add(evento);
                    await db.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["msg"] = "<script>alert('La fecha debe de ser en un plazo mayor a la actual');</script>";
                }

               


                ViewBag.id_tipo = new SelectList(db.c_Tipo_Evento, "id_tipo", "descripcion", evento.id_tipo);

                return View(evento);

            }
            return View(evento);
        }

        private string GenerarCodigo(int matricula,int tipo, DateTime fecha)
        {
            string codigo = "";
            try
            {
                // TODO: Add insert logic here

                db.Configuration.LazyLoadingEnabled = false;

                Usuario usuario = new Usuario();
                c_Programa_Edu programa_Edu = new c_Programa_Edu();
                c_Direccion direccion = new c_Direccion();
                c_Tipo_Evento tipo_evento = new c_Tipo_Evento();

                usuario = db.Usuario.Find(matricula);
                programa_Edu = db.c_Programa_Edu.Find(usuario.id_pro_edu);
                direccion = db.c_Direccion.Find(programa_Edu.id_direccion);
                tipo_evento = db.c_Tipo_Evento.Find(tipo);



                var cuenta = db.Evento.Count(x => x.matricula_co == usuario.matricula && x.id_tipo == tipo_evento.id_tipo  && x.fecha.Year == fecha.Year);

                Console.Write(cuenta);

                if (cuenta < 10)
                {
                    codigo = $"{direccion.siglas}/{programa_Edu.siglas}0{cuenta + 1}/{DateTime.Now.Year}";
                }
                else
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
           
            //Ruta del archivo donde se almacenara.
            FileStream fs = new FileStream("D:/Juan/Documents/Invitación.pdf", FileMode.Create);
            Document document = new Document(iTextSharp.text.PageSize.LETTER, 0, 0, 0, 0);
            PdfWriter writer = PdfWriter.GetInstance(document, fs);

            document.AddTitle("Invitación");
            document.AddCreator("SAEP");

            document.Open();


            iTextSharp.text.Font verdana1 = FontFactory.GetFont("Verdana", 18, iTextSharp.text.Font.BOLDITALIC, new iTextSharp.text.BaseColor(4, 26, 144));
            iTextSharp.text.Font verdana2 = FontFactory.GetFont("Verdana", 25, iTextSharp.text.Font.BOLDITALIC, new iTextSharp.text.BaseColor(4, 26, 144));
            iTextSharp.text.Font Times1 = FontFactory.GetFont("Times", 14, iTextSharp.text.Font.ITALIC, new iTextSharp.text.BaseColor(125, 88, 15));
            iTextSharp.text.Font Times2 = FontFactory.GetFont("Times", 8, iTextSharp.text.Font.ITALIC, new iTextSharp.text.BaseColor(125, 88, 15));

            iTextSharp.text.Rectangle rect = new iTextSharp.text.Rectangle(200f, 900f);
            rect.BackgroundColor = new BaseColor(134, 8, 18);
            
            document.Add(rect);

            Paragraph p = new Paragraph();
            p.Alignment = Element.ALIGN_CENTER;

            p.IndentationLeft = 200f;
            p.IndentationRight = 200f;


            p.Add(new Paragraph($"{evento.Usuario.c_Programa_Edu.c_Direccion.nombre}", verdana1));
            p.Add(new Paragraph("INVITA ", verdana2));
            document.Add(p);

            

            document.Add(new Paragraph($"{evento.codigo}",Times1));
            document.Add(new Paragraph($"{evento.ponente}", Times1));
            document.Add(new Paragraph($"{evento.titulo}", Times1));
            document.Add(new Paragraph($"{evento.Usuario.c_Programa_Edu.nombre}", verdana1));
            document.Add(new Paragraph($"{evento.c_Tipo_Evento.descripcion}", Times1));
            document.Add(new Paragraph($"{evento.lugar}", Times1));
            document.Add(new Paragraph($"{evento.fecha.ToShortDateString()}", Times1));
            document.Add(new Paragraph($"{evento.asesor}", Times1));
            document.Add(new Paragraph($"{evento.abstracto}", Times1));
            
            Chunk codigo = new Chunk($"{evento.codigo}", Times2);
            
            iTextSharp.text.Image png1 = iTextSharp.text.Image.GetInstance("D:/Juan/Documents/logo.png");
            png1.ScalePercent(60f);
            png1.SetAbsolutePosition(40f, 630f);
            document.Add(png1);

            iTextSharp.text.Image png2 = iTextSharp.text.Image.GetInstance("D:/Juan/Documents/QR-UPT.png");
            png2.ScalePercent(45f);
            png2.SetAbsolutePosition(460f, 15f);
            document.Add(png2);

           
            document.Close();

            System.Diagnostics.Process.Start("D:/Juan/Documents/Invitación.pdf");

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