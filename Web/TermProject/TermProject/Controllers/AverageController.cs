using Microsoft.AspNetCore.Mvc;

namespace TermProject.Controllers
{
    [Route("Average")]
    public class AverageController : Controller
    {
        [HttpGet("")]
        public IActionResult Index()
        {
            return View();
        }
    }
}