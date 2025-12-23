using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace TermProject.Controllers;
[Authorize]
public class ProfileController : Controller
{
    public IActionResult MyListings()
    {
        return View();
    }

    public IActionResult Settings()
    {
        return View();
    }
}