using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HUTECH_Hospital.Models;
using HUTECH_Hospital.Repositories;
using HUTECH_Hospital.Services;
using HUTECH_Hospital.ViewModels;

namespace HUTECH_Hospital.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class DepartmentController : Controller
    {
        private readonly IDepartmentRepository _departmentRepo;
        private readonly FileUploadService _fileUploadService;

        public DepartmentController(IDepartmentRepository departmentRepo, FileUploadService fileUploadService)
        {
            _departmentRepo = departmentRepo;
            _fileUploadService = fileUploadService;
        }

        public async Task<IActionResult> Index()
        {
            var departments = await _departmentRepo.GetAllAsync();
            return View(departments);
        }

        public async Task<IActionResult> Details(int id)
        {
            var department = await _departmentRepo.GetByIdAsync(id);
            if (department == null) return NotFound();
            return View(department);
        }

        public IActionResult Create()
        {
            return View(new DepartmentViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DepartmentViewModel model)
        {
            if (ModelState.IsValid)
            {
                var department = new Department
                {
                    Name = model.Name,
                    Description = model.Description,
                    IsActive = model.IsActive
                };

                // Handle Image Upload
                if (model.ImageFile != null)
                {
                    department.ImageUrl = await _fileUploadService.UploadFileAsync(model.ImageFile, "departments");
                }

                await _departmentRepo.AddAsync(department);
                TempData["Success"] = "Thêm chuyên khoa thành công!";
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var department = await _departmentRepo.GetByIdAsync(id);
            if (department == null) return NotFound();

            var model = new DepartmentViewModel
            {
                Id = department.Id,
                Name = department.Name,
                Description = department.Description,
                ImageUrl = department.ImageUrl,
                IsActive = department.IsActive
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, DepartmentViewModel model)
        {
            if (id != model.Id) return NotFound();

            if (ModelState.IsValid)
            {
                var department = await _departmentRepo.GetByIdAsync(id);
                if (department == null) return NotFound();

                department.Name = model.Name;
                department.Description = model.Description;
                department.IsActive = model.IsActive;
                department.UpdatedAt = System.DateTime.Now;

                // Handle New Image Upload
                if (model.ImageFile != null)
                {
                    // Optionally delete old image if required:
                    // _fileUploadService.DeleteFile(department.ImageUrl);
                    
                    department.ImageUrl = await _fileUploadService.UploadFileAsync(model.ImageFile, "departments");
                }

                await _departmentRepo.UpdateAsync(department);
                TempData["Success"] = "Cập nhật chuyên khoa thành công!";
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var department = await _departmentRepo.GetByIdAsync(id);
            if (department == null) return NotFound();
            return View(department);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _departmentRepo.DeleteAsync(id);
            TempData["Success"] = "Đã vô hiệu hoá/Xoá chuyên khoa.";
            return RedirectToAction(nameof(Index));
        }
    }
}
