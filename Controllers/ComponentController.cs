using Microsoft.AspNetCore.Mvc;

namespace Poseidon.Controllers
{
    public class ComponentController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
