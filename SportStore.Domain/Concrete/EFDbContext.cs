using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SportStore.Domain.Entities;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration;
namespace SportStore.Domain.Concrete
{
    public class EFDbContext : DbContext
    {
        public EFDbContext() : base("EFDbContext") { }
        public DbSet<Product> Products { get; set; }

        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    base.OnConfiguring(optionsBuilder);
        //}
    }
}
