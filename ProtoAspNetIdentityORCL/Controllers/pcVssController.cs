using NSPecor.Models;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using AspNet.Identity.OracleProvider;

namespace NSPecor.Controllers
{
    public class pcVssController : Controller
    {
        private pcUpmeCnx db = new pcUpmeCnx();
        //public long id_proy { get; set; }
        //static string id_proy;
        public static long id_proy;
        public static long py_id_dpto;
        public static long py_id_mpio;


        // GET: pcVss
        public ActionResult Index(long? id)
        {
            //Response.Write("Antes de: " + id_proy);
            if (id != null)
            {
                id_proy = (long)id;
            }
            
            py_id_dpto = 0;
            py_id_mpio = 0;

            var suma_vss = db.MUB_PECOR_CP_VSS.Where(u => u.ID_PROYECTO_PECOR == (long)id_proy).Sum(v => v.VSS_BENEF).ToString();
            var suma_dem = db.MUB_PECOR_CP_VSS.Where(u => u.ID_PROYECTO_PECOR == (long)id_proy).Sum(v => v.DEMANDA_ANUAL).ToString();
            if (suma_vss == "" || suma_dem == "")
            {
                suma_vss = "0";
                suma_dem = "0";
            }
            ViewBag.SumaVss = suma_vss;
            ViewBag.SumaDem = suma_dem;

           var mU_PECOR_CP_VSS = (from b in db.MUB_PECOR_CP_VSS
                         join c in db.BC_DP_SITIOS_UPME on b.ID_CENTRO_POBLADO equals c.ID_CENTRO_POBLADO into a
                         join d in db.MUB_PROYECTOS_PECOR on b.ID_PROYECTO_PECOR equals d.ID_PROYECTO_PECOR into aa
                         select b).ToList();
            
           return View(mU_PECOR_CP_VSS.Where(s => s.ID_PROYECTO_PECOR == id_proy).ToList());
            
        }

        public ActionResult Listado(long? id)
        {
            var mU_PECOR_CP_VSS = db.MUB_PECOR_CP_VSS.Include(m => m.BC_DP_SITIOS_UPME).Include(m => m.MUB_PROYECTOS_PECOR);
            return View(mU_PECOR_CP_VSS.ToList());
        }

        // GET: pcVss/Details/5
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MUB_PECOR_CP_VSS mU_PECOR_CP_VSS = db.MUB_PECOR_CP_VSS.Find(id);
            if (mU_PECOR_CP_VSS == null)
            {
                return HttpNotFound();
            }
            return View(mU_PECOR_CP_VSS);
        }

        public class DistinctDepto : IEqualityComparer<VISTA_BC_MPIO_DPTO>
        {
            public bool Equals(VISTA_BC_MPIO_DPTO x, VISTA_BC_MPIO_DPTO y)
            {
                return x.DPTO_CCDGO.Equals(y.DPTO_CCDGO);
            }

