using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kervan.Core.CoreServiceFolder;

namespace Kervan.Model.TablesOfKervan
{
   public class Category : CoreService
    {
        public string CategoryName { get; set; } = string.Empty;
  
        public string Image {  get; set; } = string.Empty;
        public ICollection<Listing> listings { get; set; } = new List<Listing>();
    }
}
