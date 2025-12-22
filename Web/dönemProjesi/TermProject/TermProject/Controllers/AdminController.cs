using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace TermProject.Controllers;
[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    public IActionResult Dashboard()
    {
        ViewData["Title"] = "Admin Dashboard";
        return View();
    }

    public IActionResult ApproveListings()
    {
        return View();
    }

    public IActionResult UserManagement()
    {
        return View();
    }
}