            public int GetHashCode(VISTA_BC_MPIO_DPTO obj)
            {
                return obj.DPTO_CCDGO.GetHashCode();
            }
        }
        // GET: pcVss/Create
        public ActionResult Create()
        {
            //Response.Write("INICIO Id Proy Crear: " + id_proy.ToString());
            //SI NO HAY UN PROYECTO SELECCIONADO
            if (id_proy.ToString().CompareTo("0") == 0)
            {
                return RedirectToAction("Index", "pcProyecto");
            }
            //VALIDA SI ES PROGRAMA O PROYECTO
            var tipo = (from b in db.MUB_PROYECTOS_PECOR
                        where b.ID_PROYECTO_PECOR == id_proy
                        select b.PROG_PROY).ToList();
            if (tipo[0].ToString() == "py")
            {
                var cp_py = (from c in db.BC_DP_SITIOS_UPME
                             join v in db.MUB_PECOR_CP_VSS on c.ID_CENTRO_POBLADO equals v.ID_CENTRO_POBLADO
                             where v.ID_PROYECTO_PECOR == id_proy
                             select new { c.ID_CENTRO_POBLADO}
                    ).ToList();

                if (cp_py.Count > 0)
                {
                    //Response.Write("id_centropoblado: " + cp_py[0].ID_CENTRO_POBLADO);
                    int idcp_tmp = cp_py[0].ID_CENTRO_POBLADO;
                    IQueryable<VISTA_BC_MPIO_DPTO> mpio = (from m in db.VISTA_BC_MPIO_DPTO
                                  join s in db.BC_DP_SITIOS_UPME on m.MPIO_CCDGO equals s.COD_MPIO
                                  where s.COD_DPTO == m.DPTO_CCDGO && s.ID_CENTRO_POBLADO == idcp_tmp
                                  select m);
                    py_id_dpto = Convert.ToInt64(mpio.FirstOrDefault().DPTO_CCDGO);
                    py_id_mpio = Convert.ToInt64(mpio.FirstOrDefault().MPIO_CCDGO);
                }
            }

            IEqualityComparer<VISTA_BC_MPIO_DPTO> customComparer = new DistinctDepto();
            IEnumerable<VISTA_BC_MPIO_DPTO> y;
            if (py_id_dpto != null && py_id_dpto != 0)
            {
                string id_dpto_tmp = py_id_dpto.ToString();
                y = db.VISTA_BC_MPIO_DPTO.Where(d => d.DPTO_CCDGO == id_dpto_tmp).OrderBy(s => s.DPTO_CNMBR).ToList().Distinct(customComparer);
                ViewBag.COD_DPTO = new SelectList(y, "DPTO_CCDGO", "DPTO_CNMBR", id_dpto_tmp);
            }
            else
            {
                y = db.VISTA_BC_MPIO_DPTO.OrderBy(s => s.DPTO_CNMBR).ToList().Distinct(customComparer);
                ViewBag.COD_DPTO = new SelectList(y, "DPTO_CCDGO", "DPTO_CNMBR");
            }
            
            if (py_id_mpio != null && py_id_mpio != 0)
            {
                string id_dpto_tmp = py_id_dpto.ToString();
                string id_mpio_tmp = py_id_mpio.ToString();
                ViewBag.COD_MPIO = new SelectList(db.VISTA_BC_MPIO_DPTO.Where(s => s.MPIO_CCDGO == id_mpio_tmp).OrderBy(s => s.MPIO_CNMBR), "MPIO_CCDGO", "MPIO_CNMBR", id_mpio_tmp);
                ViewBag.ID_CENTRO_POBLADO = new SelectList(db.BC_DP_SITIOS_UPME.Where(j => j.COD_MPIO == id_mpio_tmp && j.COD_DPTO == id_dpto_tmp).OrderBy(s => s.NOMBRE_SITIO), "ID_CENTRO_POBLADO", "NOMBRE_SITIO");
            }
            else
            {
                ViewBag.COD_MPIO = new SelectList(db.VISTA_BC_MPIO_DPTO.Where(s => s.DPTO_CCDGO == "").OrderBy(s=>s.MPIO_CNMBR), "MPIO_CCDGO", "MPIO_CNMBR");
                ViewBag.ID_CENTRO_POBLADO = new SelectList(db.BC_DP_SITIOS_UPME.Where(j => j.ID_CENTRO_POBLADO == -1), "ID_CENTRO_POBLADO", "NOMBRE_SITIO");
            }
            
            ViewBag.ID_PROYECTO_PECOR = new SelectList(db.MUB_PROYECTOS_PECOR.Where(s => s.ID_PROYECTO_PECOR == id_proy), "ID_PROYECTO_PECOR", "CODIGO_UPME");

            return View();
        }

