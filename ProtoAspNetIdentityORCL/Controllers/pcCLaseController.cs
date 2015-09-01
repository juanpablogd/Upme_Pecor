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
    public class pcCLaseController : Controller
    {
        private pcUpmeCnx db = new pcUpmeCnx();
        
        // GET: pcCLase
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

            return View(db.MUB_CLASE_CP.ToList());
        }

        // GET: pcCLase/Details/5
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MUB_CLASE_CP mU_CLASE_CP = db.MUB_CLASE_CP.Find(id);
            if (mU_CLASE_CP == null)
            {
                return HttpNotFound();
            }
            return View(mU_CLASE_CP);
        }

        // GET: pcCLase/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: pcCLase/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "NOM_CLASE_CP")] MUB_CLASE_CP mU_CLASE_CP)
        {
            if (ModelState.IsValid)
            {
                //JP
                mU_CLASE_CP.ID_USUARIO_ACTUALIZACION = Convert.ToInt32(GlobalVariables.idUsuario);
                db.MUB_CLASE_CP.Add(mU_CLASE_CP);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(mU_CLASE_CP);
        }

        // GET: pcCLase/Edit/5
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MUB_CLASE_CP mU_CLASE_CP = db.MUB_CLASE_CP.Find(id);
            if (mU_CLASE_CP == null)
            {
                return HttpNotFound();
            }
            return View(mU_CLASE_CP);
        }

        // POST: pcCLase/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID_CLASE_CP,NOM_CLASE_CP")] MUB_CLASE_CP mU_CLASE_CP)
        {
            if (ModelState.IsValid)
            {
                mU_CLASE_CP.ID_USUARIO_ACTUALIZACION = Convert.ToInt32(GlobalVariables.idUsuario);
                db.Entry(mU_CLASE_CP).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(mU_CLASE_CP);
        }

        // GET: pcCLase/Delete/5
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MUB_CLASE_CP mU_CLASE_CP = db.MUB_CLASE_CP.Find(id);
            if (mU_CLASE_CP == null)
            {
                return HttpNotFound();
            }
            return View(mU_CLASE_CP);
        }

        // POST: pcCLase/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            MUB_CLASE_CP mU_CLASE_CP = db.MUB_CLASE_CP.Find(id);
            db.MUB_CLASE_CP.Remove(mU_CLASE_CP);
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
