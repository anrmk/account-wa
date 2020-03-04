﻿using System.Security.Principal;

using Core.Context;
using Core.Extensions;
using Core.Services.Business;
using Core.Services.Managers;
using Core.Services.Managers.Nsi;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Services {
    public class ServicesConfig {
        public static void Configuration(IServiceCollection services) {
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddTransient<IActionContextAccessor, ActionContextAccessor>();

            services.AddSingleton<IPrincipal>(provider => provider.GetService<IHttpContextAccessor>().HttpContext?.User);

            ///Context
            services.AddTransient<IApplicationContext, ApplicationContext>();
            services.AddTransient<IUserProfileManager, UserProfileManager>();
            //services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            ///Extension Service
            services.AddTransient<IViewRenderService, ViewRenderService>();

            ///Managers
            services.AddTransient<IReportManager, ReportManager>();
            services.AddTransient<ICompanyManager, CompanyManager>();
            services.AddTransient<ICompanyAddressMananger, CompanyAddressManager>();
            services.AddTransient<ICompanySummaryRangeManager, CompanySummaryRangeManager>();
            services.AddTransient<ICompanyExportSettingsManager, CompanyExportSettingsManager>();
            services.AddTransient<ICompanyExportSettingsFieldManager, CompanyExportSettingsFieldManager>();
            services.AddTransient<ICustomerManager, CustomerManager>();
            services.AddTransient<ICustomerActivityManager, CustomerActivityManager>();
            services.AddTransient<IInvoiceManager, InvoiceManager>();
            services.AddTransient<IPaymentManager, PaymentManager>();

            ///NSI
            //services.AddTransient<IReportPeriodManager, ReportPeriodManager>();
            services.AddTransient<IReportFieldManager, ReportFieldManager>();

            ///Business
            services.AddTransient<INsiBusinessManager, NsiBusinessManager>();
            services.AddTransient<ICrudBusinessManager, CrudBusinessManager>();
            services.AddTransient<IAccountBusinessService, AccountBusinessService>();
            services.AddTransient<IReportBusinessManager, ReportBusinessManager>();
        }
    }
}
