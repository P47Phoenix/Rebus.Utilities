using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.ObjectPool;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Rebus.AspNetCoreExtensions;
using Rebus.Config;
using Rebus.Manager.Testing;
using Rebus.Manager.Testing.Contracts.Messages;
using Rebus.Manager.Testing.Handlers;
using Rebus.Pipeline;
using Rebus.Pipeline.Send;
using Rebus.RabbitMq;
using Rebus.Retry.Simple;
using Rebus.Routing.TypeBased;
using Rebus.ServiceProvider;
using Swashbuckle.AspNetCore.Swagger;
using Untilities.Api;
using Utilities.Api;

namespace Rebus.ServiceHost.Manager.Testing
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var logger = services
                .BuildServiceProvider()
                .GetService<ILoggerFactory>()
                .CreateLogger<Startup>();
            

            services
                .AddMvcCore(options =>
                {
                })
                .AddVersionedApiExplorer(
                    options =>
                    {
                        options.GroupNameFormat = "'v'VVV";
                    });

            services
                .AddMvc(options =>
                {
                })
                .ConfigureApplicationPartManager(manager =>
                {
                    manager.ApplicationParts.Clear();
                    manager.ApplicationParts.Add(new AssemblyPart(typeof(IpAddressController).Assembly));
                })
                .AddMvcOptions(options =>
                {
                    options.OutputFormatters.Clear();
                    options.InputFormatters.Clear();

                    var jsonSettings = new JsonSerializerSettings
                    {
                        DateTimeZoneHandling = DateTimeZoneHandling.RoundtripKind,
                        DefaultValueHandling = DefaultValueHandling.Ignore
                    };

                    jsonSettings.Converters.Add(new IsoDateTimeConverter());
                    jsonSettings.Converters.Add(new StringEnumConverter()
                    {
                        AllowIntegerValues = false,
                        CamelCaseText = true
                    });

                    options.OutputFormatters.Add(new JsonOutputFormatter(jsonSettings, ArrayPool<char>.Shared));
                    options.InputFormatters.Add(new JsonInputFormatter(logger, jsonSettings, ArrayPool<char>.Shared, new DefaultObjectPoolProvider()));
                });

            services.AddApiVersioning(o => o.ReportApiVersions = true);
            services.AddSwaggerGen(
                options =>
                {
                    // resolve the IApiVersionDescriptionProvider service
                    // note: that we have to build a temporary service provider here because one has not been created yet
                    var provider = services
                        .BuildServiceProvider()
                        .GetRequiredService<IApiVersionDescriptionProvider>();

                    // add a swagger document for each discovered API version
                    // note: you might choose to skip or document deprecated API versions differently
                    foreach (var description in provider.ApiVersionDescriptions)
                    {
                        options.SwaggerDoc(description.GroupName, CreateInfoForApiVersion(description));
                    }

                    // add a custom operation filter which sets default values
                    options.OperationFilter<SwaggerDefaultValues>();
                });

            services.AutoRegisterHandlersFromAssemblyOf<GetExternalIpAddressMessageHandler>();

            services.AddRebus(configurer =>
            {
                configurer.Options(optionsConfigurer =>
                {
                    optionsConfigurer.SetMaxParallelism(1);
                    optionsConfigurer.SetNumberOfWorkers(1);
                    optionsConfigurer.LogPipeline(true);

                    optionsConfigurer.SimpleRetryStrategy();

                    optionsConfigurer.Register(c=> new AspNetCorrelationIdStep(new HttpContextAccessor()));

                    optionsConfigurer.Decorate<IPipeline>(context => new PipelineStepInjector(context.Get<IPipeline>())
                        .OnSend(context.Get<AspNetCorrelationIdStep>(), PipelineRelativePosition.Before, typeof(SendOutgoingMessageStep)));
                });

                configurer.Routing(standardConfigurer => standardConfigurer
                    .TypeBased()
                    .MapAssemblyOf<GetExternalIpAddressMessage>("TestWorkFlow"));

                configurer.Transport(standardConfigurer =>
                {
                    var connectionEndpoints =  new List<ConnectionEndpoint>
                    {
                        new ConnectionEndpoint
                        {
                            ConnectionString = "amqp://rabbitmq"
                        }
                    };
                    standardConfigurer.UseRabbitMq(connectionEndpoints, "TestWorkFlow");
                });


                return configurer;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, IApiVersionDescriptionProvider provider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRebus();

            app.UseMiddleware<CorrelationIdMiddleware>();

            app.UseMvc();
            
            app.UseSwagger();
            app.UseSwaggerUI(
                options =>
                {
                    // build a swagger endpoint for each discovered API version
                    foreach (var description in provider.ApiVersionDescriptions)
                    {
                        options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json", description.GroupName.ToUpperInvariant());
                    }
                });
        }
        
        static Info CreateInfoForApiVersion(ApiVersionDescription description)
        {
            var info = new Info()
            {
                Title = $"Sample API {description.ApiVersion}",
                Version = description.ApiVersion.ToString(),
                Description = "A sample application with Swagger, Swashbuckle, and API versioning.",
                Contact = new Contact() { Name = "Michael Connelly", Email = "michaelconne@gmail.com" },
                TermsOfService = "Closed",
            };

            if (description.IsDeprecated)
            {
                info.Description += " This API version has been deprecated.";
            }

            return info;
        }
    }
}
