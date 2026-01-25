using Microsoft.AspNetCore.Mvc;

namespace Poseidon.Controllers
{
    public class RedirectController : Controller
    {
        [Route("invalid-link")]
        public IActionResult InvalidLink()
        {
            return View();
        }
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
