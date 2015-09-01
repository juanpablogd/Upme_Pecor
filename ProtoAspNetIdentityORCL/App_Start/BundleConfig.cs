using System.Web;
using System.Web.Optimization;

namespace NSPecor
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/Proyecto").Include(
                        "~/Scripts/Proyecto.js"));

            bundles.Add(new ScriptBundle("~/bundles/PriorizacionUno").Include(
            "~/Scripts/PriorizacionUno.js"));

            bundles.Add(new ScriptBundle("~/bundles/Proyecto_editar").Include(
                        "~/Scripts/Proyecto_editar.js"));

            bundles.Add(new ScriptBundle("~/bundles/Planes").Include(
                        "~/Scripts/Planes.js"));

            bundles.Add(new ScriptBundle("~/bundles/vss").Include(
                        "~/Scripts/vss.js"));

            bundles.Add(new ScriptBundle("~/bundles/vss_valida").Include(
                        "~/Scripts/vss_valida.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/libs/bootstrap-datepicker-1.4.0/js/bootstrap-datepicker.js",
                      "~/Scripts/respond.js"));

            bundles.Add(new ScriptBundle("~/bundles/calendariojs").Include(
                      "~/Scripts/libs/pickadate.js-3.5.6/picker.js",
                      "~/Scripts/libs/pickadate.js-3.5.6/picker.date.js",
                      "~/Scripts/libs/pickadate.js-3.5.6/picker.time.js"
                      ));
            bundles.Add(new StyleBundle("~/bundles/calendariocss").Include(   //"~/Content/bootstrap.css",
                      "~/Scripts/libs/pickadate.js-3.5.6/themes/default.css",
                      "~/Scripts/libs/pickadate.js-3.5.6/themes/default.date.css",
                      "~/Scripts/libs/pickadate.js-3.5.6/themes/default.time.css"));

            bundles.Add(new StyleBundle("~/Content/css").Include(   //"~/Content/bootstrap.css",
                      "~/Content/bootstrap.css",
                      "~/Content/css/main.css",
                      "~/Content/site.css"));
            bundles.Add(new StyleBundle("~/Content/Proyecto").Include(   //"~/Content/bootstrap.css",
                      "~/Content/Proyecto.css"));
        }
    }
}
