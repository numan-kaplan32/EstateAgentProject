using System.Security.Claims;
using System.Threading.Tasks;
using Kervan.Core.ICoreServiceFolder;
using Kervan.Model.KervanDbModelFolder;
using Kervan.Model.TablesOfKervan;
using Kervan.Web.Helpers.aboutHash;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace Kervan.Web.Areas.Admin.Controllers
{
    [Area("Admin"), Authorize]
    public class AdministratorController : Controller
    {
        private readonly ICoreService<Administrator> _serviceAdmin;
        private readonly KervanDbContext _kervanDb;

        public AdministratorController(ICoreService<Administrator> serviceAdmin, KervanDbContext kervanDb)
        {
            _serviceAdmin = serviceAdmin;
            _kervanDb = kervanDb;
        }
        //Profile 
        public IActionResult Profile()
        {
            var dataAll = _serviceAdmin.GetAll();
            return View(dataAll);
        }

        //Create x2
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(Administrator admin, IFormFile file)
        {
            if (User.FindFirst(ClaimTypes.Role)?.Value == "True")
            {
                if (admin == null)
                {
                    TempData["Error"] = "Yönetici bilgisi boş.";
                    return RedirectToAction("Create");
                }

                if (file != null)
                {
                    string ext = Path.GetExtension(file.FileName);
                    string fileName = Guid.NewGuid().ToString() + ext;
                    string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "AdminImage", fileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(fileStream);
                    }

                    admin.Image = fileName;
                }
                admin.Password = HasherByBcrypt.HashBcrypt(admin.Password);

                var result = await _serviceAdmin.CreateAsync(admin);

                if (result)
                {
                    TempData["Success"] = "Yönetici başarıyla oluşturuldu.";
                    return RedirectToAction("List");
                }

                TempData["Error"] = "Yönetici oluşturulamadı.";
                return RedirectToAction("Create");
            }
            return View();
        }



        //Delete
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            if (User.FindFirst(ClaimTypes.Role)?.Value == "True")
            {
                if (id <= 0)
                {
                    TempData["Error"] = "Erişime izniniz yok veya id bulunamadı";
                    return RedirectToAction("List", "Administrator", new { area = "Admin" });
                }
                var findId = await _serviceAdmin.GetByIdAsync(id);
                if (findId == null)
                {
                    TempData["Error"] = "Erişime izniniz yok veya id bulunamadı";
                    return RedirectToAction("List", "Administrator", new { area = "Admin" });
                }
                var deleteAdmin = await _serviceAdmin.DeleteAsync(findId);
                TempData["Success"] = "Başarıyla silindi";
                return RedirectToAction("List", "Administrator", new { area = "Admin" });
            }
            return RedirectToAction("Index", "Home");
        }

        //Update x2 
        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            var admin = await _serviceAdmin.GetByIdAsync(id);
            if (admin == null)
            {
                TempData["Error"] = "Yönetici Id bulunamadı.";
                return RedirectToAction("Profile", "Administrator", new { area = "Admin" });
            }
            return View(admin);
        }

        [HttpPost]
        public async Task<IActionResult> Update(Administrator administrator, IFormFile file)
        {
         if(User.FindFirst(ClaimTypes.Role)?.Value == "True")
{
            if (administrator == null)
            {
                TempData["Error"] = "Veriler eksik.";
                return RedirectToAction("List");
            }

            var existingAdmin = await _serviceAdmin.GetByIdAsync(administrator.Id);
            if (existingAdmin == null)
            {
                TempData["Error"] = "Yönetici bulunamadı.";
                return RedirectToAction("List");
            }


            existingAdmin.Name = administrator.Name;
            existingAdmin.Surname = administrator.Surname;
            existingAdmin.Email = administrator.Email;
            existingAdmin.Phone = administrator.Phone;
            existingAdmin.Resume = administrator.Resume;
            existingAdmin.Gender = administrator.Gender;
            existingAdmin.City = administrator.City;
            existingAdmin.District = administrator.District;
            existingAdmin.Position = administrator.Position;
            existingAdmin.isSuperAdmin = administrator.isSuperAdmin;


            if (file != null)
            {
                if (!string.IsNullOrEmpty(existingAdmin.Image))
                {
                    var oldPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "AdminImage", existingAdmin.Image);
                    if (System.IO.File.Exists(oldPath))
                        System.IO.File.Delete(oldPath);
                }

                string extension = Path.GetExtension(file.FileName);
                string fileName = Guid.NewGuid().ToString() + extension;
                string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "AdminImage", fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                existingAdmin.Image = fileName;
            }

            await _serviceAdmin.UpdateAsync(existingAdmin);
            TempData["Success"] = "Yönetici başarıyla güncellendi.";

                return RedirectToAction("List");
            }
            return View();
        }

        //List
        public async Task<IActionResult> List()
        {
            var dataAdmin = await _serviceAdmin.GetAllAsync();
            return View(dataAdmin);
        }

        //Login x2
        [HttpGet, AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost, AllowAnonymous]
        public async Task<IActionResult> Login(Administrator administrator)
        {
            if (administrator == null)
            {
                TempData["Empty"] = "Boş giriş yapılamaz.";
                return View();
            }

            var isAdmin = await _kervanDb.Administrators
                .FirstOrDefaultAsync(a => a.Email == administrator.Email);

            if (isAdmin == null)
            {
                TempData["Empty"] = "Böyle bir admin bulunamadı.";
                return View();
            }

            bool isPasswordValid = HasherByBcrypt.Verify(administrator.Password, isAdmin.Password);

            if (!isPasswordValid)
            {
                TempData["Empty"] = "Şifre hatalı. Lütfen tekrar deneyin!";
                return View();
            }

            var claimsValue = new List<Claim>
        {
            new Claim("AdminId", isAdmin.Id.ToString()),
            new Claim(ClaimTypes.Name, isAdmin.Name),
            new Claim(ClaimTypes.Surname, isAdmin.Surname),
            new Claim(ClaimTypes.Email, isAdmin.Email),
            new Claim("AdminImage",isAdmin.Image),
            new Claim("AdminPhone", isAdmin.Phone),
            new Claim("AdminPosition", isAdmin.Position),
            new Claim(ClaimTypes.Role, isAdmin.isSuperAdmin.ToString())
        };

            var identity = new ClaimsIdentity(claimsValue, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            return RedirectToAction("Profile", "Administrator");
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Administrator");
        }

    }
}
