using System.Security.Principal;

using Core.Context;
using Core.Services.Business;
using Core.Services.Managers;

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
            //services.AddTransient<IViewRenderService, ViewRenderService>();

            ///Managers
            services.AddTransient<ICompanyManager, CompanyManager>();
            services.AddTransient<ICompanyAddressMananger, CompanyAddressManager>();
            services.AddTransient<ICompanySummaryRangeManager, CompanySummaryRangeManager>();
            services.AddTransient<ICompanySettingsManager, CompanySettingsManager>();
            services.AddTransient<ICompanyExportSettingsManager, CompanyExportSettingsManager>();
            services.AddTransient<ICompanyExportSettingsFieldManager, CompanyExportSettingsFieldManager>();
            services.AddTransient<ICompanySettingsRestrictedWordManager, CompanySettingsRestrictedWordManager>();

            services.AddTransient<ICustomerManager, CustomerManager>();
            services.AddTransient<ICustomerActivityManager, CustomerActivityManager>();
            services.AddTransient<ICustomerCreditLimitManager, CustomerCreditLimitManager>();
            services.AddTransient<ICustomerCreditUtilizedManager, CustomerCreditUtilizedManager>();
            services.AddTransient<ICustomerCreditUtilizedSettingsManager, CustomerCreditUtilizedSettingsManager>();

            services.AddTransient<ICustomerTagManager, CustomerTagManager>();
            services.AddTransient<ICustomerTagLinkManager, CustomerTagLinkManager>();
            services.AddTransient<ICustomerTypeManager, CustomerTypeManager>();
            services.AddTransient<ICustomerRecheckManager, CustomerRecheckManager>();
            services.AddTransient<ICustomerSettingsManager, CustomerSettingsManager>();
            services.AddTransient<ISettingsRestrictedWordManager, SettingsRestrictedWordManager>();

            services.AddTransient<IInvoiceManager, InvoiceManager>();
            services.AddTransient<IInvoiceConstructorManager, InvoiceConstructorManager>();
            services.AddTransient<IInvoiceDraftManager, InvoiceDraftManager>();
            services.AddTransient<IInvoiceConstructorSearchManager, InvoiceConstructorSearchManager>();

            services.AddTransient<IPaymentManager, PaymentManager>();

            services.AddTransient<IReportManager, ReportManager>();
            services.AddTransient<ISavedReportManager, SavedReportManager>();
            services.AddTransient<ISavedReportFieldManager, SavedReportFieldManager>();
            services.AddTransient<ISavedReportFileManager, SavedReportFileManager>();

            ///Business
            services.AddTransient<ICrudBusinessManager, CrudBusinessManager>();
            services.AddTransient<IAccountBusinessService, AccountBusinessService>();
            services.AddTransient<ICompanyBusinessManager, CompanyBusinessManager>();
            services.AddTransient<ICustomerBusinessManager, CustomerBusinessManager>();

            services.AddTransient<ISettingsBusinessService, SettingsBusinessService>();

            services.AddTransient<IReportBusinessManager, ReportBusinessManager>();
        }
    }
}
