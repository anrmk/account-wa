using System;
using System.Collections.Generic;
using System.Text;
using Core.Context;
using Core.Data.Entities.Nsi;
using Core.Services.Base;

namespace Core.Services.Managers.Nsi {
    public interface ICustomerTypeManager: IEntityManager<CustomerTypeEntity> {

    }

    public class CustomerTypeManager: AsyncEntityManager<CustomerTypeEntity>, ICustomerTypeManager {
        public CustomerTypeManager(IApplicationContext context) : base(context) { }
    }
}
