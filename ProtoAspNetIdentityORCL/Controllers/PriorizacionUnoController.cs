using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using NSPecor.Models;
using System.Data.SqlClient;
using Oracle.ManagedDataAccess.Client;

namespace NSPecor.Controllers
{
    public class PriorizacionUnoController : Controller
    {
        private pcUpmeCnx db = new pcUpmeCnx();

        // GET: /PriorizacionUno/
        public ActionResult Index()
        {
            string res_proc1 = "false";
            var vIdPlan = 0;
            var myString = Request.Form["VID_PLAN"];

            if (!String.IsNullOrEmpty(myString) && myString.Length > 0)
            {
                vIdPlan = Convert.ToInt32(Request.Form["VID_PLAN"]);

                string sqldb = "select pecor_f1_cudis (" + vIdPlan.ToString() + ") from dual";
                var resultado = db.Database.SqlQuery<String>(sqldb).ToList();

                if (!String.IsNullOrEmpty(resultado[0]))
                {
                    res_proc1 = resultado[0].ToString();
                }

            }
           
            DateTime fecha_consulta = DateTime.Now;
            ViewBag.VID_PLAN = new SelectList(db.MUB_PECOR_PLAN.Where(f => f.FECHA_FINAL < fecha_consulta), "ID_PLAN", "DESCRIPCION", vIdPlan);

            var muh_pecor_cobertura = db.MUH_PECOR_COBERTURA.Include(m => m.MUB_PECOR_PLAN).Where(f => f.ID_PLAN == vIdPlan);
            return View(muh_pecor_cobertura.ToList());
        }

        // GET: /PriorizacionUno/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MUH_PECOR_COBERTURA muh_pecor_cobertura = db.MUH_PECOR_COBERTURA.Find(id);
            if (muh_pecor_cobertura == null)
            {
                return HttpNotFound();
            }
            return View(muh_pecor_cobertura);
        }

        // GET: /PriorizacionUno/Create
        public ActionResult Create()
        {
            ViewBag.ID_PLAN = new SelectList(db.MUB_PECOR_PLAN, "ID_PLAN", "DESCRIPCION");
            return View();
        }

        // POST: /PriorizacionUno/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="DPTO_CCDGO,MPIO_CCDGO,VIGENCIA_ANT,VSS_BENEFIADAS,ID_PLAN,ICEE,V_TOTAL,VSS_TOTAL,NBI,ICEE_VIVTOT,ICEE_VSS_TOT,CU_DIESEL,DPTO_NOMBRE,MPIO_NOMBRE")] MUH_PECOR_COBERTURA muh_pecor_cobertura)
        {
            if (ModelState.IsValid)
            {
                db.MUH_PECOR_COBERTURA.Add(muh_pecor_cobertura);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ID_PLAN = new SelectList(db.MUB_PECOR_PLAN, "ID_PLAN", "DESCRIPCION", muh_pecor_cobertura.ID_PLAN);
            return View(muh_pecor_cobertura);
        }

        // GET: /PriorizacionUno/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MUH_PECOR_COBERTURA muh_pecor_cobertura = db.MUH_PECOR_COBERTURA.Find(id);
            if (muh_pecor_cobertura == null)
            {
                return HttpNotFound();
            }
            ViewBag.ID_PLAN = new SelectList(db.MUB_PECOR_PLAN, "ID_PLAN", "DESCRIPCION", muh_pecor_cobertura.ID_PLAN);
            return View(muh_pecor_cobertura);
        }

        // POST: /PriorizacionUno/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="DPTO_CCDGO,MPIO_CCDGO,VIGENCIA_ANT,VSS_BENEFIADAS,ID_PLAN,ICEE,V_TOTAL,VSS_TOTAL,NBI,ICEE_VIVTOT,ICEE_VSS_TOT,CU_DIESEL,DPTO_NOMBRE,MPIO_NOMBRE")] MUH_PECOR_COBERTURA muh_pecor_cobertura)
        {
            if (ModelState.IsValid)
            {
                db.Entry(muh_pecor_cobertura).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ID_PLAN = new SelectList(db.MUB_PECOR_PLAN, "ID_PLAN", "DESCRIPCION", muh_pecor_cobertura.ID_PLAN);
            return View(muh_pecor_cobertura);
        }

        // GET: /PriorizacionUno/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MUH_PECOR_COBERTURA muh_pecor_cobertura = db.MUH_PECOR_COBERTURA.Find(id);
            if (muh_pecor_cobertura == null)
            {
                return HttpNotFound();
            }
            return View(muh_pecor_cobertura);
        }

        // POST: /PriorizacionUno/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            MUH_PECOR_COBERTURA muh_pecor_cobertura = db.MUH_PECOR_COBERTURA.Find(id);
            db.MUH_PECOR_COBERTURA.Remove(muh_pecor_cobertura);
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
