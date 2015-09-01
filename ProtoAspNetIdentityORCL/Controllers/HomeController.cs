using NSPecor.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace NSPecor.Controllers
{
    public class HomeController : Controller
    {
        private pcUpmeCnx dbUsr = new pcUpmeCnx();
        public ActionResult Index()
        {
            if (User.Identity.IsAuthenticated == true)
            {
                var usr_actual = User.Identity.Name.ToString();
                foreach (var item in dbUsr.MUB_USUARIOS.Where(u => u.EMAIL == usr_actual.ToString()).Select(u => new {ID_USUARIO = u.ID_USUARIO, ID_ORGANIZACION = u.ID_ORGANIZACION }))
                {
                    GlobalVariables.idUsuario = item.ID_USUARIO.ToString();
                    GlobalVariables.idOrganizacion = item.ID_ORGANIZACION.ToString();
                }
            }

            return View();
        }

        public ActionResult Parametros()
        {
           // ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Proyectos()
        {
            //ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}