        // POST: pcVss/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID_PROYECTO_PECOR,ID_CENTRO_POBLADO,VSS_BENEF,DEMANDA_ANUAL")] MUB_PECOR_CP_VSS mU_PECOR_CP_VSS)
        {
            //Response.Write("SUBMIT Id Proy Crear: " + id_proy.ToString());
            //Response.Write(Request.Form["COD_DPTO"].ToString() + " " + Request.Form["COD_MPIO"].ToString());
            //Response.Write(Request.Form["chgDepto"].ToString());
            var cod_depto = Request.Form["COD_DPTO"].ToString();
            var cod_mpio = Request.Form["COD_MPIO"].ToString();
            if (ModelState.IsValid && Request.Form["chgSitio"] != "true")
            {
                //Response.Write("vss Antes de Guardar: " + mU_PECOR_CP_VSS.VSS_BENEF);
                //Response.Write("Mes: " + DateTime.Now.Year.ToString() + " " + DateTime.Now.Month.ToString());
                //Response.Write("Fecha: " + DateTime.Now.ToString("yyyyMM"));
                long fecha_consulta = Convert.ToInt64(DateTime.Now.ToString("yyyyMM"));
                //VALIDA CON RESPECTO AL NÚMERO TOTAL DE VIVIENDAS SIN SERVICIO EL VALOR DIGITADO
                var VssCentroPob = db.MUB_VSS.Where(m => m.ID_CENTRO_POBLADO == mU_PECOR_CP_VSS.ID_CENTRO_POBLADO).Where(f => f.VIGENCIA == fecha_consulta);
                long totalVss = 0;
                foreach (MUB_VSS cust in VssCentroPob)
                {
                    totalVss = totalVss + (long)cust.VSS_TOTAL;
                }
                //VALIDA SI EL CENTRO POBLADO YA EXISTE!
                var ExisteVssCentroPob = db.MUB_PECOR_CP_VSS.Where(m => m.ID_CENTRO_POBLADO == mU_PECOR_CP_VSS.ID_CENTRO_POBLADO).Where(f => f.ID_PROYECTO_PECOR == id_proy);
                var Repetido = 0;
                foreach (MUB_PECOR_CP_VSS cust in ExisteVssCentroPob)
                {
                    Repetido = 1;
                }
                //Response.Write("vss MAX: " + totalVss + "  - Valor: " + mU_PECOR_CP_VSS.VSS_BENEF);
                //validación manual 
                if (Repetido == 0) {
                    if (mU_PECOR_CP_VSS.VSS_BENEF.ToString() != "") {
                        if (mU_PECOR_CP_VSS.DEMANDA_ANUAL.ToString() != "")
                        {
                            if (totalVss == 0 || totalVss >= mU_PECOR_CP_VSS.VSS_BENEF)
                            {
                                //CALCULA VSS Y DEMANDA TOTAL
                                var suma_vss = db.MUB_PECOR_CP_VSS.Where(u => u.ID_PROYECTO_PECOR == (long)id_proy).Sum(v => v.VSS_BENEF).ToString();
                                var suma_dem = db.MUB_PECOR_CP_VSS.Where(u => u.ID_PROYECTO_PECOR == (long)id_proy).Sum(v => v.DEMANDA_ANUAL).ToString();
                                if (suma_vss == "" || suma_dem == "")
                                {
                                    suma_vss = "0";
                                    suma_dem = "0";
                                }

                                //GUARDA EL ID DEL PROYECTO
                                mU_PECOR_CP_VSS.ID_PROYECTO_PECOR = id_proy;
                                db.MUB_PECOR_CP_VSS.Add(mU_PECOR_CP_VSS);
                                db.SaveChanges();

                                //ACTUALIZA TOTAL VSS Y DEMANDA EN EL PROYECTO
                                MUB_PROYECTOS_PECOR proyecto_actualiza = db.MUB_PROYECTOS_PECOR.Where(u => u.ID_PROYECTO_PECOR == (long)id_proy).First();
                                proyecto_actualiza.V_BENEFICIA = Convert.ToInt64(suma_vss) + Convert.ToInt64(mU_PECOR_CP_VSS.VSS_BENEF);
                                proyecto_actualiza.DEMANDA_ANUAL = Convert.ToInt64(suma_dem) + Convert.ToInt64(mU_PECOR_CP_VSS.DEMANDA_ANUAL);

                                db.MUB_PROYECTOS_PECOR.Attach(proyecto_actualiza); // Adiciona en estado descargado
                                db.Entry(proyecto_actualiza).Property(r => r.V_BENEFICIA).IsModified = true;
                                db.Entry(proyecto_actualiza).Property(r => r.DEMANDA_ANUAL).IsModified = true;
                                db.SaveChanges();

                                //Redirecciona al listado
                                return RedirectToAction("Index");
                            }
                            else
                            {
                                ViewBag.MsjValida = "El valor de viviendas no puede ser mayor a " + totalVss;
                            }
                        }
                        else
                        {
                            ViewBag.MsjValidaDemanda = "Debe ingresar la Demanda.";
                        }
                    } else
                    {
                        ViewBag.MsjValida = "Debe ingresar el número de Viviendas Beneficiadas sin servicio ";
                    }
                } else
                {
                    ViewBag.MsjValidaCentroPob = "El centro poblado ya existe para este Proyecto";
                }
            }
            //VALIDA SI ES PROGRAMA O PROYECTO
            var tipo = (from b in db.MUB_PROYECTOS_PECOR
                        where b.ID_PROYECTO_PECOR == id_proy
                        select b.PROG_PROY).ToList();
            if (tipo[0].ToString() == "py")
            {
                var cp_py = (from c in db.BC_DP_SITIOS_UPME
                             join v in db.MUB_PECOR_CP_VSS on c.ID_CENTRO_POBLADO equals v.ID_CENTRO_POBLADO
                             where v.ID_PROYECTO_PECOR == id_proy
                             select new { c.ID_CENTRO_POBLADO }
                    ).ToList();

                if (cp_py.Count > 0)
                {
                    //Response.Write("id_centropoblado: " + cp_py[0].ID_CENTRO_POBLADO);
                    int idcp_tmp = cp_py[0].ID_CENTRO_POBLADO;
                    IQueryable<VISTA_BC_MPIO_DPTO> mpio = (from m in db.VISTA_BC_MPIO_DPTO
                                                           join s in db.BC_DP_SITIOS_UPME on m.MPIO_CCDGO equals s.COD_MPIO
                                                           where s.COD_DPTO == m.DPTO_CCDGO && s.ID_CENTRO_POBLADO == idcp_tmp
                                                           select m);
                    py_id_dpto = Convert.ToInt64(mpio.FirstOrDefault().DPTO_CCDGO);
                    py_id_mpio = Convert.ToInt64(mpio.FirstOrDefault().MPIO_CCDGO);
                }
            }

            IEqualityComparer<VISTA_BC_MPIO_DPTO> customComparer = new DistinctDepto();
            IEnumerable<VISTA_BC_MPIO_DPTO> y;
            if (py_id_dpto != null && py_id_dpto != 0)
            {
                string id_dpto_tmp = py_id_dpto.ToString();
                y = db.VISTA_BC_MPIO_DPTO.Where(d => d.DPTO_CCDGO == id_dpto_tmp).OrderBy(s => s.DPTO_CNMBR).ToList().Distinct(customComparer);
                ViewBag.COD_DPTO = new SelectList(y, "DPTO_CCDGO", "DPTO_CNMBR", id_dpto_tmp);
            }
            else
            {
                y = db.VISTA_BC_MPIO_DPTO.OrderBy(s => s.DPTO_CNMBR).ToList().Distinct(customComparer);
                ViewBag.COD_DPTO = new SelectList(y, "DPTO_CCDGO", "DPTO_CNMBR", cod_depto);
            }

            if (py_id_mpio != null && py_id_mpio != 0)
            {
                string id_dpto_tmp = py_id_dpto.ToString();
                string id_mpio_tmp = py_id_mpio.ToString();
                ViewBag.COD_MPIO = new SelectList(db.VISTA_BC_MPIO_DPTO.Where(s => s.MPIO_CCDGO == id_mpio_tmp).OrderBy(s => s.MPIO_CNMBR), "MPIO_CCDGO", "MPIO_CNMBR", id_mpio_tmp);
                ViewBag.ID_CENTRO_POBLADO = new SelectList(db.BC_DP_SITIOS_UPME.Where(j => j.COD_MPIO == id_mpio_tmp && j.COD_DPTO == id_dpto_tmp), "ID_CENTRO_POBLADO", "NOMBRE_SITIO", mU_PECOR_CP_VSS.ID_CENTRO_POBLADO);
            }
            else
            {
                ViewBag.COD_MPIO = new SelectList(db.VISTA_BC_MPIO_DPTO.Where(s => s.DPTO_CCDGO == cod_depto).OrderBy(s => s.MPIO_CNMBR), "MPIO_CCDGO", "MPIO_CNMBR", cod_mpio);
                ViewBag.ID_CENTRO_POBLADO = new SelectList(db.BC_DP_SITIOS_UPME.Where(s => s.COD_MPIO == cod_mpio), "ID_CENTRO_POBLADO", "NOMBRE_SITIO", mU_PECOR_CP_VSS.ID_CENTRO_POBLADO);
            }
            //CARGA ------PROYECTO------
            ViewBag.ID_PROYECTO_PECOR = new SelectList(db.MUB_PROYECTOS_PECOR.Where(s => s.ID_PROYECTO_PECOR == id_proy), "ID_PROYECTO_PECOR", "CODIGO_UPME", mU_PECOR_CP_VSS.ID_PROYECTO_PECOR);
            //ViewBag.ID_PROYECTO_PECOR = new SelectList(db.MU_PROYECTOS_PECOR, "ID_PROYECTO_PECOR", "CODIGO_UPME", mU_PECOR_CP_VSS.ID_PROYECTO_PECOR);
            return View(mU_PECOR_CP_VSS);
        }

