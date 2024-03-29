﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Core.Context;
using Core.Data.Entities;
using Core.Services.Base;

using Microsoft.EntityFrameworkCore;

namespace Core.Services.Managers {
    public interface ICompanyManager: IEntityManager<CompanyEntity> {
        Task<CompanyEntity> FindInclude(long id);
        Task<List<CompanyEntity>> AllInclude();
        //Task<CompanyEntity> FindByCodeAsync(string code);
    }

    public class CompanyManager: AsyncEntityManager<CompanyEntity>, ICompanyManager {
        public CompanyManager(IApplicationContext context) : base(context) { }

        public async Task<CompanyEntity> FindInclude(long id) {
            return await DbSet
                .Include(x => x.Address)
                .Include(x => x.Settings)
                .Include(x => x.Customers)
                .Include(x => x.SummaryRange)
                .Where(x => x.Id == id)
                .FirstOrDefaultAsync();
        }

        public async Task<List<CompanyEntity>> AllInclude() {
            return await DbSet
                .Include(x => x.Address)
                //.Include(x => x.Customers)
                .ToListAsync();
        }
    }
}
