using AspNet.Identity.OracleProvider;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NSPecor.Controllers
{
    public class PriorizacionController : Controller
    {

        private readonly OracleDataContext _db;

        public PriorizacionController(OracleDataContext oracleContext)
        {
            _db = oracleContext;
        }

        public ActionResult p1()
        {
            var result = _db.ExecuteQuery(
              String.Format("select MPIO_CCDGO from  MUH_PECOR_COBERTURA "));

            var row = result.Rows.Cast<DataRow>().SingleOrDefault();

            if (row != null)
            {
                return new MUH_PECOR_COBERTURA
                {
                    MPIO_CCDGO = row[0].ToString()
                };
            }

            return null;

        }
        //
        // GET: /Priorizacion/
        public ActionResult Index()
        {
            return View();
        }

        //
        // GET: /Priorizacion/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        //
        // GET: /Priorizacion/Create
        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Priorizacion/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /Priorizacion/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        //
        // POST: /Priorizacion/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /Priorizacion/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        //
        // POST: /Priorizacion/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
