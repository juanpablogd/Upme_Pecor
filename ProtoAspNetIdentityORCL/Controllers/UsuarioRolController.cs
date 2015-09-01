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
using Microsoft.AspNet.Identity;

namespace NSPecor.Controllers
{
    public class UsuarioRolController : Controller
    {
        private pcUpmeCnx db = new pcUpmeCnx();
        public static long id_usr;
        public static string nombre_usr;
        public static long id_rol_actual;

        // GET: /UsuarioRol/
        public ActionResult Index(string nombre, long? id)
        {
            if (User.Identity.IsAuthenticated == false)
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

            if (id != null)
            {
                id_usr = (long)id;
                nombre_usr = nombre;
            }

            var mub_usuarios_roles = db.MUB_USUARIOS_ROLES.Include(m => m.MUB_ROL).Include(m => m.MUB_USUARIOS).Where(s => s.ID_USUARIO == id_usr).ToList();
            ViewBag.Usuario = "Roles para el Usuario: " + nombre_usr;
            return View(mub_usuarios_roles);
        }

        // GET: /UsuarioRol/Details/5
        public ActionResult Details(long? ID_USUARIO, long? ID_ROL)
        {
            if (ID_USUARIO == null && ID_ROL == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var mub_usuarios_roles = db.MUB_USUARIOS_ROLES.Where(u => u.ID_ROL == ID_ROL).Where(u => u.ID_USUARIO == ID_USUARIO);
            //MUB_USUARIOS_ROLES mub_usuarios_roles = db.MUB_USUARIOS_ROLES.Find(3);
            //News news = newsServices.Getnews(GroupID).FirstOrDefault();   //or expect an ineumerable
            //IEnumerable<MUB_USUARIOS_ROLES> mub_usuarios_roles = db.MUB_USUARIOS_ROLES.Where(u => u.ID_ROL == ID_ROL).Where(u => u.ID_USUARIO == ID_USUARIO);
           // MUB_USUARIOS_ROLES mub_usuarios_roles = new MUB_USUARIOS_ROLES(db.MUB_USUARIOS_ROLES.Where(u => u.ID_ROL == ID_ROL).Where(u => u.ID_USUARIO == ID_USUARIO));

            if (mub_usuarios_roles == null)
            {
                return HttpNotFound();
            }
            return View(mub_usuarios_roles.ToList());
        }

        // GET: /UsuarioRol/Create
        public ActionResult Create()
        {
            if (id_usr == null || id_usr == 0)
            {
                return RedirectToAction("../Usuarios/Index");
            } 
            ViewBag.ID_ROL = new SelectList((from s in db.MUB_ROL
                                             join m in db.MUB_MODULOS on s.ID_MODULO equals m.ID_MODULO
                                             where s.ID_MODULO == (long)5 || s.ID_MODULO == (long)6 || s.ID_MODULO == (long)7
                                             orderby s.MUB_MODULOS.DESCRIPCION, s.NOMBRE
                                             select new { ID_ROL = s.ID_ROL, NOMBRE = m.DESCRIPCION + " / " + s.NOMBRE })
                                        .ToList(),
                                        "ID_ROL",
                                        "NOMBRE",
                                        null);
            ViewBag.ID_USUARIO = new SelectList(db.MUB_USUARIOS, "ID_USUARIO", "NOMBRE");
            return View();
        }

        // POST: /UsuarioRol/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="ID_USUARIO,ID_ROL,FECHA_ACTUALIZACION")] MUB_USUARIOS_ROLES mub_usuarios_roles)
        {
            if (ModelState.IsValid)
            {
                //VALIDA SI EL CENTRO POBLADO YA EXISTE!
                var ExisteVssCentroPob = from s in db.MUB_USUARIOS_ROLES where s.ID_USUARIO == id_usr && s.ID_ROL == mub_usuarios_roles.ID_ROL select s;
                var Repetido = 0;
                foreach (MUB_USUARIOS_ROLES cust in ExisteVssCentroPob)
                {
                    Repetido = 1;
                }
                if (Repetido == 0) {
                    //ACTUALIZA EL ROL
                    db.Database.ExecuteSqlCommand("INSERT INTO MUB_USUARIOS_ROLES  (ID_USUARIO,ID_ROL) VALUES (:ID_USR,:ID_ROL_NEW)",
                        new[] { new OracleParameter("ID_USR", id_usr), new OracleParameter("ID_ROL_NEW", mub_usuarios_roles.ID_ROL) });

                    //db.SaveChanges();
                    return RedirectToAction("Index");
                }
                else
                {
                    ViewBag.MsjValidaRol = "El Rol ya existe para este Usuario";
                }
            }

            ViewBag.ID_ROL = new SelectList((from s in db.MUB_ROL
                                             join m in db.MUB_MODULOS on s.ID_MODULO equals m.ID_MODULO
                                             where s.ID_MODULO == (long)5 || s.ID_MODULO == (long)6 || s.ID_MODULO == (long)7
                                             orderby s.MUB_MODULOS.DESCRIPCION, s.NOMBRE
                                             select new { ID_ROL = s.ID_ROL, NOMBRE = m.DESCRIPCION + " / " + s.NOMBRE })
                                    .ToList(),
                                    "ID_ROL",
                                    "NOMBRE",
                                    null);

            ViewBag.ID_USUARIO = new SelectList(db.MUB_USUARIOS, "ID_USUARIO", "NOMBRE", mub_usuarios_roles.ID_USUARIO);
            return View(mub_usuarios_roles);
        }

        // GET: /UsuarioRol/Edit/5  long? ID_USUARIO, long? ID_ROL      long? id
        public ActionResult Edit(long? ID_USUARIO, long? ID_ROL)
        {
            if (ID_ROL == null && ID_USUARIO == null)
            {
                return RedirectToAction("../Usuarios/Index");
            }
            MUB_USUARIOS_ROLES mub_usuarios_roles = db.MUB_USUARIOS_ROLES.Find(ID_USUARIO, ID_ROL);

            id_rol_actual = (long)ID_ROL;
            id_usr = (long)ID_USUARIO;

            if (mub_usuarios_roles == null)
            {
                return HttpNotFound();
            }

            ViewBag.ID_ROL = new SelectList((from s in db.MUB_ROL
                                             join m in db.MUB_MODULOS on s.ID_MODULO equals m.ID_MODULO
                                             where s.ID_MODULO == (long)5 || s.ID_MODULO == (long)6 || s.ID_MODULO == (long)7
                                             orderby s.MUB_MODULOS.DESCRIPCION, s.NOMBRE
                                             select new { ID_ROL = s.ID_ROL, NOMBRE = m.DESCRIPCION + " / " + s.NOMBRE })
                                    .ToList(),
                                    "ID_ROL",
                                    "NOMBRE",
                                    ID_ROL);

            return View(mub_usuarios_roles);
        }

        // POST: /UsuarioRol/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="ID_USUARIO,ID_ROL,FECHA_ACTUALIZACION")] MUB_USUARIOS_ROLES mub_usuarios_roles)
        {
            if (ModelState.IsValid)
            {
                if (mub_usuarios_roles.ID_ROL != id_rol_actual)
                {
                    //VALIDA SI EL CENTRO POBLADO YA EXISTE!
                    var ExisteVssCentroPob = from s in db.MUB_USUARIOS_ROLES where s.ID_USUARIO == id_usr && s.ID_ROL == mub_usuarios_roles.ID_ROL select s;
                    var Repetido = 0;
                    foreach (MUB_USUARIOS_ROLES cust in ExisteVssCentroPob)
                    {
                        Repetido = 1;
                    }
                    if (Repetido == 0)
                    {
                        //ACTUALIZA EL ROL
                        db.Database.ExecuteSqlCommand("update MUB_USUARIOS_ROLES set ID_ROL = :ID_ROL_NEW where ID_USUARIO = :ID_USR and ID_ROL = :ID_ROL_OLD ",
                            new[] { new OracleParameter("ID_ROL_NEW", mub_usuarios_roles.ID_ROL), new OracleParameter("ID_USR", id_usr), new OracleParameter("ID_ROL_OLD", id_rol_actual) });
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        ViewBag.MsjValidaRol = "El Rol ya existe para este Usuario";
                    }
                }
                else {
                    return RedirectToAction("Index");
                }
                
            }
            ViewBag.ID_ROL = new SelectList((from s in db.MUB_ROL
                                             join m in db.MUB_MODULOS on s.ID_MODULO equals m.ID_MODULO
                                             where s.ID_MODULO == (long)5 || s.ID_MODULO == (long)6 || s.ID_MODULO == (long)7
                                             orderby s.MUB_MODULOS.DESCRIPCION, s.NOMBRE
                                             select new { ID_ROL = s.ID_ROL, NOMBRE = m.DESCRIPCION + " / " + s.NOMBRE })
                                                .ToList(),
                                                "ID_ROL",
                                                "NOMBRE",
                                                mub_usuarios_roles.ID_ROL);
            return View(mub_usuarios_roles);
        }

        // GET: /UsuarioRol/Delete/5
        public ActionResult Delete(long? ID_USUARIO, long? ID_ROL)
        {
            if (ID_ROL == null && ID_USUARIO == null)
            {
                return RedirectToAction("../Usuarios/Index");
            }
            
            id_rol_actual = (long)ID_ROL;
            id_usr = (long)ID_USUARIO;

            MUB_USUARIOS_ROLES mub_usuarios_roles = db.MUB_USUARIOS_ROLES.Find(ID_USUARIO, ID_ROL);
            if (mub_usuarios_roles == null)
            {
                return HttpNotFound();
            }
            return View(mub_usuarios_roles);
        }

        // POST: /UsuarioRol/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long? ID_USUARIO, long? ID_ROL)
        {
            //ACTUALIZA EL ROL
            db.Database.ExecuteSqlCommand("DELETE FROM MUB_USUARIOS_ROLES where ID_USUARIO = :ID_USR and ID_ROL = :ID_ROL_OLD ",
                new[] { new OracleParameter("ID_USR", id_usr), new OracleParameter("ID_ROL_OLD", id_rol_actual) });
/*            MUB_USUARIOS_ROLES mub_usuarios_roles = db.MUB_USUARIOS_ROLES.Find(id);
            db.MUB_USUARIOS_ROLES.Remove(mub_usuarios_roles);
            db.SaveChanges();   */
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
