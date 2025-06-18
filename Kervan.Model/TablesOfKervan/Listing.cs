using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Kervan.Core.CoreServiceFolder;

namespace Kervan.Model.TablesOfKervan
{
    public class Listing : CoreService
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; } = DateTime.Now;


        public decimal? Price { get; set; }
        public string Location { get; set; } = string.Empty;
        public string PropertyType { get; set; } = string.Empty;
        public int RoomCount { get; set; }
        public int BathroomCount { get; set; }
        public double Area { get; set; }
        public int? Floor { get; set; }
        public bool IsFurnished { get; set; } = false;
        public string HeatingType { get; set; } = string.Empty;
        public int? YearBuilt { get; set; }
        public bool IsActive { get; set; } = true;

        public string ContactName { get; set; } = string.Empty;
        public string ContactPhone { get; set; } = string.Empty;
        public string ContactEmail { get; set; } = string.Empty;

        [NotMapped]
        public List<string> ImageUrls { get; set; } = new();

        public string ImageUrlsSerialized
        {
            get => JsonSerializer.Serialize(ImageUrls);
            set => ImageUrls = string.IsNullOrEmpty(value)
                ? new List<string>()
                : JsonSerializer.Deserialize<List<string>>(value)!;
        }

        public int CategoryId { get; set; }
        public Category Category { get; set; }

    }

}
