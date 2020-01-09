using System.Collections.Generic;
using System.Linq;

using Core.Data.Entities;

using Newtonsoft.Json;

namespace Core.Context {
    public class ApplicationInitializer {
        private readonly ApplicationContext _context;

        public static ApplicationInitializer Initialize(ApplicationContext context) {
            return new ApplicationInitializer(context);
        }

        public ApplicationInitializer(ApplicationContext context) {
            _context = context;

           // CompanyInitializer();
            CustomerInitializer();
            _context.SaveChanges();
        }

        private void CompanyInitializer() {
            if(_context.Companies.Any()) {
                return;
            }

            string rootPath = System.IO.Directory.GetCurrentDirectory();

            var JSON = System.IO.File.ReadAllText($"{rootPath}\\Db\\companies.json");
            var companyList = JsonConvert.DeserializeObject<List<CompanyEntity>>(JSON);
            _context.Companies.AddRange(companyList);
        }

        private void CustomerInitializer() {
            if(_context.Customers.Any()) {
                return;
            }

            string rootPath = System.IO.Directory.GetCurrentDirectory();

            var JSON = System.IO.File.ReadAllText($"{rootPath}\\Db\\customers.json");
            var customerList = JsonConvert.DeserializeObject<List<CustomerEntity>>(JSON);
            _context.Customers.AddRange(customerList);
        }
    }
}
