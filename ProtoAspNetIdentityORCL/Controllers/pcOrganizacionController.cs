using NSPecor.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;

namespace NSPecor.Controllers
{
    public class pcOrganizacionController : Controller
    {
        private pcUpmeCnx db = new pcUpmeCnx();

        // GET: pcOrganizacion
        public ActionResult Index()
        {
            if (User.Identity.IsAuthenticated == false || GlobalVariables.Acceso("ADMIN") == false)
            {
                return RedirectToAction("../Home/Index/");
            }
            else
            {
                if (GlobalVariables.idUsuario == null || GlobalVariables.idUsuario == "" || GlobalVariables.idOrganizacion == null)
                {
                    var usr_actual = User.Identity.GetUserName();
                    foreach (var item in db.MUB_USUARIOS.Where(u => u.EMAIL == usr_actual.ToString()))
                    {
                        GlobalVariables.idUsuario = item.ID_USUARIO.ToString();
                        GlobalVariables.idOrganizacion = item.ID_ORGANIZACION.ToString();
                    }
                }
            }
            var mU_ORGANIZACIONES = db.MUB_ORGANIZACIONES.Include(m => m.MUB_TIPO_ORGANIZACION);
            return View(mU_ORGANIZACIONES);
        }

        // GET: pcOrganizacion/Details/5
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MUB_ORGANIZACIONES mU_ORGANIZACIONES = db.MUB_ORGANIZACIONES.Find(id);
            if (mU_ORGANIZACIONES == null)
            {
                return HttpNotFound();
            }
            return View(mU_ORGANIZACIONES);
        }

        // GET: pcOrganizacion/Create
        public ActionResult Create()
        {
            ViewBag.ID_TIPO_ORGANIZACION = new SelectList(db.MUB_TIPO_ORGANIZACION, "ID_TIPO_ORGANIZACION", "NOM_TIPO_ORG");
            return View();
        }

        // POST: pcOrganizacion/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "NIT,RAZON_SOCIAL,DIRECCION,TELEFONO,REPRESENTANTE,ID_TIPO_ORGANIZACION,SIGLA,MP_UPME,ID_AREA_DISTRIBUCION,CODIGO")] MUB_ORGANIZACIONES mU_ORGANIZACIONES)
        {
            if (ModelState.IsValid)
            {
                mU_ORGANIZACIONES.ACTIVO = 1;                
                db.MUB_ORGANIZACIONES.Add(mU_ORGANIZACIONES);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ID_TIPO_ORGANIZACION = new SelectList(db.MUB_TIPO_ORGANIZACION, "ID_TIPO_ORGANIZACION", "NOM_TIPO_ORG", mU_ORGANIZACIONES.ID_TIPO_ORGANIZACION);
            return View(mU_ORGANIZACIONES);
        }

        // GET: pcOrganizacion/Edit/5
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MUB_ORGANIZACIONES mU_ORGANIZACIONES = db.MUB_ORGANIZACIONES.Find(id);
            if (mU_ORGANIZACIONES == null)
            {
                return HttpNotFound();
            }
            ViewBag.ID_TIPO_ORGANIZACION = new SelectList(db.MUB_TIPO_ORGANIZACION, "ID_TIPO_ORGANIZACION", "NOM_TIPO_ORG", mU_ORGANIZACIONES.ID_TIPO_ORGANIZACION);
            return View(mU_ORGANIZACIONES);
        }

        // POST: pcOrganizacion/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID_ORGANIZACION,NIT,RAZON_SOCIAL,DIRECCION,TELEFONO,REPRESENTANTE,ID_TIPO_ORGANIZACION,ACTIVO,SIGLA,MP_UPME,ID_AREA_DISTRIBUCION,CODIGO")] MUB_ORGANIZACIONES mU_ORGANIZACIONES)
        {
            if (ModelState.IsValid)
            {
                db.Entry(mU_ORGANIZACIONES).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ID_TIPO_ORGANIZACION = new SelectList(db.MUB_TIPO_ORGANIZACION, "ID_TIPO_ORGANIZACION", "NOM_TIPO_ORG", mU_ORGANIZACIONES.ID_TIPO_ORGANIZACION);
            return View(mU_ORGANIZACIONES);
        }

        // GET: pcOrganizacion/Delete/5
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MUB_ORGANIZACIONES mU_ORGANIZACIONES = db.MUB_ORGANIZACIONES.Find(id);
            if (mU_ORGANIZACIONES == null)
            {
                return HttpNotFound();
            }
            return View(mU_ORGANIZACIONES);
        }

        // POST: pcOrganizacion/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            MUB_ORGANIZACIONES mU_ORGANIZACIONES = db.MUB_ORGANIZACIONES.Find(id);
            db.MUB_ORGANIZACIONES.Remove(mU_ORGANIZACIONES);
            db.SaveChanges();
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
