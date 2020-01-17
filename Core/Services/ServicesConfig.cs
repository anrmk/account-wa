﻿using Core.Context;
using Core.Services.Business;
using Core.Services.Managers;

using Microsoft.Extensions.DependencyInjection;

namespace Core.Services {
    public class ServicesConfig {
        public static void Configuration(IServiceCollection services) {
            ///Context
            services.AddTransient<IApplicationContext, ApplicationContext>();
            //services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            ///Managers
            services.AddTransient<IReportManager, ReportManager>();
            services.AddTransient<ICompanyManager, CompanyManager>();
            services.AddTransient<ICustomerManager, CustomerManager>();
            services.AddTransient<ICompanyCustomerManager, CompanyCustomerManager>();
            services.AddTransient<IInvoiceManager, InvoiceManager>();
            services.AddTransient<IPaymentManager, PaymentManager>();

            //services.AddTransient<IAgingManager, AgingManager>();

            ///Business
            services.AddTransient<ICrudBusinessManager, CrudBusinessManager>();
            services.AddTransient<IReportBusinessManager, ReportBusinessManager>();

        }
    }
}
