using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using HUTECH_Hospital.Models;
using HUTECH_Hospital.Repositories;

namespace HUTECH_Hospital.Controllers;

public class HomeController : Controller
{
    private readonly IDepartmentRepository _departmentRepo;
    private readonly IDoctorRepository _doctorRepo;

    public HomeController(IDepartmentRepository departmentRepo, IDoctorRepository doctorRepo)
    {
        _departmentRepo = departmentRepo;
        _doctorRepo = doctorRepo;
    }

    public async Task<IActionResult> Index()
    {
        // Lấy 6 chuyên khoa tiêu biểu
        var departments = (await _departmentRepo.GetActiveAsync()).Take(6).ToList();
        
        // Lấy 4 bác sĩ tiêu biểu
        var doctors = (await _doctorRepo.GetActiveAsync()).Take(4).ToList();

        ViewBag.Departments = departments;
        ViewBag.Doctors = doctors;

        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
