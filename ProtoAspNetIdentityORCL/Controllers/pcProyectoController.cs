using NSPecor.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;

namespace NSPecor.Controllers
{
    public class pcProyectoController : Controller
    {
        private pcUpmeCnx db = new pcUpmeCnx();
        private long IdOrganizacion;
        private decimal IdPlan;
        

        // GET: pcProyecto
        public ActionResult Index()
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

            long Habilitado = 0;
            decimal IdPlan = 0;

            if (GlobalVariables.idOrganizacion != null) IdOrganizacion = Convert.ToInt64(GlobalVariables.idOrganizacion);
            DateTime fecha_consulta = DateTime.Now;
            //VALIDA SI HAY PLANES ACTIVOS EN LA FECHA ACTUAL
            var VssCentroPob = db.MUB_PECOR_PLAN.Where(f => f.FECHA_FINAL >= fecha_consulta && f.FECHA_INICIO <= fecha_consulta && f.ACTIVO == "S");
            
            foreach (MUB_PECOR_PLAN cust in VssCentroPob)
            {
                Habilitado = 1;
            }
            //VERIFICA EL PLAN ACTUAL
            var VssCentroPob2 = db.MUB_PECOR_PLAN.Where(f => f.ACTIVO == "S");
            foreach (MUB_PECOR_PLAN cust in VssCentroPob2)
            {
                IdPlan = cust.ID_PLAN;
            }
            if (Habilitado == 1)
            {
                ViewBag.Habilitado = "1";
            }
            else
            {
                ViewBag.Habilitado = "0";
            }

            var mU_PROYECTOS_PECOR = db.MUB_PROYECTOS_PECOR.Include(m => m.MUB_CLASE_CP).Include(m => m.MUB_ORGANIZACIONES).Include(m => m.MUB_TIPO_PROY_PECOR);
            return View(mU_PROYECTOS_PECOR.Where(p => p.ID_PLAN == IdPlan));

        }

