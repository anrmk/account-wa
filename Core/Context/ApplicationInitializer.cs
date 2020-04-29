using System;
using System.Collections.Generic;
using System.Linq;

using Core.Data.Entities;
using Core.Extension;

using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

using Newtonsoft.Json;

namespace Core.Context {
    public class ApplicationInitializer {
        private readonly IServiceProvider _serviceProvider;

        public static ApplicationInitializer Initialize(IServiceProvider serviceProvider) {
            return new ApplicationInitializer(serviceProvider);
        }

        public ApplicationInitializer(IServiceProvider serviceProvider) {
            _serviceProvider = serviceProvider;

            RoleManager();
            ApplicationUser();

            string rootPath = System.IO.Directory.GetCurrentDirectory();

            string company = "western";
            string folder = $"{rootPath}\\Db\\{company}";


            // CustomerInitializer($"{rootPath}\\Db\\arbear_customers_feb_2020.json");

            //InvoceInitializerDraft($"{folder}\\invoices_jan_2020.json");
            // InvoceInitializerDraft($"{folder}\\invoices_feb_2020.json");
            //InvoceInitializerDraft($"{folder}\\invoices_march_2020.json");

            //PaymentInitializerDraft($"{folder}\\invoices_jan_2020.json",
            //    $"{folder}\\invoices_feb_2020.json", new DateTime(2020, 2, 1), new DateTime(2020, 2, 29));

            //PaymentInitializerDraft($"{folder}\\invoices_feb_2020.json",
            //    $"{folder}\\invoices_march_2020.json", new DateTime(2020, 3, 1), new DateTime(2020, 3, 31));

        }

        private void RoleManager() {
            var roleManager = _serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            if(!roleManager.RoleExistsAsync("Administrator").Result) {
                IdentityResult roleResult = roleManager.CreateAsync(new IdentityRole() {
                    Name = "Administrator",
                    NormalizedName = "ADMINISTRATOR"
                }).Result;
            }

            if(!roleManager.RoleExistsAsync("User").Result) {
                IdentityResult roleResult = roleManager.CreateAsync(new IdentityRole() {
                    Name = "User",
                    NormalizedName = "USER"
                }).Result;
            }
        }

        private void ApplicationUser() {
            var userManager = _serviceProvider.GetRequiredService<UserManager<ApplicationUserEntity>>();

            if(userManager.FindByEmailAsync("test@test.com").Result == null) {
                var user = new ApplicationUserEntity() {
                    UserName = "test@test.com",
                    NormalizedUserName = "Тестовый пользователь",
                    Email = "test@test.com",
                    EmailConfirmed = true
                };

                var result = userManager.CreateAsync(user, "123qweAS1!").Result;
                if(result.Succeeded) {
                    userManager.AddToRoleAsync(user, "Administrator").Wait();
                }
            }
        }

        #region TEST
        private void CustomerInitializer(string fileUrl) {
            var _context = _serviceProvider.GetRequiredService<ApplicationContext>();
            var customers = _context.Customers.ToList();
            var customersIds = customers.Select(x => x.No);

            var JSON = System.IO.File.ReadAllText(fileUrl);
            var customerList = JsonConvert.DeserializeObject<List<CustomerEntity>>(JSON);

            var diffCustomers = customerList.Where(x => !customersIds.Contains(x.No));
            if(diffCustomers.Count() > 0) {
                _context.Customers.AddRange(diffCustomers);
                _context.SaveChanges();

                var activities = diffCustomers.Select(x => new CustomerActivityEntity() {
                    CreatedDate = x.CreatedDate,
                    CustomerId = x.Id,
                    IsActive = true
                });

                _context.CustomerActivities.AddRange(activities);
                _context.SaveChanges();
            }
        }

