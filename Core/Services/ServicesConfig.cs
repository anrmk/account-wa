using System;
using System.Collections.Generic;
using System.Text;
using Core.Context;
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
            services.AddTransient<ICompanyManager, CompanyManager>();
            services.AddTransient<ICustomerManager, CustomerManager>();

            ///Business
            services.AddTransient<ICompanyBusinessManager, CompanyBusinessManager>();

        }
    }
}
