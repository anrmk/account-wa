using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

            CompanyInitializer();
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
    }
}
