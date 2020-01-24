using System;
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
            //CustomerInitializerDec();
            //InvoceInitializerDec();
            //CustomerInitializer();
            //InvoceInitializer();
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

            var JSON = System.IO.File.ReadAllText($"{rootPath}\\Db\\customer_bc_bundt.json");
            var customerList = JsonConvert.DeserializeObject<List<CustomerEntity>>(JSON);
            _context.Customers.AddRange(customerList);
        }


        private void InvoceInitializer() {
            if(_context.Invoices.Any()) {
                return;
            }

            string rootPath = System.IO.Directory.GetCurrentDirectory();

            var JSON = System.IO.File.ReadAllText($"{rootPath}\\Db\\invoice_bc_bundt.json");
            var invoiceList = JsonConvert.DeserializeObject<List<InvoiceEntity>>(JSON);
            foreach(var i in invoiceList) {
                var customer = _context.Customers.Where(x => x.AccountNumber.Equals(i.CustomerAccountNumber)).FirstOrDefault();
                if(customer != null) {
                    i.CustomerId = customer.Id;
                    _context.Invoices.Add(i);

                } else {
                    Console.WriteLine("NOT FOUND: " + i.CustomerAccountNumber);
                }
            }
            //_context.Customers.AddRange(customerList);
        }

        #region TEST
        private void CustomerInitializerDec() {
            var customers = _context.Customers.ToList();
            var customersIds = customers.Select(x => x.AccountNumber);

            string rootPath = System.IO.Directory.GetCurrentDirectory();
            var JSON = System.IO.File.ReadAllText($"{rootPath}\\Db\\customer_bc_bundt_dec.json");
            var customerList = JsonConvert.DeserializeObject<List<CustomerEntity>>(JSON);

            var diffCustomers = customerList.Where(x => !customersIds.Contains(x.AccountNumber));
            _context.Customers.AddRange(diffCustomers);
            _context.SaveChanges();

            var activities = diffCustomers.Select(x => new CustomerActivityEntity() {
                CreatedDate = new DateTime(2019, 12, 1),
                CustomerId = x.Id
            });
            _context.CustomerActivities.AddRange(activities);
        }

        private void InvoceInitializerDec() {
            var customers = _context.Invoices.ToList();

            string rootPath = System.IO.Directory.GetCurrentDirectory();

            var JSON = System.IO.File.ReadAllText($"{rootPath}\\Db\\invoice_bc_bundt_dec.json");
            var invoiceList = JsonConvert.DeserializeObject<List<InvoiceEntity>>(JSON);
            foreach(var i in invoiceList) {
                if(i.Subtotal != 0) {
                    var customer = _context.Customers.Where(x => x.AccountNumber.Equals(i.CustomerAccountNumber)).FirstOrDefault();

                    if(customer != null) {
                        var customerId = customer.Id;
                        var invoice = _context.Invoices.Where(x => x.CustomerId.Equals(customerId) && x.Subtotal.Equals(i.Subtotal)).FirstOrDefault();
                        if(invoice == null) {

                            i.CustomerId = customerId;
                            _context.Invoices.Add(i);
                        } else {
                            Console.WriteLine($"Invoice {invoice.No}");
                        }

                    } else {
                        Console.WriteLine("NOT FOUND: " + i.CustomerAccountNumber);
                    }
                }
            }
        }

        #endregion

    }
}
