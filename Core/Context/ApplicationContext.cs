﻿using System;
using System.Linq;
using System.Threading.Tasks;

using Core.Data.Entities;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Configuration;


using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Core.Context {
    public interface IApplicationContext {
        Database ApplicationDatabase { get; }
        DbSet<T> Set<T>() where T : class;
        EntityEntry<T> Entry<T>(T entity) where T : class;
        Task<int> SaveChangesAsync();
        int SaveChanges();
    }

    public class ApplicationContext: IdentityDbContext<ApplicationUserEntity>, IApplicationContext {
        private readonly IConfiguration _configuration;
        private readonly string _contentRootPath = "";

        #region DbSet
        public DbSet<CompanyEntity> Companies { get; set; }
        public DbSet<CompanyAddressEntity> CompanyAdresses { get; set; }
        public DbSet<CompanySummaryRangeEntity> CompanySummaryRanges { get; set; }

        public DbSet<CustomerEntity> Customers { get; set; }
        public DbSet<CustomerAddressEntity> CustomerAdresses { get; set; }
        public DbSet<CustomerActivityEntity> CustomerActivities { get; set; }
        //public DbSet<CustomerBulkEntity> CustomersBulk { get; set; }

        public DbSet<InvoiceEntity> Invoices { get; set; }
        public DbSet<PaymentEntity> Payments { get; set; }
        #endregion

        public Database ApplicationDatabase { get; private set; }

        public ApplicationContext(DbContextOptions<ApplicationContext> options, IConfiguration configuration, IHostingEnvironment environment) : base(options) {
            _configuration = configuration;
            _contentRootPath = environment.ContentRootPath;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options) => options.UseSqlServer(_configuration.GetConnectionString("DefaultConnection"));

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            base.OnModelCreating(modelBuilder);
        }

        public async Task<int> SaveChangesAsync() {
            var modifiedEntries = ChangeTracker.Entries()
              .Where(x => x.Entity is IAuditableEntity
                  && (x.State == EntityState.Added || x.State == EntityState.Modified));


            foreach(var entry in modifiedEntries) {
                IAuditableEntity entity = entry.Entity as IAuditableEntity;
                if(entity != null) {
                    string identityName = ""; // Thread.CurrentPrincipal.Identity.Name;
                    DateTime now = DateTime.UtcNow;

                    if(entry.State == EntityState.Added) {
                        entity.CreatedBy = identityName;
                        entity.CreatedDate = now;
                    } else {
                        Entry(entity).Property(x => x.CreatedBy).IsModified = false;
                        Entry(entity).Property(x => x.CreatedDate).IsModified = false;
                    }
                    entity.UpdatedBy = identityName;
                    entity.UpdatedDate = now;
                }
            }
            bool saveFailed;
            do {
                saveFailed = false;
                try {
                    return await base.SaveChangesAsync();
                } catch(DbUpdateConcurrencyException e) {
                    saveFailed = true;
                    Console.WriteLine(e.Message);
                    return -1001;
                } catch(DbUpdateException e) {
                    Console.WriteLine(e.Message);
                    saveFailed = true;
                    return -1002;
                } catch(Exception e) {
                    Console.WriteLine(e.Message);
                    saveFailed = true;
                    return -1003;
                }
            } while(saveFailed);
        }
    }
}
