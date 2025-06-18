using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Kervan.Model.TablesOfKervan;
using Microsoft.EntityFrameworkCore;
using static System.Net.Mime.MediaTypeNames;
using Image = Kervan.Model.TablesOfKervan.Image;

namespace Kervan.Model.KervanDbModelFolder
{
   public class KervanDbContext : DbContext
    {  
        public KervanDbContext(DbContextOptions<KervanDbContext> options): base(options) { }

        public DbSet<Listing> Listings { get; set; }
        public DbSet<Administrator> Administrators { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Category> Category{ get; set; }
        public DbSet<Image>  Images { get; set; }
    }
}
