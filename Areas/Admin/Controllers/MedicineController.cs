using HUTECH_Hospital.Models;
using HUTECH_Hospital.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace HUTECH_Hospital.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class MedicineController : Controller
    {
        private readonly IMedicineRepository _medicineRepository;

        public MedicineController(IMedicineRepository medicineRepository)
        {
            _medicineRepository = medicineRepository;
        }

        public async Task<IActionResult> Index(string keyword)
        {
            var medicines = await _medicineRepository.SearchAsync(keyword);
            ViewBag.Keyword = keyword;
            return View(medicines);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Medicine model)
        {
            if (ModelState.IsValid)
            {
                model.CreatedAt = System.DateTime.Now;
                await _medicineRepository.AddAsync(model);
                TempData["SuccessMessage"] = "Thêm thuốc mới thành công!";
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var medicine = await _medicineRepository.GetByIdAsync(id);
            if (medicine == null) return NotFound();
            return View(medicine);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Medicine model)
        {
            if (id != model.Id) return NotFound();

            if (ModelState.IsValid)
            {
                var existing = await _medicineRepository.GetByIdAsync(id);
                if (existing == null) return NotFound();

                existing.Name = model.Name;
                existing.Unit = model.Unit;
                existing.Description = model.Description;
                existing.Usage = model.Usage;
                existing.IsActive = model.IsActive;
                existing.UpdatedAt = System.DateTime.Now;

                await _medicineRepository.UpdateAsync(existing);
                TempData["SuccessMessage"] = "Cập nhật thông tin thuốc thành công!";
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        public async Task<IActionResult> Details(int id)
        {
            var medicine = await _medicineRepository.GetByIdAsync(id);
            if (medicine == null) return NotFound();
            return View(medicine);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var medicine = await _medicineRepository.GetByIdAsync(id);
            if (medicine == null) return NotFound();
            return View(medicine);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _medicineRepository.DeleteAsync(id);
            TempData["SuccessMessage"] = "Đã xóa thuốc thành công!";
            return RedirectToAction(nameof(Index));
        }
    }
}
