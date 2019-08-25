using System;
using System.Reflection;
using System.Text.Json;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using REDTransport.NET.Server.AspNet;
using REDTransport.NET.Server.AspNet.Crud;
using REDTransport.NET.Server.AspNet.GraphQL;
using REDTransport.NET.Server.AspNet.OData;
using REDTransport.NET.Server.AspNet.Pipeline;

namespace REDTransport.NET.Sample0
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public IWebHostEnvironment Environment { get; }


        public Startup(IWebHostEnvironment environment, IConfiguration configuration)
        {
            Environment = environment;
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public virtual IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton(Configuration);

            services.AddRedTransport(opt =>
            {
                //opt.MapControllers();
                opt.AllowRequestAggregation = true;
                opt.RequestDispatcherStrategy = RequestDispatcherStrategy.InProcess;
                opt.InProcessScopeMode = RedTransportInProcessScopeMode.UseRootScope;
                
                opt.AddOData();
                opt.AddGraphQL();
                opt.AddCrud(cOpt => { });

                opt.AddEndpoint("rt", endpoint =>
                {
                    endpoint.Route = "/api/rt";

                    endpoint.WhiteListSubRoutes = new[]
                    {
                        "/*",
                        "/v2/test/*",
                        "/v2/controller1/*",
                        "/v2/sub-directory/*",
                    };

                    endpoint.BlackListSubRoutes = new[]
                    {
                        "/v1/*"
                    };

                    endpoint.MapToRoute = "/api";
                });
            });

            services.AddControllers()
                .AddJsonOptions(opt =>
                {
                    opt.JsonSerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;
                    opt.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                    opt.JsonSerializerOptions.IgnoreNullValues = true;
                    opt.JsonSerializerOptions.PropertyNameCaseInsensitive = true;

                });
            
            
            var startupAssembly = typeof(Startup).GetTypeInfo().Assembly;

            var manager = new ApplicationPartManager
            {
                ApplicationParts =
                {
                    new AssemblyPart(startupAssembly)
                },
                FeatureProviders =
                {
                    new ControllerFeatureProvider(),
                    new ViewComponentFeatureProvider()
                }
            };

            services.AddSingleton(manager);
            
            return services.BuildServiceProvider();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public virtual void Configure(IApplicationBuilder app)
        {
            if (Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseRedTransport();

            app.UseRouting();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}