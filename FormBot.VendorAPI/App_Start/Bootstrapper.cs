using Autofac;
using Autofac.Integration.WebApi;
using FormBot.BAL.Service;
using FormBot.BAL.Service.CommonRules;
using FormBot.VendorAPI.Models;
using FormBot.VendorAPI.Service;
using FormBot.VendorAPI.Service.Job;
using System;
using System.Diagnostics;
using System.Net.Http;
using System.Reflection;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Dispatcher;

namespace FormBot.VendorAPI.App_Start
{
    public static class Bootstrapper
    {
        public static void Run()
        {
            BootStrapAutofac();
        }

        private static void BootStrapAutofac()
        {
            var builder = new ContainerBuilder();
            var config = GlobalConfiguration.Configuration;

            
            
            builder.RegisterApiControllers(Assembly.GetExecutingAssembly());
            builder.RegisterAssemblyTypes(typeof(JobRulesBAL).Assembly).Where(t => t.Name.EndsWith("BAL")).AsImplementedInterfaces().InstancePerApiRequest();
            builder.RegisterAssemblyTypes(typeof(JobDetails).Assembly).Where(t => t.Name.EndsWith("ils")).AsImplementedInterfaces().InstancePerApiRequest();
            builder.RegisterAssemblyTypes(typeof(UserBAL).Assembly).Where(t => t.Name.EndsWith("BAL")).AsImplementedInterfaces().InstancePerApiRequest();
            //builder.RegisterAssemblyTypes(typeof(VendorAPI.Controllers.AccountController).Assembly).Where(t => t.Name.EndsWith("BAL")).AsImplementedInterfaces().InstancePerApiRequest();
            builder.RegisterWebApiFilterProvider(config);


            //builder.RegisterType<ApplicationDbContext>().AsSelf().As<ApplicationUser>().InstancePerApiRequest();


            //builder.RegisterType<VendorAPIS>().As<IVendorAPI>();
            //builder.RegisterType<JobRulesBAL>().As<IJobRulesBAL>();
            IContainer container = builder.Build();

            //DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
            config.DependencyResolver = new AutofacWebApiDependencyResolver(container);

            
        }

        //private sealed class AutofacControllerActivator : IHttpControllerActivator
        //{
        //    private readonly IContainer _container;
        //    public AutofacControllerActivator(IContainer container)
        //    {
        //        this._container = container;
        //    }

        //    [DebuggerStepThrough]
        //    public IHttpController Create(HttpRequestMessage request, HttpControllerDescriptor controllerDescriptor, Type controllerType)
        //    {
        //        return (IHttpController)this._container.Resolve(controllerType);
        //    }
        //}
    }
}