using System.Threading.Tasks;
using Kervan.Core.ICoreServiceFolder;
using Kervan.Model.TablesOfKervan;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kervan.Web.Areas.Admin.Controllers
{
    [Area("Admin"), Authorize]
    public class CategoryController : Controller
    {
        private readonly ICoreService<Category> _coreService;
        public CategoryController(ICoreService<Category> coreService)
        {
            _coreService = coreService;
        }
        public async Task<IActionResult> CreateList()
        {
            var dataCategory = await _coreService.GetAllAsync();
            return View(dataCategory);
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(IFormFile file, string Name)
        {

            if (!string.IsNullOrEmpty(Name) && file != null)
            {
                string ext = Path.GetExtension(file.FileName);
                string fileName = Guid.NewGuid() + ext;
                string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "CategoryImage", fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                var category = new Category
                {
                    CategoryName = Name,
                    Image = fileName
                };

                await _coreService.CreateAsync(category);
                TempData["Success"] = "Kategori başarıyla oluşturuldu!";
                return RedirectToAction("Create");
            }

            TempData["Error"] = "Kategori adı veya görseli eksik.";
            return View();
        }
        public async Task<IActionResult> Update(int id)
        {
            if (id != 0)
            {
                var findId = await _coreService.GetByIdAsync(id);
                if (findId != null)
                {
                    return View(findId);
                }
            }
            TempData["Error"] = "Category bulunamadı.";
            return RedirectToAction("CategoryList", "Category", new { area = "Admin" });
        }
        [HttpPost]
        public async Task<IActionResult> Update(Category category, IFormFile file)
        {
            try
            {
                var findId = await _coreService.GetByIdAsync(category.Id);
                if (findId == null)
                {
                    TempData["Error"] = "Kategori bulunamadı.";
                    return RedirectToAction("CategoryList", "Category", new { area = "Admin" });
                }

                findId.CategoryName = category.CategoryName;

                if (file != null)
                {
                    if (!string.IsNullOrEmpty(findId.Image))
                    {
                        var oldPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "CategoryImage", findId.Image);
                        if (System.IO.File.Exists(oldPath))
                            System.IO.File.Delete(oldPath);
                    }

                    string ext = Path.GetExtension(file.FileName);
                    string fileName = Guid.NewGuid().ToString() + ext;
                    string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "CategoryImage", fileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(fileStream);
                    }

                    findId.Image = fileName;
                }

                await _coreService.UpdateAsync(findId);
                TempData["Success"] = "Kategori başarıyla güncellendi.";
                return RedirectToAction("CreateList", "Category", new { area = "Admin" });
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Bir hata oluştu: {ex.Message}";
                return View(category);
            }
        }

    }
}
