using System.Text.Json;
using Kervan.Core.ICoreServiceFolder;
using Kervan.Model.KervanDbModelFolder;
using Kervan.Model.TablesOfKervan;
using Kervan.Web.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Kervan.Web.Areas.Admin.Controllers
{
    [Area("Admin"), Authorize]
    public class ListingController : Controller
    {
        private readonly ICoreService<Listing> _serviceListing;

        private readonly KervanDbContext _kervanDb;

        public ListingController(ICoreService<Listing> serviceListing, KervanDbContext kervanDb)
        {
            _serviceListing = serviceListing;
            _kervanDb = kervanDb;
        }

        
 public IActionResult Create()
        {
            var categories = _kervanDb.Category
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.CategoryName
                }).ToList();

            var model = new ListingCategoryDto
            {
                Listing = new Listing(),
                Categories = categories
            };

            return View(model);
        }

        // POST: /Admin/Listing/Create
        [HttpPost]
        public async Task<IActionResult> Create(Listing listing, List<IFormFile> files)
        {
            if (files == null || files.Count == 0)
            {
                ModelState.AddModelError("ImageUrlsSerialized", "Lütfen en az bir resim yükleyin.");

                var categories = _kervanDb.Category
                    .Select(c => new SelectListItem
                    {
                        Value = c.Id.ToString(),
                        Text = c.CategoryName
                    }).ToList();

                var dto = new ListingCategoryDto
                {
                    Listing = listing,
                    Categories = categories
                };

                return View(dto);
            }

            listing.ImageUrls = new List<string>();

            foreach (var file in files)
            {
                if (file.Length > 0)
                {
                    var ext = Path.GetExtension(file.FileName);
                    var fileName = Guid.NewGuid() + ext;
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "ImageListing", fileName);

                    using var stream = new FileStream(filePath, FileMode.Create);
                    await file.CopyToAsync(stream);

                    listing.ImageUrls.Add(fileName);
                }
            }

            listing.ImageUrlsSerialized = JsonSerializer.Serialize(listing.ImageUrls);

            var result = await _serviceListing.CreateAsync(listing);

            if (result)
            {
                TempData["Success"] = "İlan başarıyla oluşturuldu.";
                return RedirectToAction("Create", "Listing");
            }

            TempData["Error"] = "İlan oluşturulamadı.";

            var retryCategories = _kervanDb.Category
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.CategoryName
                }).ToList();

            return View(new ListingCategoryDto { Listing = listing, Categories = retryCategories });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0)
            {
                TempData["Error"] = "İlan bulunamadı";
            }
            else
            {
                var listing = await _serviceListing.GetByIdAsync(id);
                if (listing != null)
                {
                    bool isDeleted = await _serviceListing.DeleteAsync(listing);
                    TempData["Success"] = isDeleted ? "İlan başarıyla silindi" : "İlan silinirken bir hata oluştu";
                }
                else
                {
                    TempData["Error"] = "İlan bulunamadı";
                }
            }

            return RedirectToAction("Index", "Home", new { area = "Admin" });
        }

        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            if (id == 0)
            {
                TempData["Error"] = "Geçersiz ilan ID'si.";
                return RedirectToAction("Index", "Listing", new { area = "Admin" });
            }

            var findId = await _serviceListing.GetByIdAsync(id);
            if (findId == null)
            {
                TempData["Error"] = "İlan bulunamadı.";
                return RedirectToAction("Index", "Listing", new { area = "Admin" });
            }

            return View(findId);
        }

        [HttpPost]
        public async Task<IActionResult> Update(Listing listing, List<IFormFile> files)
        {
            if (files == null || files.Count == 0)
            {
                TempData["Error"] = "Lütfen en az bir resim yükleyin.";
                return RedirectToAction("Update", "Listing", new { area = "Admin" });
            }

            var findId = await _serviceListing.GetByIdAsync(listing.Id);
            if (findId == null)
            {
                TempData["Error"] = "İlgili ilan bulunamadı.";
                return RedirectToAction("Update", "Listing", new { area = "Admin" });
            }

            var oldImages = JsonSerializer.Deserialize<List<string>>(findId.ImageUrlsSerialized) ?? new List<string>();
            foreach (var oldImg in oldImages)
            {
                var oldPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "ImageListing", oldImg);
                if (System.IO.File.Exists(oldPath))
                {
                    System.IO.File.Delete(oldPath);
                }
            }

            var newImageList = new List<string>();
            foreach (var file in files)
            {
                var ext = Path.GetExtension(file.FileName);
                var newFileName = Guid.NewGuid() + ext;
                var newFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "ImageListing", newFileName);

                using var stream = new FileStream(newFilePath, FileMode.Create);
                await file.CopyToAsync(stream);

                newImageList.Add(newFileName);
            }

            findId.ImageUrlsSerialized = JsonSerializer.Serialize(newImageList);

            var updateResult = await _serviceListing.UpdateAsync(findId);

            if (!updateResult)
            {
                TempData["Error"] = "İlan güncellenirken hata oluştu.";
                return RedirectToAction("Update", "Listing", new { area = "Admin" });
            }

            TempData["Success"] = "İlan başarıyla güncellendi.";
            return RedirectToAction("Index", "Home", new { area = "Admin" });
        }
    }
}
