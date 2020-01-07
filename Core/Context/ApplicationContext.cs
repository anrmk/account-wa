using Core.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Core.Context {
    public class ApplicationContext: DbContext {
        //public DbSet<Period> Periods { get; set; }
        //public DbSet<Payment> Payments { get; set; }
        //public DbSet<Invoice> Invoices { get; set; }
        //public DbSet<Customer> Customers { get; set; }
        public DbSet<CompanyEntity> Companies { get; set; }
        public DbSet<CompanyAddressEntity> CompanyAdresses { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options) => options.UseSqlite("Data Source=application.db");
    }
}
