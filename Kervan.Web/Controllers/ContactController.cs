using System.Threading.Tasks;
using Kervan.Core.ICoreServiceFolder;
using Kervan.Model.TablesOfKervan;
using Microsoft.AspNetCore.Mvc;

namespace Kervan.Web.Controllers
{
    public class ContactController : Controller
    {
        private readonly ICoreService<Message> _coreMessage;
        public ContactController(ICoreService<Message> coreMessage)
        {
            _coreMessage = coreMessage;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> MessageCreate(Message message)
        {
            try
            {
                if (message != null)
                {
                    var createMessage = await _coreMessage.CreateAsync(message);
                    TempData["Success"] = "Mesajınız başarıyla gönderildi.";
                    return RedirectToAction("Index", "Contact");
                }
                TempData["Empty"] = "Mesaj boş gönderilemez.";
                return RedirectToAction("Index", "Contact");
            }
            catch (Exception ex)
            {
                TempData["Empty"] = ex.Message;
                return RedirectToAction("Index", "Contact");
            }
        }

        public async Task<IActionResult> MessageBox()
        {
            var getMessageAll = await _coreMessage.GetAllAsync();
            return View(getMessageAll); 
        }
    }
}
