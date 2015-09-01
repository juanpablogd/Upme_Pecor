using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using NSPecor.Models;
using Microsoft.AspNet.Identity;

namespace NSPecor.Controllers
{
    public class pcPlanesController : Controller
    {
        private pcUpmeCnx db = new pcUpmeCnx();

        // GET: /pcPlanes/
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

            return View(db.MUB_PECOR_PLAN.ToList());
        }

        // GET: /pcPlanes/Details/5
        public ActionResult Details(decimal id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MUB_PECOR_PLAN mub_pecor_plan = db.MUB_PECOR_PLAN.Find(id);
            if (mub_pecor_plan == null)
            {
                return HttpNotFound();
            }
            return View(mub_pecor_plan);
        }

        // GET: /pcPlanes/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: /pcPlanes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="ID_PLAN,DESCRIPCION,FECHA_INICIO,FECHA_FINAL,ACTIVO")] MUB_PECOR_PLAN mub_pecor_plan)
        {
            if (ModelState.IsValid)
            {
                db.MUB_PECOR_PLAN.Add(mub_pecor_plan);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(mub_pecor_plan);
        }

        // GET: /pcPlanes/Edit/5
        public ActionResult Edit(decimal id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MUB_PECOR_PLAN mub_pecor_plan = db.MUB_PECOR_PLAN.Find(id);
            if (mub_pecor_plan == null)
            {
                return HttpNotFound();
            }
            return View(mub_pecor_plan);
        }

        // POST: /pcPlanes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="ID_PLAN,DESCRIPCION,FECHA_INICIO,FECHA_FINAL,ACTIVO")] MUB_PECOR_PLAN mub_pecor_plan)
        {
            if (ModelState.IsValid)
            {
                db.Entry(mub_pecor_plan).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(mub_pecor_plan);
        }

        // GET: /pcPlanes/Delete/5
        public ActionResult Delete(decimal id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MUB_PECOR_PLAN mub_pecor_plan = db.MUB_PECOR_PLAN.Find(id);
            if (mub_pecor_plan == null)
            {
                return HttpNotFound();
            }
            return View(mub_pecor_plan);
        }

        // POST: /pcPlanes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(decimal id)
        {
            MUB_PECOR_PLAN mub_pecor_plan = db.MUB_PECOR_PLAN.Find(id);
            db.MUB_PECOR_PLAN.Remove(mub_pecor_plan);
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