        // GET: pcVss/Edit/5
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            } else if (id_proy.ToString().CompareTo("0") == 0)
            {
                return RedirectToAction("Index", "pcProyecto");
            }
            MUB_PECOR_CP_VSS mU_PECOR_CP_VSS = db.MUB_PECOR_CP_VSS.Find(id);
            if (mU_PECOR_CP_VSS == null)
            {
                return HttpNotFound();
            }

            //VALIDA SI ES PROGRAMA O PROYECTO
            var tipo = (from b in db.MUB_PROYECTOS_PECOR
                        where b.ID_PROYECTO_PECOR == id_proy
                        select b.PROG_PROY).ToList();
            if (tipo[0].ToString() == "py")
            {
                var cp_py = (from c in db.BC_DP_SITIOS_UPME
                             join v in db.MUB_PECOR_CP_VSS on c.ID_CENTRO_POBLADO equals v.ID_CENTRO_POBLADO
                             where v.ID_PROYECTO_PECOR == id_proy
                             select new { c.ID_CENTRO_POBLADO }
                    ).ToList();

                if (cp_py.Count > 0)
                {
                    //Response.Write("id_centropoblado: " + cp_py[0].ID_CENTRO_POBLADO);

                    int idcp_tmp = cp_py[0].ID_CENTRO_POBLADO;
                    IQueryable<VISTA_BC_MPIO_DPTO> mpio = (from m in db.VISTA_BC_MPIO_DPTO
                                                           join s in db.BC_DP_SITIOS_UPME on m.MPIO_CCDGO equals s.COD_MPIO
                                                           where s.COD_DPTO == m.DPTO_CCDGO && s.ID_CENTRO_POBLADO == idcp_tmp
                                                           select m);
                    py_id_dpto = Convert.ToInt64(mpio.FirstOrDefault().DPTO_CCDGO);
                    py_id_mpio = Convert.ToInt64(mpio.FirstOrDefault().MPIO_CCDGO);
                }
            }

            IEqualityComparer<VISTA_BC_MPIO_DPTO> customComparer = new DistinctDepto();
            IEnumerable<VISTA_BC_MPIO_DPTO> y;
            if (py_id_dpto != null && py_id_dpto != 0)
            {
                string id_dpto_tmp = py_id_dpto.ToString();
                y = db.VISTA_BC_MPIO_DPTO.Where(d => d.DPTO_CCDGO == id_dpto_tmp).ToList().Distinct(customComparer);
                ViewBag.COD_DPTO = new SelectList(y, "DPTO_CCDGO", "DPTO_CNMBR", id_dpto_tmp);
            }
            else
            {
                y = db.VISTA_BC_MPIO_DPTO.ToList().Distinct(customComparer);
                ViewBag.COD_DPTO = new SelectList(y, "DPTO_CCDGO", "DPTO_CNMBR");
            }

            if (py_id_mpio != null && py_id_mpio != 0)
            {
                string id_dpto_tmp = py_id_dpto.ToString();
                string id_mpio_tmp = py_id_mpio.ToString();
                ViewBag.COD_MPIO = new SelectList(db.VISTA_BC_MPIO_DPTO.Where(s => s.MPIO_CCDGO == id_mpio_tmp).OrderBy(s => s.MPIO_CNMBR), "MPIO_CCDGO", "MPIO_CNMBR", id_mpio_tmp);
                ViewBag.ID_CENTRO_POBLADO = new SelectList(db.BC_DP_SITIOS_UPME.Where(j => j.COD_MPIO == id_mpio_tmp && j.COD_DPTO == id_dpto_tmp).OrderBy(s => s.NOMBRE_SITIO), "ID_CENTRO_POBLADO", "NOMBRE_SITIO", mU_PECOR_CP_VSS.ID_CENTRO_POBLADO);
            }
            else
            {
                ViewBag.COD_MPIO = new SelectList(db.VISTA_BC_MPIO_DPTO.Where(s => s.DPTO_CCDGO == "").OrderBy(s => s.MPIO_CNMBR), "MPIO_CCDGO", "MPIO_CNMBR");
                ViewBag.ID_CENTRO_POBLADO = new SelectList(db.BC_DP_SITIOS_UPME.Where(j => j.ID_CENTRO_POBLADO == -1), "ID_CENTRO_POBLADO", "NOMBRE_SITIO");
            }

            //CARGA PROYECTOS SELECCIONANDO EL VALOR POR DEFECTO                        
            ViewBag.ID_PROYECTO_PECOR = new SelectList(db.MUB_PROYECTOS_PECOR.Where(s => s.ID_PROYECTO_PECOR == id_proy), "ID_PROYECTO_PECOR", "CODIGO_UPME", mU_PECOR_CP_VSS.ID_PROYECTO_PECOR);
            return View(mU_PECOR_CP_VSS);

        }

        // POST: pcVss/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID_PROYECTO_PECOR,ID_CENTRO_POBLADO,VSS_BENEF,DEMANDA_ANUAL,ID_CP_PY")] MUB_PECOR_CP_VSS mU_PECOR_CP_VSS)
        {
            var cod_depto = Request.Form["COD_DPTO"].ToString();
            var cod_mpio = Request.Form["COD_MPIO"].ToString();
            if (ModelState.IsValid && Request.Form["chgSitio"] != "true")
            {
                //Response.Write("vss Antes de Guardar: " + mU_PECOR_CP_VSS.VSS_BENEF);
                //VALIDA CON RESPECTO AL NÚMERO TOTAL DE VIVIENDAS SIN SERVICIO EL VALOR DIGITADO
                var VssCentroPob = db.MUB_VSS.Where(m => m.ID_CENTRO_POBLADO == mU_PECOR_CP_VSS.ID_CENTRO_POBLADO);
                long totalVss = 0;

                foreach (MUB_VSS cust in VssCentroPob)
                {
                    totalVss = totalVss + (long)cust.VSS_TOTAL;
                }
                //VALIDA SI EL CENTRO POBLADO YA EXISTE!
                var ExisteVssCentroPob = db.MUB_PECOR_CP_VSS.Where(m => m.ID_CENTRO_POBLADO == mU_PECOR_CP_VSS.ID_CENTRO_POBLADO).Where(m => m.ID_CP_PY != mU_PECOR_CP_VSS.ID_CP_PY).Where(f => f.ID_PROYECTO_PECOR == mU_PECOR_CP_VSS.ID_PROYECTO_PECOR);
                var Repetido = 0;

                foreach (MUB_PECOR_CP_VSS cust in ExisteVssCentroPob)
                {
                    Repetido = 1;
                }

                //validación manual 
                if (Repetido == 0)
                {
                    if (mU_PECOR_CP_VSS.VSS_BENEF.ToString() != "")
                    {
                        if (mU_PECOR_CP_VSS.DEMANDA_ANUAL.ToString() != "")
                        {   //Response.Write("vss MAX: " + totalVss + "  - Valor: " + mU_PECOR_CP_VSS.VSS_BENEF);
                            if (totalVss == 0 || totalVss >= mU_PECOR_CP_VSS.VSS_BENEF)
                            {
                                //CALCULA VSS Y DEMANDA TOTAL
                                var suma_vss = db.MUB_PECOR_CP_VSS.Where(u => u.ID_PROYECTO_PECOR == (long)id_proy && u.ID_CP_PY != mU_PECOR_CP_VSS.ID_CP_PY).Sum(v => v.VSS_BENEF).ToString();
                                var suma_dem = db.MUB_PECOR_CP_VSS.Where(u => u.ID_PROYECTO_PECOR == (long)id_proy && u.ID_CP_PY != mU_PECOR_CP_VSS.ID_CP_PY).Sum(v => v.DEMANDA_ANUAL).ToString();
                                
                                //GUARDA EL ID DEL PROYECTO
                                db.Entry(mU_PECOR_CP_VSS).State = EntityState.Modified;
                                db.SaveChanges();

                                //ACTUALIZA TOTAL VSS Y DEMANDA EN EL PROYECTO
                                MUB_PROYECTOS_PECOR proyecto_actualiza = db.MUB_PROYECTOS_PECOR.Where(u => u.ID_PROYECTO_PECOR == (long)id_proy).First();
                                proyecto_actualiza.V_BENEFICIA = Convert.ToInt64(suma_vss) + Convert.ToInt64(mU_PECOR_CP_VSS.VSS_BENEF);
                                proyecto_actualiza.DEMANDA_ANUAL = Convert.ToInt64(suma_dem) + Convert.ToInt64(mU_PECOR_CP_VSS.DEMANDA_ANUAL);
                                db.MUB_PROYECTOS_PECOR.Attach(proyecto_actualiza); // Adiciona en estado descargado
                                db.Entry(proyecto_actualiza).Property(r => r.V_BENEFICIA).IsModified = true;
                                db.Entry(proyecto_actualiza).Property(r => r.DEMANDA_ANUAL).IsModified = true;
                                db.SaveChanges();


                                return RedirectToAction("Index");
                            }
                            else
                            {
                                ViewBag.MsjValida = "El valor de viviendas no puede ser mayor a " + totalVss;
                            }
                        }
                        else
                        {
                            ViewBag.MsjValidaDemanda = "Debe ingresar la Demanda.";
                        }
                    }
                    else
                    {
                        ViewBag.MsjValida = "Debe ingresar el número de Viviendas Beneficiadas sin servicio ";
                    }
                }
                else
                {
                    ViewBag.MsjValidaCentroPob = "El centro poblado ya existe para este Proyecto";
                }
            }
/*            //CARGA ------DEPARTAMENTO------                  */
            //VALIDA SI ES PROGRAMA O PROYECTO
            var tipo = (from b in db.MUB_PROYECTOS_PECOR
                        where b.ID_PROYECTO_PECOR == id_proy
                        select b.PROG_PROY).ToList();
            if (tipo[0].ToString() == "py")
            {
                var cp_py = (from c in db.BC_DP_SITIOS_UPME
                             join v in db.MUB_PECOR_CP_VSS on c.ID_CENTRO_POBLADO equals v.ID_CENTRO_POBLADO
                             where v.ID_PROYECTO_PECOR == id_proy
                             select new { c.ID_CENTRO_POBLADO }
                    ).ToList();

                if (cp_py.Count > 0)
                {
                    //Response.Write("id_centropoblado: " + cp_py[0].ID_CENTRO_POBLADO);

                    int idcp_tmp = cp_py[0].ID_CENTRO_POBLADO;
                    IQueryable<VISTA_BC_MPIO_DPTO> mpio = (from m in db.VISTA_BC_MPIO_DPTO
                                                           join s in db.BC_DP_SITIOS_UPME on m.MPIO_CCDGO equals s.COD_MPIO
                                                           where s.COD_DPTO == m.DPTO_CCDGO && s.ID_CENTRO_POBLADO == idcp_tmp
                                                           select m);
                    py_id_dpto = Convert.ToInt64(mpio.FirstOrDefault().DPTO_CCDGO);
                    py_id_mpio = Convert.ToInt64(mpio.FirstOrDefault().MPIO_CCDGO);
                }
            }

            IEqualityComparer<VISTA_BC_MPIO_DPTO> customComparer = new DistinctDepto();
            IEnumerable<VISTA_BC_MPIO_DPTO> y;
            if (py_id_dpto != null && py_id_dpto != 0)
            {
                string id_dpto_tmp = py_id_dpto.ToString();
                y = db.VISTA_BC_MPIO_DPTO.Where(d => d.DPTO_CCDGO == id_dpto_tmp).OrderBy(s => s.DPTO_CNMBR).ToList().Distinct(customComparer);
                ViewBag.COD_DPTO = new SelectList(y, "DPTO_CCDGO", "DPTO_CNMBR", id_dpto_tmp);
            }
            else
            {
                y = db.VISTA_BC_MPIO_DPTO.OrderBy(s => s.DPTO_CNMBR).ToList().Distinct(customComparer);
                ViewBag.COD_DPTO = new SelectList(y, "DPTO_CCDGO", "DPTO_CNMBR");
            }

            if (py_id_mpio != null && py_id_mpio != 0)
            {
                string id_dpto_tmp = py_id_dpto.ToString();
                string id_mpio_tmp = py_id_mpio.ToString();
                ViewBag.COD_MPIO = new SelectList(db.VISTA_BC_MPIO_DPTO.Where(s => s.MPIO_CCDGO == id_mpio_tmp).OrderBy(s => s.MPIO_CNMBR), "MPIO_CCDGO", "MPIO_CNMBR", id_mpio_tmp);
                ViewBag.ID_CENTRO_POBLADO = new SelectList(db.BC_DP_SITIOS_UPME.Where(j => j.COD_MPIO == id_mpio_tmp && j.COD_DPTO == id_dpto_tmp), "ID_CENTRO_POBLADO", "NOMBRE_SITIO", mU_PECOR_CP_VSS.ID_CENTRO_POBLADO);
            }
            else
            {
                ViewBag.COD_MPIO = new SelectList(db.VISTA_BC_MPIO_DPTO.Where(s => s.DPTO_CCDGO == "").OrderBy(s => s.MPIO_CNMBR), "MPIO_CCDGO", "MPIO_CNMBR");
                ViewBag.ID_CENTRO_POBLADO = new SelectList(db.BC_DP_SITIOS_UPME.Where(j => j.ID_CENTRO_POBLADO == -1), "ID_CENTRO_POBLADO", "NOMBRE_SITIO");
            }
            //CARGA ------PROYECTO------
            ViewBag.ID_PROYECTO_PECOR = new SelectList(db.MUB_PROYECTOS_PECOR.Where(s => s.ID_PROYECTO_PECOR == id_proy), "ID_PROYECTO_PECOR", "CODIGO_UPME");
            return View(mU_PECOR_CP_VSS);
        }

        // GET: pcVss/Delete/5
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MUB_PECOR_CP_VSS mU_PECOR_CP_VSS = db.MUB_PECOR_CP_VSS.Find(id);
            if (mU_PECOR_CP_VSS == null)
            {
                return HttpNotFound();
            }
            return View(mU_PECOR_CP_VSS);
        }

        // POST: pcVss/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            //CALCULA VSS Y DEMANDA TOTAL
            var suma_vss = db.MUB_PECOR_CP_VSS.Where(u => u.ID_PROYECTO_PECOR == (long)id_proy && u.ID_CP_PY != id).Sum(v => v.VSS_BENEF).ToString();
            var suma_dem = db.MUB_PECOR_CP_VSS.Where(u => u.ID_PROYECTO_PECOR == (long)id_proy && u.ID_CP_PY != id).Sum(v => v.DEMANDA_ANUAL).ToString();
            if (suma_vss == "" || suma_dem == "")
            {
                suma_vss = "0";
                suma_dem = "0";
            }

            MUB_PECOR_CP_VSS mU_PECOR_CP_VSS = db.MUB_PECOR_CP_VSS.Find(id);
            db.MUB_PECOR_CP_VSS.Remove(mU_PECOR_CP_VSS);
            db.SaveChanges();

            //ACTUALIZA TOTAL VSS Y DEMANDA EN EL PROYECTO
            MUB_PROYECTOS_PECOR proyecto_actualiza = db.MUB_PROYECTOS_PECOR.Where(u => u.ID_PROYECTO_PECOR == (long)id_proy).First();
            proyecto_actualiza.V_BENEFICIA = Convert.ToInt64(suma_vss);
            proyecto_actualiza.DEMANDA_ANUAL = Convert.ToInt64(suma_dem);
            db.MUB_PROYECTOS_PECOR.Attach(proyecto_actualiza); // Adiciona en estado descargado
            db.Entry(proyecto_actualiza).Property(r => r.V_BENEFICIA).IsModified = true;
            db.Entry(proyecto_actualiza).Property(r => r.DEMANDA_ANUAL).IsModified = true;
            db.SaveChanges();


            return RedirectToAction("Index");
        }

        // GET: pcProyecto/vss/5
        public ActionResult proyectos()
        {
            return RedirectToAction("Index", "pcProyecto");
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