        // GET: pcProyecto/Details/5
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MUB_PROYECTOS_PECOR mU_PROYECTOS_PECOR = db.MUB_PROYECTOS_PECOR.Find(id);
            if (mU_PROYECTOS_PECOR == null)
            {
                return HttpNotFound();
            }
            return View(mU_PROYECTOS_PECOR);
        }

        // GET: pcProyecto/Create
        public ActionResult Create()
        {
            /*if (IdOrganizacion == null){ViewBag.ID_ORGANIZACION = new SelectList(db.MUB_ORGANIZACIONES, "ID_ORGANIZACION", "RAZON_SOCIAL");}else{ViewBag.ID_ORGANIZACION = new SelectList(db.MUB_ORGANIZACIONES.Where(o => o.ID_ORGANIZACION == IdOrganizacion), "ID_ORGANIZACION", "RAZON_SOCIAL", GlobalVariables.idOrganizacion);} */
            ViewBag.ID_CLASE = new SelectList(db.MUB_CLASE_CP, "ID_CLASE_CP", "NOM_CLASE_CP");
            ViewBag.ID_ORGANIZACION = new SelectList(db.MUB_ORGANIZACIONES.Where(o => o.ID_TIPO_ORGANIZACION == 2).OrderBy(o => o.RAZON_SOCIAL), "ID_ORGANIZACION", "RAZON_SOCIAL");
            ViewBag.ID_TIPO_PROY_PECOR = new SelectList(db.MUB_TIPO_PROY_PECOR, "ID_TIPO_PROY_PECOR", "NOM_TIPO");
            ViewBag.ID_SUBESTACION = new SelectList(db.VISTA_SUBESTACION.Where(o => o.ID_ORGANIZACION == 0).OrderBy(o => o.NOM_SUBESTACION), "ID_SUBESTACION", "NOM_SUBESTACION");
            ViewBag.ID_PLAN = new SelectList(db.MUB_PECOR_PLAN.Where(f => f.ACTIVO == "S"), "ID_PLAN", "DESCRIPCION");
            return View();
        }

        // POST: pcProyecto/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID_ORGANIZACION,PROG_PROY,NOMBRE,ID_TIPO_PROY_PECOR,ID_CLASE,REALIZARA_PROYECTO,ADMIN_PROYECTO,FINANCIA_ACOMETIDA,INI_EJEC_INV,FIN_EJE_INV,PUESTA_MARCHA,DEMANDA_ANUAL,V_BENEFICIA,CIRCUITO,NIVEL_TENSION,RED_MT_KM,RED_BT_KM,NUM_TRANSFORMADORES,AOM_N1,COSTO_MEDIO_N1,INVERSION_N1,AOM_N2,COSTO_MEDIO_N2,INVERSION_N2,AOM_N3,COSTO_MEDIO_N3,INVERSION_N3,CU_MODIFICADO,ID_SUBESTACION,ID_PLAN")] MUB_PROYECTOS_PECOR mU_PROYECTOS_PECOR)
        {
            var submit = Request.Form["chgSitio"];   //String.IsNullOrEmpty(myString)
            if (ModelState.IsValid && String.IsNullOrEmpty(submit))
            {
            try{
                    if (mU_PROYECTOS_PECOR.NOMBRE != ""){
                        if (mU_PROYECTOS_PECOR.PUESTA_MARCHA < mU_PROYECTOS_PECOR.FIN_EJE_INV){
                            db.MUB_PROYECTOS_PECOR.Add(mU_PROYECTOS_PECOR);
                            System.Diagnostics.Debug.WriteLine("JP guardar");
                            db.SaveChanges();
                            return RedirectToAction("Index");
                        }else{
                            ViewBag.MsjValidaFechaMarcha = "La fecha de Puesta en Marcha debe ser MENOR a la Fecha final de Ejecución";
                        }
                    }
                    else
                    {
                        ViewBag.MsjValidaNombre = "Debe ingresar el Nombre del proyecto";
                    }
               }
                catch (DbEntityValidationException dbEx)
                {
                    foreach (var validationErrors in dbEx.EntityValidationErrors)
                    {
                        foreach (var validationError in validationErrors.ValidationErrors)
                        {
                            System.Diagnostics.Debug.WriteLine("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);
                        }
                    }
                } 
            }
/*            if (GlobalVariables.idOrganizacion == null){ViewBag.ID_ORGANIZACION = new SelectList(db.MUB_ORGANIZACIONES, "ID_ORGANIZACION", "RAZON_SOCIAL", mU_PROYECTOS_PECOR.ID_ORGANIZACION);}else{ViewBag.ID_ORGANIZACION = new SelectList(db.MUB_ORGANIZACIONES.Where(o => o.ID_ORGANIZACION == IdOrganizacion), "ID_ORGANIZACION", "RAZON_SOCIAL", GlobalVariables.idOrganizacion);}*/
            ViewBag.ID_CLASE = new SelectList(db.MUB_CLASE_CP, "ID_CLASE_CP", "NOM_CLASE_CP", mU_PROYECTOS_PECOR.ID_CLASE);
            ViewBag.ID_ORGANIZACION = new SelectList(db.MUB_ORGANIZACIONES.Where(o => o.ID_TIPO_ORGANIZACION == 2).OrderBy(o=>o.RAZON_SOCIAL), "ID_ORGANIZACION", "RAZON_SOCIAL",mU_PROYECTOS_PECOR.ID_ORGANIZACION);
            ViewBag.ID_TIPO_PROY_PECOR = new SelectList(db.MUB_TIPO_PROY_PECOR, "ID_TIPO_PROY_PECOR", "NOM_TIPO", mU_PROYECTOS_PECOR.ID_TIPO_PROY_PECOR);
            ViewBag.ID_SUBESTACION = new SelectList(db.VISTA_SUBESTACION.Where(o => o.ID_ORGANIZACION == mU_PROYECTOS_PECOR.ID_ORGANIZACION).OrderBy(o => o.NOM_SUBESTACION), "ID_SUBESTACION", "NOM_SUBESTACION", mU_PROYECTOS_PECOR.ID_SUBESTACION);
            ViewBag.ID_PLAN = new SelectList(db.MUB_PECOR_PLAN.Where(f => f.ACTIVO == "S"), "ID_PLAN", "DESCRIPCION", mU_PROYECTOS_PECOR.ID_PLAN);
            return View(mU_PROYECTOS_PECOR);
        }

        // GET: pcProyecto/Edit/5
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MUB_PROYECTOS_PECOR mU_PROYECTOS_PECOR = db.MUB_PROYECTOS_PECOR.Find(id);
            if (mU_PROYECTOS_PECOR == null)
            {
                return HttpNotFound();
            }
/*            if (GlobalVariables.idOrganizacion == null){ViewBag.ID_ORGANIZACION = new SelectList(db.MUB_ORGANIZACIONES, "ID_ORGANIZACION", "RAZON_SOCIAL", mU_PROYECTOS_PECOR.ID_ORGANIZACION);}else{ViewBag.ID_ORGANIZACION = new SelectList(db.MUB_ORGANIZACIONES.Where(o => o.ID_ORGANIZACION == IdOrganizacion), "ID_ORGANIZACION", "RAZON_SOCIAL", GlobalVariables.idOrganizacion);} */
            ViewBag.ID_CLASE = new SelectList(db.MUB_CLASE_CP, "ID_CLASE_CP", "NOM_CLASE_CP", mU_PROYECTOS_PECOR.ID_CLASE);
            ViewBag.ID_ORGANIZACION = new SelectList(db.MUB_ORGANIZACIONES.Where(o => o.ID_TIPO_ORGANIZACION == 2).OrderBy(o => o.RAZON_SOCIAL), "ID_ORGANIZACION", "RAZON_SOCIAL", mU_PROYECTOS_PECOR.ID_ORGANIZACION);
            ViewBag.ID_TIPO_PROY_PECOR = new SelectList(db.MUB_TIPO_PROY_PECOR, "ID_TIPO_PROY_PECOR", "NOM_TIPO", mU_PROYECTOS_PECOR.ID_TIPO_PROY_PECOR);
            ViewBag.ID_SUBESTACION = new SelectList(db.VISTA_SUBESTACION.Where(o => o.ID_ORGANIZACION == mU_PROYECTOS_PECOR.ID_ORGANIZACION).OrderBy(o => o.NOM_SUBESTACION), "ID_SUBESTACION", "NOM_SUBESTACION", mU_PROYECTOS_PECOR.ID_SUBESTACION);
            //           ViewBag.ID_SUBESTACION = new SelectList(db.VISTA_SUBESTACION, "ID_SUBESTACION", "NOM_SUBESTACION", mU_PROYECTOS_PECOR.ID_SUBESTACION);
            ViewBag.ID_PLAN = new SelectList(db.MUB_PECOR_PLAN.Where(f => f.ACTIVO == "S"), "ID_PLAN", "DESCRIPCION", mU_PROYECTOS_PECOR.ID_PLAN);
            return View(mU_PROYECTOS_PECOR);
        }

        // POST: pcProyecto/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID_PROYECTO_PECOR,CODIGO_UPME,ID_ORGANIZACION,PROG_PROY,NOMBRE,ID_TIPO_PROY_PECOR,ID_CLASE,REALIZARA_PROYECTO,ADMIN_PROYECTO,FINANCIA_ACOMETIDA,INI_EJEC_INV,FIN_EJE_INV,PUESTA_MARCHA,DEMANDA_ANUAL,V_BENEFICIA,CIRCUITO,NIVEL_TENSION,RED_MT_KM,RED_BT_KM,NUM_TRANSFORMADORES,AOM_N1,COSTO_MEDIO_N1,INVERSION_N1,AOM_N2,COSTO_MEDIO_N2,INVERSION_N2,AOM_N3,COSTO_MEDIO_N3,INVERSION_N3,CU_MODIFICADO,ID_USUARIO_REGISITRO,FECHA_REGISTRO,ID_SUBESTACION,ID_PLAN")] MUB_PROYECTOS_PECOR mU_PROYECTOS_PECOR)
        {                                                         //ID_ORGANIZACION,PROG_PROY,NOMBRE,ID_TIPO_PROY_PECOR,ID_CLASE,REALIZARA_PROYECTO,FINANCIA_ACOMETIDA,INI_EJEC_INV,FIN_EJE_INV,PUESTA_MARCHA,DEMANDA_ANUAL,V_BENEFICIA,CIRCUITO,NIVEL_TENSION,RED_MT_KM,RED_BT_KM,NUM_TRANSFORMADORES,AOM_N1,COSTO_MEDIO_N1,INVERSION_N1,AOM_N2,COSTO_MEDIO_N2,INVERSION_N2,AOM_N3,COSTO_MEDIO_N3,INVERSION_N3,CU_MODIFICADO
            var submit = Request.Form["chgSitio"];   //String.IsNullOrEmpty(myString)
            if (ModelState.IsValid && String.IsNullOrEmpty(submit))
            {
                if (mU_PROYECTOS_PECOR.PUESTA_MARCHA < mU_PROYECTOS_PECOR.FIN_EJE_INV)
                {
                    db.Entry(mU_PROYECTOS_PECOR).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                else
                {
                    ViewBag.MsjValidaFechaMarcha = "La fecha de Puesta en Marcha debe ser MENOR a la Fecha final de Ejecución";
                }
            }
/*            if (GlobalVariables.idOrganizacion == null){ViewBag.ID_ORGANIZACION = new SelectList(db.MUB_ORGANIZACIONES, "ID_ORGANIZACION", "RAZON_SOCIAL", mU_PROYECTOS_PECOR.ID_ORGANIZACION);}else{ViewBag.ID_ORGANIZACION = new SelectList(db.MUB_ORGANIZACIONES.Where(o => o.ID_ORGANIZACION == IdOrganizacion), "ID_ORGANIZACION", "RAZON_SOCIAL", GlobalVariables.idOrganizacion);}*/
            ViewBag.ID_CLASE = new SelectList(db.MUB_CLASE_CP, "ID_CLASE_CP", "NOM_CLASE_CP", mU_PROYECTOS_PECOR.ID_CLASE);
            ViewBag.ID_ORGANIZACION = new SelectList(db.MUB_ORGANIZACIONES.Where(o => o.ID_TIPO_ORGANIZACION == 2).OrderBy(o => o.RAZON_SOCIAL), "ID_ORGANIZACION", "RAZON_SOCIAL", mU_PROYECTOS_PECOR.ID_ORGANIZACION);
            ViewBag.ID_TIPO_PROY_PECOR = new SelectList(db.MUB_TIPO_PROY_PECOR, "ID_TIPO_PROY_PECOR", "NOM_TIPO", mU_PROYECTOS_PECOR.ID_TIPO_PROY_PECOR);
            ViewBag.ID_SUBESTACION = new SelectList(db.VISTA_SUBESTACION.Where(o => o.ID_ORGANIZACION == mU_PROYECTOS_PECOR.ID_ORGANIZACION).OrderBy(o => o.NOM_SUBESTACION), "ID_SUBESTACION", "NOM_SUBESTACION", mU_PROYECTOS_PECOR.ID_SUBESTACION);
            ViewBag.ID_PLAN = new SelectList(db.MUB_PECOR_PLAN.Where(f => f.ACTIVO == "S"), "ID_PLAN", "DESCRIPCION", mU_PROYECTOS_PECOR.ID_PLAN);
            return View(mU_PROYECTOS_PECOR);
        }

        // GET: pcProyecto/Delete/5
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MUB_PROYECTOS_PECOR mU_PROYECTOS_PECOR = db.MUB_PROYECTOS_PECOR.Find(id);
            if (mU_PROYECTOS_PECOR == null)
            {
                return HttpNotFound();
            }
            return View(mU_PROYECTOS_PECOR);
        }

        // POST: pcProyecto/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            MUB_PROYECTOS_PECOR mU_PROYECTOS_PECOR = db.MUB_PROYECTOS_PECOR.Find(id);

            //ELIMINA LAS FILAS DE SITIOS RELACIONADOS A LOS CENTROS POBLADOS
            db.MUB_PECOR_CP_VSS.RemoveRange(db.MUB_PECOR_CP_VSS.Where(x => x.ID_PROYECTO_PECOR== id));
            db.SaveChanges();

            db.MUB_PROYECTOS_PECOR.Remove(mU_PROYECTOS_PECOR);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        // GET: pcProyecto/vss/5
        public ActionResult vss(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            return RedirectToAction("Index", "pcVss", new { id = id });
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
