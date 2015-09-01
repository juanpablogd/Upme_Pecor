using NSPecor.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace NSPecor.Controllers
{
    public class pcTipoProyController : Controller
    {
        private pcUpmeCnx db = new pcUpmeCnx();

        // GET: pcTipoProy
        public ActionResult Index()
        {
            var mU_TIPO_PROY_PECOR = db.MUB_TIPO_PROY_PECOR;
            return View(mU_TIPO_PROY_PECOR.ToList());
        }

        // GET: pcTipoProy/Details/5
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MUB_TIPO_PROY_PECOR mU_TIPO_PROY_PECOR = db.MUB_TIPO_PROY_PECOR.Find(id);
            if (mU_TIPO_PROY_PECOR == null)
            {
                return HttpNotFound();
            }
            return View(mU_TIPO_PROY_PECOR);
        }

        // GET: pcTipoProy/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: pcTipoProy/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "NOM_TIPO")] MUB_TIPO_PROY_PECOR mU_TIPO_PROY_PECOR)
        {
            if (ModelState.IsValid)
            {
                mU_TIPO_PROY_PECOR.ACTIVO = "1";
                db.MUB_TIPO_PROY_PECOR.Add(mU_TIPO_PROY_PECOR);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(mU_TIPO_PROY_PECOR);
        }

        // GET: pcTipoProy/Edit/5
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MUB_TIPO_PROY_PECOR mU_TIPO_PROY_PECOR = db.MUB_TIPO_PROY_PECOR.Find(id);
            if (mU_TIPO_PROY_PECOR == null)
            {
                return HttpNotFound();
            }
            return View(mU_TIPO_PROY_PECOR);
        }

        // POST: pcTipoProy/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID_TIPO_PROY_PECOR,NOM_TIPO")] MUB_TIPO_PROY_PECOR mU_TIPO_PROY_PECOR)
        {
            if (ModelState.IsValid)
            {
                db.Entry(mU_TIPO_PROY_PECOR).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(mU_TIPO_PROY_PECOR);
        }

        // GET: pcTipoProy/Delete/5
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MUB_TIPO_PROY_PECOR mU_TIPO_PROY_PECOR = db.MUB_TIPO_PROY_PECOR.Find(id);
            if (mU_TIPO_PROY_PECOR == null)
            {
                return HttpNotFound();
            }
            return View(mU_TIPO_PROY_PECOR);
        }

        // POST: pcTipoProy/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            MUB_TIPO_PROY_PECOR mU_TIPO_PROY_PECOR = db.MUB_TIPO_PROY_PECOR.Find(id);
            db.MUB_TIPO_PROY_PECOR.Remove(mU_TIPO_PROY_PECOR);
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
