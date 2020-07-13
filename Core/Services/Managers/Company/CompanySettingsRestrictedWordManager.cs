using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Core.Context;
using Core.Data.Entities;
using Core.Services.Base;

using Microsoft.EntityFrameworkCore;

namespace Core.Services.Managers {
    public interface ICompanySettingsRestrictedWordManager: IEntityManager<CompanyRestrictedWordEntity> {
        Task<List<CompanyRestrictedWordEntity>> FindByCompanyId(long id);
        Task<List<CompanyRestrictedWordEntity>> FindByWordId(long id);
        Task<List<CompanyRestrictedWordEntity>> FindByWordIds(long[] ids);
    }

    public class CompanySettingsRestrictedWordManager: AsyncEntityManager<CompanyRestrictedWordEntity>, ICompanySettingsRestrictedWordManager {
        public CompanySettingsRestrictedWordManager(IApplicationContext context) : base(context) { }

        public async Task<List<CompanyRestrictedWordEntity>> FindByCompanyId(long id) {
            return await DbSet
                .Include(x => x.RestrictedWord)
                .Include(x => x.Company)
                .Where(x => x.CompanyId == id).ToListAsync();
        }

        public async Task<List<CompanyRestrictedWordEntity>> FindByWordId(long id) {
            //return await DbSet
            //    .Include(x => x.RestrictedWord)
            //    .Include(x => x.Company)
            //    .Where(x => x.RestrictedWordId == id).ToListAsync();
            return await FindByWordIds(new long[] { id });
        }

        public async Task<List<CompanyRestrictedWordEntity>> FindByWordIds(long[] ids) {
            return await DbSet
                .Include(x => x.RestrictedWord)
                .Include(x => x.Company)
                .Where(x => ids.Contains(x.RestrictedWordId ?? 0)).ToListAsync();
        }
    }
}
