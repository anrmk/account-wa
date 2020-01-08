using System;
using System.Collections.Generic;
using System.Text;
using Core.Context;
using Core.Data.Entities;
using Core.Services.Base;

namespace Core.Services.Managers {
    public interface ICustomerManager: IEntityManager<CustomerEntity> {

    }

    public class CustomerManager: AsyncEntityManager<CustomerEntity>, ICustomerManager {
        public CustomerManager(IApplicationContext context) : base(context) { }
    }
}
