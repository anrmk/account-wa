using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Context;
using Core.Data.Entities;
using Core.Services.Base;

using Microsoft.EntityFrameworkCore;

namespace Core.Services.Managers {
    public interface ICustomerCreditUtilizedSettingsManager: IEntityManager<CustomerCreditUtilizedSettingsEntity> {
        Task<List<CustomerCreditUtilizedSettingsEntity>> FindByCompany(long companyId);
        Task<CustomerCreditUtilizedSettingsEntity> FindInclude(long companyId, DateTime date);
    }

    public class CustomerCreditUtilizedSettingsManager: AsyncEntityManager<CustomerCreditUtilizedSettingsEntity>, ICustomerCreditUtilizedSettingsManager {
        public CustomerCreditUtilizedSettingsManager(IApplicationContext context) : base(context) { }
        
        public async Task<CustomerCreditUtilizedSettingsEntity> FindInclude(long companyId, DateTime date) {
            return await DbSet.Where(x => x.CompanyId == companyId && x.Date == date).FirstOrDefaultAsync();
        }

        public async Task<List<CustomerCreditUtilizedSettingsEntity>> FindByCompany(long companyId) {
            return await DbSet.Where(x => x.CompanyId == companyId).ToListAsync();
        }
    }
}
