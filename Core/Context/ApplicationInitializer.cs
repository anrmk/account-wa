using System;
using System.Collections.Generic;
using System.Linq;

using Core.Data.Entities;
using Core.Extension;
using Core.Services.Managers;
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
            //CustomerInitializer($"{rootPath}\\Db\\express_customers_jan_2020.json");

            //InvoceInitializerDraft($"{rootPath}\\Db\\express_invoices_jan_2020.json");

            //PaymentInitializerDraft($"{rootPath}\\Db\\western_invoices_dec_2019.json", 
            //    $"{rootPath}\\Db\\western_invoices_jan_2020.json", new DateTime(2020, 1, 1), new DateTime(2020, 1, 31));

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

            if(userManager.FindByEmailAsync("test@test.kz").Result == null) {
                var user = new ApplicationUserEntity() {
                    UserName = "test@test.kz",
                    NormalizedUserName = "Тестовый пользователь",
                    Email = "test@test.kz",
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
            var customersIds = customers.Select(x => x.AccountNumber);

            var JSON = System.IO.File.ReadAllText(fileUrl);
            var customerList = JsonConvert.DeserializeObject<List<CustomerEntity>>(JSON);

            var diffCustomers = customerList.Where(x => !customersIds.Contains(x.AccountNumber));
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
                    var customer = _context.Customers.Where(x => x.AccountNumber.Equals(i.CustomerAccountNumber)).FirstOrDefault();

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

            foreach(var i in diffInvoiceList) {
                var customer = _context.Customers.Where(x => x.AccountNumber.Equals(i.CustomerAccountNumber)).FirstOrDefault();
                if(customer != null) {
                    var invoice = _context.Invoices.Where(x => x.CustomerId.Equals(customer.Id) && x.Subtotal.Equals(i.Subtotal)).FirstOrDefault();
                    if(invoice != null) {
                        var newDate = RandomExtansion.GetRandomDateTime(startDate, endDate);

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
