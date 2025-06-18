using Kervan.Model.TablesOfKervan;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Kervan.Web.Dtos
{
    public class ListingCategoryDto
    {
        public Listing Listing { get; set; }
        public List<SelectListItem> Categories { get; set; }
    }
}
