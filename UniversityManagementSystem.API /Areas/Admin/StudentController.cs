using Microsoft.AspNetCore.Mvc;

namespace UniversityManagementSystem.API.Areas.Admin;

public class StudentController : Controller
{
    // GET
    public IActionResult Index()
    {
        return View();
    }
}