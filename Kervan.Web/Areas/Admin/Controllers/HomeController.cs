using System.Text.Json;
using Kervan.Core.ICoreServiceFolder;
using Kervan.Model.TablesOfKervan;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kervan.Web.Areas.Admin.Controllers
{
    [Area("Admin"),Authorize]
    public class HomeController : Controller
    {
        private readonly ICoreService<Listing> _serviceListing;
        public HomeController(ICoreService<Listing> serviceListing)
        {
            _serviceListing = serviceListing;
        }
        public IActionResult Index()
            {
            var listings = _serviceListing.GetAll().ToList();   

            foreach (var listing in listings)
            {
                if (!string.IsNullOrEmpty(listing.ImageUrlsSerialized))
                {
                    try
                    {
                        listing.ImageUrls = JsonSerializer.Deserialize<List<string>>(listing.ImageUrlsSerialized);
                    }
                    catch
                    {
                        listing.ImageUrls = new List<string>();
                    }
                }
                else
                {
                    listing.ImageUrls = new List<string>();
                }
            }

            return View(listings);
        }

    }
}
