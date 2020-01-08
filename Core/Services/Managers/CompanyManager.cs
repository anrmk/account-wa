using System.Threading.Tasks;
using Core.Context;
using Core.Data.Entities;
using Core.Services.Base;


namespace Core.Services.Managers {
    public interface ICompanyManager: IEntityManager<CompanyEntity> {
        //Task<CompanyEntity> FindByCodeAsync(string code);
    }

    public class CompanyManager: AsyncEntityManager<CompanyEntity>, ICompanyManager {
        public CompanyManager(IApplicationContext context) : base(context) { }
    }

}
