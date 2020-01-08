using System;
using System.Collections.Generic;
using System.Text;
using Core.Data.Entities;

namespace Core.Context {
    public class ApplicationInitializer {
        private readonly ApplicationContext context;

        public static ApplicationInitializer Initialize(ApplicationContext context) {
            return new ApplicationInitializer(context);
        }

        public ApplicationInitializer(ApplicationContext context) {

            context.SaveChanges();
        }

        private void CompanyInitializer() {
            var JSON = System.IO.File.ReadAllText("/Db/companies.json");
       //     var companyList = JsonConvert

            context.Companies.AddRange(new CompanyEntity[]{
                new CompanyEntity(){ AccountNumber="", Name = "", Description= "", PhoneNumber = "", Address = new CompanyAddressEntity() { Address = "", City = "", State="", ZipCode="", Country="" }  }
                    
            
            });
        }
    }
}
