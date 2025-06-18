using Microsoft.AspNetCore.Mvc;

namespace Kervan.Web.Controllers
{
    public class AboutController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
