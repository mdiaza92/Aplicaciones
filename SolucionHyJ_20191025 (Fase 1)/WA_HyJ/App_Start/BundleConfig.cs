using System.Web;
using System.Web.Optimization;

namespace WA_HyJ
{
    public class BundleConfig
    {
        // Para obtener más información sobre las uniones, visite https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Utilice la versión de desarrollo de Modernizr para desarrollar y obtener información. De este modo, estará
            // para la producción, use la herramienta de compilación disponible en https://modernizr.com para seleccionar solo las pruebas que necesite.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                        "~/Scripts/bootstrap.bundle.min.js"
                      ));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      //"~/Content/bootstrap.css", -> no es usado por LTE
                      //"~/Content/site.css", -> no es usado por LTE
                      "~/Admin-LTE/dist/css/adminlte.min.css",
                      "~/Admin-LTE/dist/css/skins/skin-blue.css"
                      ));

            bundles.Add(new StyleBundle("~/admin-lte/css")
                .Include("~/Admin-LTE/plugins/datatables-bs4/css/dataTables.bootstrap4.css",
                "~/Admin-LTE/plugins/toastr/toastr.min.css",
                "~/Admin-LTE/plugins/select2/css/select2.min.css",
                "~/Admin-LTE/plugins/select2-bootstrap4-theme/select2-bootstrap4.min.css",
                "~/Admin-LTE/plugins/tempusdominus-bootstrap-4/css/tempusdominus-bootstrap-4.min.css",
                "~/Admin-LTE/plugins/datatables-buttons/css/buttons.bootstrap4.min.css",
                "~/Admin-LTE/plugins/datatables-responsive/css/responsive.bootstrap4.min.css",
                "~/Content/bootstrap-fileinput/fileinput.min.css")
                .Include("~/Admin-LTE/plugins/fontawesome-free/css/all.min.css", new CssRewriteUrlTransform()));
            
              bundles.Add(new ScriptBundle("~/bundles/datatables").Include(
                      "~/Admin-LTE/plugins/datatables/jquery.dataTables.js",
                      "~/Admin-LTE/plugins/datatables-bs4/js/dataTables.bootstrap4.js",
                      //Buttons
                      "~/Admin-LTE/plugins/datatables-buttons/js/dataTables.buttons.min.js",
                      "~/Admin-LTE/plugins/datatables-buttons/js/buttons.bootstrap4.min.js",
                      "~/Admin-LTE/plugins/datatables-buttons/js/buttons.html5.min.js",
                      "~/Admin-LTE/plugins/datatables-buttons/js/buttons.colVis.min.js",
                      //Responsibe
                      "~/Admin-LTE/plugins/datatables-responsive/jss/dataTables.responsive.min.js"));

            bundles.Add(new ScriptBundle("~/admin-lte/js").Include(
             "~/Admin-LTE/dist/js/adminlte.js",
             "~/Admin-LTE/plugins/toastr/toastr.min.js",
             "~/Admin-LTE/plugins/select2/js/select2.full.min.js",
             "~/Admin-LTE/plugins/moment/moment.min.js",
             "~/Admin-LTE/plugins/tempusdominus-bootstrap-4/js/tempusdominus-bootstrap-4.min.js",
            #region File input
            //piexif.min.js is needed for auto orienting image files OR when restoring exif data in resized images and when you
            //wish to resize images before upload.This must be loaded before fileinput.min.js
             "~/Scripts/plugins/piexif.min.js",
            //sortable.min.js is only needed if you wish to sort / rearrange files in initial preview. 
            //This must be loaded before fileinput.min.js
             "~/Scripts/plugins/sortable.min.js",
             //purify.min.js is only needed if you wish to purify HTML content in your preview for 
            //HTML files.This must be loaded before fileinput.min.js
             "~/Scripts/plugins/purify.min.js",
             //popper.min.js below is needed if you use bootstrap 4.x(for popover and tooltips).You can also use the bootstrap js
            //3.3.x versions without popper.min.js.
             "~/Scripts/umd/pooper.min.js",
             "~/Scripts/fileinput.min.js",
             "~/Content/bootstrap-fileinput/themes/fas/theme.min.js",
             "~/Scripts/locales/es.js"
             #endregion
             ));

            #region Bootstrap dialog
            bundles.Add(new ScriptBundle("~/bundles/bootstrap-dialog-script").Include(
                        "~/Scripts/bootstrap-dialog.js"));

            bundles.Add(new StyleBundle("~/Content/bootstrap-dialog-style").Include(
                     "~/Content/bootstrap-dialog.css"));
            #endregion
        }
    }
}
