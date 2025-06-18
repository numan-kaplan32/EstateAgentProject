using System.IO;
using System.Threading.Tasks;
using Kervan.Core.ICoreServiceFolder;
using Kervan.Model.TablesOfKervan;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kervan.Web.Areas.Admin.Controllers
{
    [Area("Admin"), Authorize]
    public class ImageController : Controller
    {
        private readonly ICoreService<Image> _serviceImage;
        private readonly string _imageFolder;

        public ImageController(ICoreService<Image> service)
        {
            _serviceImage = service;
            _imageFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "FolderImage");
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Image image, IFormFile pic1, IFormFile pic2, IFormFile pic3)
        {
            try
            {
                if (image == null)
                {
                    TempData["Error"] = "Form boş gönderilemez.";
                    return View();
                }

                if (pic1 != null) image.Image1 = await SaveImageAsync(pic1);
                if (pic2 != null) image.Image2 = await SaveImageAsync(pic2);
                if (pic3 != null) image.Image3 = await SaveImageAsync(pic3);

                await _serviceImage.CreateAsync(image);
                TempData["Success"] = "İşlem başarılı.";
                return RedirectToAction("List", "Image", new {area="Admin"});
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Hata oluştu: " + ex.Message;
                return View();
            }
        }
        [HttpPost] 
        public async Task<IActionResult> Delete(int id)
        {
            if (id == 0)
            {
                TempData["Error"] = "Geçersiz ID.";
                return RedirectToAction("List");
            }

            var image = await _serviceImage.GetByIdAsync(id);
            if (image == null)
            {
                TempData["Error"] = "Kayıt bulunamadı.";
                return RedirectToAction("List");
            }
             
            DeleteFileIfExists(image.Image1);
            DeleteFileIfExists(image.Image2);
            DeleteFileIfExists(image.Image3);

            await _serviceImage.DeleteAsync(image);

            TempData["Success"] = "Silme işlemi başarılı.";
            return RedirectToAction("List", "Image", new { area = "Admin" });
        }


        public async Task<IActionResult> Update(int id)
        {
            if (id == 0) return NotFound();

            var image = await _serviceImage.GetByIdAsync(id);
            if (image == null) return NotFound();

            return View(image);
        }

        [HttpPost]
        public async Task<IActionResult> Update(Image updatedImage, IFormFile pic1, IFormFile pic2, IFormFile pic3)
        {
            try
            {
                var existingImage = await _serviceImage.GetByIdAsync(updatedImage.Id);
                if (existingImage == null) return NotFound();

                if (pic1 != null)
                {
                    DeleteFileIfExists(existingImage.Image1);
                    existingImage.Image1 = await SaveImageAsync(pic1);
                }

                if (pic2 != null)
                {
                    DeleteFileIfExists(existingImage.Image2);
                    existingImage.Image2 = await SaveImageAsync(pic2);
                }

                if (pic3 != null)
                {
                    DeleteFileIfExists(existingImage.Image3);
                    existingImage.Image3 = await SaveImageAsync(pic3);
                }

                existingImage.Name = updatedImage.Name;

                await _serviceImage.UpdateAsync(existingImage);

                TempData["Success"] = "Güncelleme başarılı.";
                return RedirectToAction("List", "Image", new { area = "Admin" });
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Hata oluştu: " + ex.Message;
                return View(updatedImage);
            }
        }

        private void DeleteFileIfExists(string fileName)
        {
            if (string.IsNullOrEmpty(fileName)) return;

            var fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "ImageHome", fileName);
            if (System.IO.File.Exists(fullPath))
            {
                System.IO.File.Delete(fullPath);
            }
        }

        private async Task<string> SaveImageAsync(IFormFile file)
        {
            if (!Directory.Exists(_imageFolder))
            {
                Directory.CreateDirectory(_imageFolder);
            }

            var extension = Path.GetExtension(file.FileName);
            var fileName = $"{Guid.NewGuid()}{extension}";
            var fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot","ImageHome",fileName);

            using var stream = new FileStream(fullPath, FileMode.Create);
            await file.CopyToAsync(stream);

            return fileName;
        }
        public async Task<IActionResult> List()
        {
            var allImages = await _serviceImage.GetAllAsync();  
            return View(allImages);
        }

    }
}
