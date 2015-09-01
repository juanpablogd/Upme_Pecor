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
    public class pcTipoOrgController : Controller
    {
        private pcUpmeCnx db = new pcUpmeCnx();

        // GET: pcTipoOrg
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
            var mU_TIPO_ORGANIZACION = db.MUB_TIPO_ORGANIZACION;
            return View(mU_TIPO_ORGANIZACION.ToList());
        }

        // GET: pcTipoOrg/Details/5
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MUB_TIPO_ORGANIZACION mU_TIPO_ORGANIZACION = db.MUB_TIPO_ORGANIZACION.Find(id);
            if (mU_TIPO_ORGANIZACION == null)
            {
                return HttpNotFound();
            }
            return View(mU_TIPO_ORGANIZACION);
        }

        // GET: pcTipoOrg/Create
        public ActionResult Create()
        {
            //ViewBag.ID_ESTADO = new SelectList(db.MUB_ESTADO, "ID_ESTADO", "DESCRIPCION");
            return View();
        }

        // POST: pcTipoOrg/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "NOM_TIPO_ORG")] MUB_TIPO_ORGANIZACION mU_TIPO_ORGANIZACION)
        {
            if (ModelState.IsValid)
            {
                mU_TIPO_ORGANIZACION.ACTIVO = "S";
                db.MUB_TIPO_ORGANIZACION.Add(mU_TIPO_ORGANIZACION);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            //ViewBag.ID_ESTADO = new SelectList(db.MUB_ESTADO, "ID_ESTADO", "DESCRIPCION", mU_TIPO_ORGANIZACION.ID_ESTADO);
            return View(mU_TIPO_ORGANIZACION);
        }

        // GET: pcTipoOrg/Edit/5
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MUB_TIPO_ORGANIZACION mU_TIPO_ORGANIZACION = db.MUB_TIPO_ORGANIZACION.Find(id);
            if (mU_TIPO_ORGANIZACION == null)
            {
                return HttpNotFound();
            }
            //ViewBag.ID_ESTADO = new SelectList(db.MUB_ESTADO, "ID_ESTADO", "DESCRIPCION", mU_TIPO_ORGANIZACION.ID_ESTADO);
            return View(mU_TIPO_ORGANIZACION);
        }

        // POST: pcTipoOrg/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID_TIPO_ORGANIZACION,NOM_TIPO_ORG,ACTIVO")] MUB_TIPO_ORGANIZACION mU_TIPO_ORGANIZACION)
        {
            if (ModelState.IsValid)
            {
                db.Entry(mU_TIPO_ORGANIZACION).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            //ViewBag.ID_ESTADO = new SelectList(db.MUB_ESTADO, "ID_ESTADO", "DESCRIPCION", mU_TIPO_ORGANIZACION.ID_ESTADO);
            return View(mU_TIPO_ORGANIZACION);
        }

        // GET: pcTipoOrg/Delete/5
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MUB_TIPO_ORGANIZACION mU_TIPO_ORGANIZACION = db.MUB_TIPO_ORGANIZACION.Find(id);
            if (mU_TIPO_ORGANIZACION == null)
            {
                return HttpNotFound();
            }
            return View(mU_TIPO_ORGANIZACION);
        }

        // POST: pcTipoOrg/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            MUB_TIPO_ORGANIZACION mU_TIPO_ORGANIZACION = db.MUB_TIPO_ORGANIZACION.Find(id);
            db.MUB_TIPO_ORGANIZACION.Remove(mU_TIPO_ORGANIZACION);
            try
            {
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.GetType().FullName);
                Console.WriteLine(ex.Message);
                return View("Error");
            }  
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
