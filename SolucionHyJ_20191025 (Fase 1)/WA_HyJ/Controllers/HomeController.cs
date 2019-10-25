using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WA_HyJ.Filter;

namespace WA_HyJ.Controllers
{
    public class HomeController : Controller
    {
        public static string _HandleUnknownAction = "HandleUnknownAction";

        [Compress]
        public ActionResult Index()
        {
            return View();
        }

        [Authorize]
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        [Compress]
        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        protected override void HandleUnknownAction(string actionName)
        {
            TempData[_HandleUnknownAction] = "No se reconoció el método: " + actionName + ".";
            RedirectToAction("Index").ExecuteResult(this.ControllerContext);
        }
    }
}