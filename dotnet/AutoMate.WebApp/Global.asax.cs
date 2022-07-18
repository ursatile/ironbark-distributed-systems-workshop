using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Autofac;
using Autofac.Integration.Mvc;
using Autofac.Integration.WebApi;
using AutoMate.Data;
using MassTransit;

namespace AutoMate.WebApp
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        private IContainer container;

        private const string RABBITMQ_URL =
            "amqps://karekqvh:5NidEd8zPSU1DIdg-kFcMB0D3B3Ws9nY@hefty-silver-gopher.rmq4.cloudamqp.com/karekqvh";
        protected void Application_Start()
        {
            var builder = new ContainerBuilder();
            var config = GlobalConfiguration.Configuration;
            builder.RegisterApiControllers(Assembly.GetExecutingAssembly());
            builder.RegisterControllers(Assembly.GetExecutingAssembly());
            builder.RegisterType<AutoMateCsvFileDatabase>().As<IAutoMateDatabase>().SingleInstance();
            builder.RegisterMassTransitBus(RABBITMQ_URL);
            container = builder.Build();
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
            config.DependencyResolver = new AutofacWebApiDependencyResolver(container);

            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
    }

    public static class ContainerBuilderExtensions
    {
        public static ContainerBuilder RegisterMassTransitBus(this ContainerBuilder builder, string rabbitMqUrl)
        {
            Messages.Conventions.MapEndpoints();
            builder.Register(c => Bus.Factory.CreateUsingRabbitMq(
                    config => config.Host(rabbitMqUrl)))
                .As<IBusControl>()
                .As<IBus>()
                .As<IPublishEndpoint>()
                .As<ISendEndpointProvider>()
                .SingleInstance();
            return builder;
        }
    }
}
