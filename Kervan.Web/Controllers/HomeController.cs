using System.Text.Json;
using Kervan.Core.ICoreServiceFolder;
using Kervan.Model.TablesOfKervan;
using Kervan.Web.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace Kervan.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ICoreService<Listing> _serviceListing;
        private readonly ICoreService<Image> _serviceImage;

        public HomeController(ICoreService<Listing> serviceListing, ICoreService<Image> serviceImage)
        {
            _serviceListing = serviceListing;
            _serviceImage = serviceImage;
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

            var images = _serviceImage.GetAll().ToList();

            var dto = new HomeIndexDto
            {
                Listings = listings,
                Images = images
            };

            return View(dto);
        }
    }
}
