using System;
using System.Collections.Generic;
using System.Linq;

using Core.Data.Entities;
using Core.Extension;

using Newtonsoft.Json;

namespace Core.Context {
    public class ApplicationInitializer {
        private readonly ApplicationContext _context;

        public static ApplicationInitializer Initialize(ApplicationContext context) {
            return new ApplicationInitializer(context);
        }

        public ApplicationInitializer(ApplicationContext context) {
            _context = context;
            string rootPath = System.IO.Directory.GetCurrentDirectory();


            //CustomerInitializer($"{rootPath}\\Db\\mactive_customers_oct.json");

            //InvoceInitializerDraft($"{rootPath}\\Db\\invoice_mactive_dec_2019.json");

            //PaymentInitializerDraft($"{rootPath}\\Db\\invoice_mactive_nov_2019.json", 
            //    $"{rootPath}\\Db\\invoice_mactive_dec_2019.json", new DateTime(2019, 12, 1), new DateTime(2019, 12, 31));

            //_context.SaveChanges();
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
        private void CustomerInitializer(string fileUrl) {
            var customers = _context.Customers.ToList();
            var customersIds = customers.Select(x => x.AccountNumber);

            var JSON = System.IO.File.ReadAllText(fileUrl);
            var customerList = JsonConvert.DeserializeObject<List<CustomerEntity>>(JSON);

            var diffCustomers = customerList.Where(x => !customersIds.Contains(x.AccountNumber));
            _context.Customers.AddRange(diffCustomers);
            //_context.SaveChanges();

            var activities = customerList.Select(x => new CustomerActivityEntity() {
                CreatedDate = x.CreatedDate,
                CustomerId = x.Id
            });
            _context.CustomerActivities.AddRange(activities);
        }

        private void InvoceInitializerDraft(string fileUrl) {
            var customers = _context.Invoices.ToList();

            var JSON = System.IO.File.ReadAllText(fileUrl);
            var invoiceList = JsonConvert.DeserializeObject<List<InvoiceEntity>>(JSON);
            var diffInvoices = 0;
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
                            diffInvoices++;
                            Console.WriteLine($"Invoice {invoice.No} already existed: Subtotal: {invoice.Subtotal}");
                        }
                    } else {
                        Console.WriteLine("NOT FOUND: " + i.CustomerAccountNumber);
                    }
                }
            }
            var test = diffInvoices;
        }

        private void PaymentInitializerDraft(string fileUrlFrom, string fileUrlTo, DateTime startDate, DateTime endDate) {
            var fromJson = System.IO.File.ReadAllText(fileUrlFrom);
            var toJson = System.IO.File.ReadAllText(fileUrlTo);

            var invoiceListFrom = JsonConvert.DeserializeObject<List<InvoiceEntity>>(fromJson);
            var invoiceListTo = JsonConvert.DeserializeObject<List<InvoiceEntity>>(toJson);

            //find records in two list with diff SUBTOTAL and the same CUSTOMERACCOUNTNUMBER
            //exclude records which the same PRICE and CUSTOMERACCOUNTNUMBER in two lists
            var diffInvoiceList = invoiceListFrom.Where(x => invoiceListTo.Any(p => x.CustomerAccountNumber == p.CustomerAccountNumber && x.Subtotal != p.Subtotal)).ToList();
            //find record in FIRST LIST witch no in th SECOND LIST
            var noInSecondInvoiceList = invoiceListFrom.Where(x => !invoiceListTo.Any(p => p.CustomerAccountNumber == x.CustomerAccountNumber)).ToList();

            diffInvoiceList.AddRange(noInSecondInvoiceList);
            //var test1 = diffInvoiceList.Any(x => x.CustomerAccountNumber.Equals("169492")); //есть в двух списках, с разной ценой
            //var test21 = noInSecondList.Any(x => x.CustomerAccountNumber.Equals("169631")); //есть в двух списках, с одной ценой - их не трогать
            //var test3 = diffInvoiceList.Any(x => x.CustomerAccountNumber.Equals("169385")); //нет во втором списке

            foreach(var i in diffInvoiceList) {
                var customer = _context.Customers.Where(x => x.AccountNumber.Equals(i.CustomerAccountNumber)).FirstOrDefault();
                if(customer != null) {
                    var invoice = _context.Invoices.Where(x => x.CustomerId.Equals(customer.Id) && x.Subtotal.Equals(i.Subtotal)).FirstOrDefault();
                    if(invoice != null) {
                        var newDate = RandomDateExtansion.GetRandomDateTime(startDate, endDate);
                        //startDate.AddHours(new Random(Convert.ToInt32(DateTime.Now.Ticks / int.MaxValue)).Next(0, (int)(endDate - startDate).TotalHours));

                        var payment = new PaymentEntity() {
                            Ref = "Inv_" + invoice.No,
                            InvoiceId = invoice.Id,
                            IsDraft = true,
                            Amount = invoice.Subtotal,
                            CreatedDate = DateTime.Now,
                            Date = newDate,
                            UpdatedDate = DateTime.Now,
                        };

                        _context.Payments.Add(payment);
                    }
                }
            }
        }
        #endregion
    }
}
