using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace TermProject.ViewComponents;
public class UserMenuViewComponent : ViewComponent
{
    public async Task<IViewComponentResult> InvokeAsync()
    {
        var isAuthenticated = HttpContext.User?.Identity?.IsAuthenticated ?? false;
        var username = isAuthenticated ? HttpContext.User.Identity.Name : null;
        var model = new { IsAuthenticated = isAuthenticated, Username = username };
        return View(model);
    }
}