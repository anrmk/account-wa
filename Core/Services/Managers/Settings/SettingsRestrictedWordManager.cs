using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Core.Context;
using Core.Data.Entities;
using Core.Services.Base;

using Microsoft.EntityFrameworkCore;

namespace Core.Services.Managers {
    public interface ISettingsRestrictedWordManager: IEntityManager<SettingsRestrictedWordEntity> {
        Task<List<SettingsRestrictedWordEntity>> FindByIds(long[] ids);
    }

    public class SettingsRestrictedWordManager: AsyncEntityManager<SettingsRestrictedWordEntity>, ISettingsRestrictedWordManager {
        public SettingsRestrictedWordManager(IApplicationContext context) : base(context) { }

        public async Task<List<SettingsRestrictedWordEntity>> FindByIds(long[] ids) {
            return await DbSet
               .Where(x => ids.Contains(x.Id))
               .ToListAsync();
        }
    }
}
