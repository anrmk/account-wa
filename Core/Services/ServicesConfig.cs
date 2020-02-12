using Core.Context;
using Core.Extensions;
using Core.Services.Business;
using Core.Services.Managers;

using Microsoft.Extensions.DependencyInjection;

namespace Core.Services {
    public class ServicesConfig {
        public static void Configuration(IServiceCollection services) {
            ///Context
            services.AddTransient<IApplicationContext, ApplicationContext>();
            services.AddTransient<IUserProfileManager, UserProfileManager>();
            //services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            ///Extension Service
            services.AddTransient<IViewRenderService, ViewRenderService>();

            ///Managers
            services.AddTransient<IReportManager, ReportManager>();
            services.AddTransient<ICompanyManager, CompanyManager>();
            services.AddTransient<ICompanySummaryRangeManager, CompanySummaryRangeManager>();
            services.AddTransient<ICustomerManager, CustomerManager>();
            services.AddTransient<ICustomerActivityManager, CustomerActivityManager>();
            services.AddTransient<IInvoiceManager, InvoiceManager>();
            services.AddTransient<IPaymentManager, PaymentManager>();

            ///Business
            services.AddTransient<ICrudBusinessManager, CrudBusinessManager>();
            services.AddTransient<IAccountBusinessService, AccountBusinessService>();
            services.AddTransient<IReportBusinessManager, ReportBusinessManager>();
            

        }
    }
}
