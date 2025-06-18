using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kervan.Core.CoreServiceFolder;

namespace Kervan.Model.TablesOfKervan
{
    public class Image : CoreService
    {
        public string Name { get; set; } = string.Empty;
        public string Image1 { get; set; } = string.Empty;
        public string Image2 { get; set; } = string.Empty;
        public string Image3 { get; set; } = string.Empty;

    }
}
