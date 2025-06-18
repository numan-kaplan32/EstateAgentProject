using System.Linq;
using System.Threading.Tasks;
using Kervan.Core.ICoreServiceFolder;
using Kervan.Model.KervanDbModelFolder;
using Kervan.Model.TablesOfKervan;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Kervan.Web.Controllers
{
    public class ListingUIController : Controller
    {
        private readonly ICoreService<Listing> _listingService;
        private readonly KervanDbContext _context;

        public ListingUIController(ICoreService<Listing> listingService, KervanDbContext kervanDbContext)
        {
            _listingService = listingService;
            _context = kervanDbContext;
        }

        public async Task<IActionResult> Index(string category)
        {
            IQueryable<Listing> query = _context.Listings.Include(c => c.Category);

            if (!string.IsNullOrEmpty(category))
            {
                query = query.Where(x => x.Category.CategoryName == category);
            }

            var dataListing = await query.ToListAsync();

            return View(dataListing);
        }

        [HttpPost]
        public async Task<IActionResult> Details(int id)
        {
            if (id <= 0)
                return RedirectToAction("Index", "ListingUI");

            var listing = await _context.Listings
                .Include(c => c.Category)
                .FirstOrDefaultAsync(c => c.Id == id);
                
            if (listing == null)
                return RedirectToAction("Index", "ListingUI");

            return View(listing);
        }

     
    }
}
