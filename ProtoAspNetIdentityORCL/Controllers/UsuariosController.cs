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
    public class UsuariosController : Controller
    {
        private pcUpmeCnx db = new pcUpmeCnx();
        // GET: /Usuarios/  @Html.ActionLink("Inicio", "Index", "Home")
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

            var ResultadoQuery = (from p in db.MUB_USUARIOS
                          join q in db.MUB_ORGANIZACIONES on p.ID_ORGANIZACION equals q.ID_ORGANIZACION into Details
                          from m in Details.DefaultIfEmpty()
                          select p).OrderBy(a=> a.NOMBRE);
            //return View(db.MUB_USUARIOS.Include(s => s.MUB_ORGANIZACIONES).ToList());
            return View(ResultadoQuery);
        }

        // GET: /Usuarios/Details/5
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MUB_USUARIOS mub_usuarios = db.MUB_USUARIOS.Find(id);
            if (mub_usuarios == null)
            {
                return HttpNotFound();
            }
            return View(mub_usuarios);
        }

        // GET: /Usuarios/Create
        public ActionResult Create()
        {
            return RedirectToAction("Index");
            //return View();
        }

        // POST: /Usuarios/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="ID_USUARIO,NOMBRE,CARGO,DIRECCION,TELEFONO,CELULAR,EXTENSION,FAX,EMAIL,ESTADO,PWDHASH,ID_ORGANIZACION")] MUB_USUARIOS mub_usuarios)
        {
            if (ModelState.IsValid)
            {
                db.MUB_USUARIOS.Add(mub_usuarios);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(mub_usuarios);
        }

        // GET: /Usuarios/Edit/5
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MUB_USUARIOS mub_usuarios = db.MUB_USUARIOS.Find(id);
            if (mub_usuarios == null)
            {
                return HttpNotFound();
            }

            ViewBag.ID_ORGANIZACION = new SelectList(db.MUB_ORGANIZACIONES.OrderBy(a=>a.RAZON_SOCIAL), "ID_ORGANIZACION", "RAZON_SOCIAL", mub_usuarios.ID_ORGANIZACION);
            return View(mub_usuarios);
        }

        // POST: /Usuarios/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="ID_USUARIO,NOMBRE,CARGO,DIRECCION,TELEFONO,CELULAR,EXTENSION,FAX,EMAIL,ESTADO,ID_ORGANIZACION")] MUB_USUARIOS mub_usuarios)
        {
            if (ModelState.IsValid)
            {
                if (mub_usuarios.ID_ORGANIZACION.ToString() != "")
                {
                    db.Entry(mub_usuarios).State = EntityState.Modified;
                    db.Entry(mub_usuarios).Property(x => x.PWDHASH).IsModified=false;   //EXCLUIR PASSWORD
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                else
                {
                    ViewBag.MsjValidaOrganizacion = "seleccione una organización";
                    ViewBag.ID_ORGANIZACION = new SelectList(db.MUB_ORGANIZACIONES.OrderBy(a => a.RAZON_SOCIAL), "ID_ORGANIZACION", "RAZON_SOCIAL", mub_usuarios.ID_ORGANIZACION);
                }
            }
            return View(mub_usuarios);
        }

        // GET: /Usuarios/Delete/5
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MUB_USUARIOS mub_usuarios = db.MUB_USUARIOS.Find(id);
            if (mub_usuarios == null)
            {
                return HttpNotFound();
            }
            return View(mub_usuarios);
        }

        // POST: /Usuarios/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            MUB_USUARIOS mub_usuarios = db.MUB_USUARIOS.Find(id);
            db.MUB_USUARIOS.Remove(mub_usuarios);
            db.SaveChanges();
            return RedirectToAction("Index");
        }


        // GET: pcProyecto/vss/5
        public ActionResult rol(long? id, string nombre)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            return RedirectToAction("Index", "UsuarioRol", new { id = id,nombre = nombre });
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
