using AutoMate.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using Autobarn.Website.GraphQL.GraphTypes;
using Autobarn.Website.GraphQL.Schemas;
using Autobarn.Website.Hubs;
using Autobarn.Website.Services;
using GraphiQl;
using GraphQL;
using GraphQL.MicrosoftDI;
using GraphQL.NewtonsoftJson;
using GraphQL.Server;
using GraphQL.Types;
using MassTransit;

namespace Autobarn.Website {
    public class Startup {
        protected virtual string DatabaseMode => Configuration["DatabaseMode"];

        public Startup(IConfiguration configuration) {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services) {
            services.AddRouting(options => options.LowercaseUrls = true);
            services.AddControllersWithViews()
                .AddNewtonsoftJson(options => options.UseCamelCasing(processDictionaryKeys: true));
            services.AddRazorPages().AddRazorRuntimeCompilation();
            Console.WriteLine(DatabaseMode);
            services.AddSingleton<IAutoMateDatabase, AutoMateCsvFileDatabase>();

            //var bus = RabbitHutch.CreateBus(amqp);
            //services.AddSingleton(bus);
            services.AddSingleton<IClock>(new SystemClock());

            services.AddGraphQL(builder =>
                builder.AddHttpMiddleware<ISchema>()
                    .AddNewtonsoftJson()
                    .AddSchema<AutobarnSchema>()
                    .AddGraphTypes(typeof(VehicleGraphType).Assembly)
            );
			services.AddSignalR();
            services.AddMassTransit(x => {
                x.SetKebabCaseEndpointNameFormatter();
                x.UsingRabbitMq((context, rabbit) => {
                    rabbit.Host("amqps://karekqvh:5NidEd8zPSU1DIdg-kFcMB0D3B3Ws9nY@hefty-silver-gopher.rmq4.cloudamqp.com/karekqvh");
                    rabbit.ConfigureEndpoints(context);
                });
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env) {
            if (env.IsDevelopment()) {
                app.UseDeveloperExceptionPage();
            } else {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseDefaultFiles();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthorization();
            app.UseGraphQL<ISchema>();
            app.UseGraphiQl("/graphiql");
            app.UseEndpoints(endpoints => {
                endpoints.MapHub<AutobarnHub>("/hub");
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
