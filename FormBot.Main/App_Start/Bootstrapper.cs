using Autofac;
using System.Reflection;
using Autofac.Integration.Mvc;
using System.Web.Mvc;
using FormBot.BAL.Service;
using FormBot.DAL;
using FormBot.Helper;

namespace FormBot.Main.App_Start
{
    public static class Bootstrapper
    {
        public static void Run()
        {
            SetAutofacContainer();
        }

        private static void SetAutofacContainer()
        {
            var builder = new ContainerBuilder();
            builder.RegisterControllers(Assembly.GetExecutingAssembly());
            builder.RegisterAssemblyTypes(typeof(CERImportBAL).Assembly).Where(t => t.Name.EndsWith("BAL")).AsImplementedInterfaces().InstancePerHttpRequest();
            builder.RegisterAssemblyTypes(typeof(UserBAL).Assembly).Where(t => t.Name.EndsWith("BAL")).AsImplementedInterfaces().InstancePerHttpRequest();
			builder.RegisterAssemblyTypes(typeof(EmailBAL).Assembly).Where(t => t.Name.EndsWith("BAL")).AsImplementedInterfaces().InstancePerHttpRequest();
            //builder.RegisterAssemblyTypes(typeof(Logger).Assembly).Where(t => t.Name == "ILogger").AsImplementedInterfaces().InstancePerHttpRequest();
            //builder.RegisterAssemblyTypes(typeof(Logger).Assembly).As<ILogger>().SingleInstance();
            builder.RegisterType<Logger>().As<ILogger>();
            //     builder.RegisterAssemblyTypes(typeof(Logger).Assembly)
            //.Where(t => t.Name == "ILogger")
            //.AsImplementedInterfaces()
            //.AsSelf()
            //.InstancePerLifetimeScope();
            builder.RegisterFilterProvider();
            IContainer container = builder.Build();
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
        }
    }

    public static class CheckApp
    {
        public static string IsStart { get; set; }
    }
}