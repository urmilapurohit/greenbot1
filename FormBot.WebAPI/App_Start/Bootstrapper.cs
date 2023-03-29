using Autofac;
using Autofac.Integration.WebApi;
using FormBot.BAL.Service;
using FormBot.DAL;
using FormBot.WebAPI.Controllers;
using System;
using System.Diagnostics;
using System.Net.Http;
using System.Reflection;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Dispatcher;

namespace WebApiDemo.App_Start
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
            /*builder.RegisterAssemblyTypes(typeof(UserBAL).Assembly).Where(t => t.Name.EndsWith("BAL")).AsImplementedInterfaces().InstancePerApiRequest();
            builder.RegisterAssemblyTypes(typeof(CommonDAL).Assembly).Where(t => t.Name.EndsWith("DAL")).AsImplementedInterfaces().InstancePerApiRequest();*/
            builder.RegisterWebApiFilterProvider(config);
            IContainer container = builder.Build();
            config.DependencyResolver = new AutofacWebApiDependencyResolver(container);
        }

        private sealed class AutofacControllerActivator : IHttpControllerActivator
        {
            private readonly IContainer _container;
            public AutofacControllerActivator(IContainer container)
            {
                this._container = container;
            }

            [DebuggerStepThrough]
            public IHttpController Create(HttpRequestMessage request, HttpControllerDescriptor controllerDescriptor, Type controllerType)
            {
                return (IHttpController)this._container.Resolve(controllerType);
            }
        }
    }
}