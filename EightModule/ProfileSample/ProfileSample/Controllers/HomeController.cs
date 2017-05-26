using System.Web.Mvc;

namespace ProfileSample.Controllers
{
	public class HomeController : Controller
    {
        public ActionResult Index()
        {
			return View();
		}

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page";

            return View();
        }

		public ActionResult About()
		{
			ViewBag.Message = "About us";

			return View();
		}
    }
}