        private void InvoceInitializerDraft(string fileUrl) {
            var _context = _serviceProvider.GetRequiredService<ApplicationContext>();
            var customers = _context.Invoices.ToList();

            var JSON = System.IO.File.ReadAllText(fileUrl);
            var invoiceList = JsonConvert.DeserializeObject<List<InvoiceEntity>>(JSON);
            var diffInvoices = 0;
            foreach(var i in invoiceList) {
                if(i.Subtotal != 0) {
                    var customer = _context.Customers.Where(x => x.No.Equals(i.CustomerAccountNumber)).FirstOrDefault();

                    if(customer != null) {
                        var invoice = _context.Invoices.Where(x => x.CustomerId.Equals(customer.Id) && x.Subtotal.Equals(i.Subtotal)).FirstOrDefault();
                        if(invoice == null) {
                            i.CustomerId = customer.Id;
                            _context.Invoices.Add(i);
                        } else {
                            diffInvoices++;
                            Console.WriteLine($"INVOICE ALREADY EXISTED: CustomerId {i.CustomerId}, No {invoice.No}, Subtotal {invoice.Subtotal}");
                        }
                    } else {
                        Console.WriteLine($"INVOICE CUSTOMER NOT FOUND: CustomerId {i.CustomerAccountNumber}");
                    }
                }
            }
            var test = diffInvoices;
            _context.SaveChanges();
        }

        private void PaymentInitializerDraft(string fileUrlFrom, string fileUrlTo, DateTime startDate, DateTime endDate) {
            var _context = _serviceProvider.GetRequiredService<ApplicationContext>();

            var fromJson = System.IO.File.ReadAllText(fileUrlFrom);
            var toJson = System.IO.File.ReadAllText(fileUrlTo);

            var invoiceListFrom = JsonConvert.DeserializeObject<List<InvoiceEntity>>(fromJson);
            var invoiceListTo = JsonConvert.DeserializeObject<List<InvoiceEntity>>(toJson);

            //find records in two list with diff SUBTOTAL and the same CUSTOMERACCOUNTNUMBER
            //exclude records which the same PRICE and CUSTOMERACCOUNTNUMBER in two lists
            var diffInvoiceList = invoiceListFrom.Where(x => !invoiceListTo.Any(p => x.CustomerAccountNumber == p.CustomerAccountNumber && x.Subtotal == p.Subtotal)).ToList();
            //find record in FIRST LIST witch no in th SECOND LIST
            //var noInSecondInvoiceList = invoiceListFrom.Where(x => !invoiceListTo.Any(p => p.CustomerAccountNumber == x.CustomerAccountNumber && p.Subtotal == x.Subtotal)).ToList();

            //diffInvoiceList.AddRange(noInSecondInvoiceList);
            var listPaymentEntity = new List<PaymentEntity>();
            foreach(var i in diffInvoiceList) {
                var customer = _context.Customers.Where(x => x.No.Equals(i.CustomerAccountNumber)).FirstOrDefault();
                if(customer != null) {
                    var random = new Random();
                    var invoice = _context.Invoices.Where(x => x.CustomerId.Equals(customer.Id) && x.Subtotal.Equals(i.Subtotal)).FirstOrDefault();
                    if(invoice != null) {
                        var newDate = random.NextDate(startDate, endDate);

                        var payment = new PaymentEntity() {
                            No = "I" + invoice.No,
                            InvoiceId = invoice.Id,
                            IsDraft = true,
                            Amount = invoice.Subtotal,
                            CreatedDate = DateTime.Now,
                            Date = newDate,
                            UpdatedDate = DateTime.Now,
                        };

                        //listPaymentEntity.Add(payment);
                        //if(listPaymentEntity.Count == 100) {
                        //    try {
                        _context.Payments.Add(payment);
                        //_context.Payments.AddRange(listPaymentEntity);
                        //_context.SaveChanges();
                        //listPaymentEntity.Clear();
                        //}catch (Exception ex) {
                        //    Console.WriteLine(ex.ToString());
                        //}
                        //  }

                    } else {
                        Console.WriteLine($"PAYMENT INVOICE NOT FOUND: CustomerId {i.CustomerId}, Payment {i.Subtotal}");
                    }
                } else {
                    Console.WriteLine($"PAYMENT CUSTOMER NOT FOUND: CustomerId {i.CustomerAccountNumber}");
                }
            }
            _context.SaveChanges();
        }
        /*
        private void CompanyInitializer() {
            if(_context.Companies.Any()) {
                return;
            }

            string rootPath = System.IO.Directory.GetCurrentDirectory();

            var JSON = System.IO.File.ReadAllText($"{rootPath}\\Db\\companies.json");
            var companyList = JsonConvert.DeserializeObject<List<CompanyEntity>>(JSON);
            _context.Companies.AddRange(companyList);
        }
        */
        #endregion
    }
}
