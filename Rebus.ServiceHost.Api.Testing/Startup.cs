using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Rebus.AspNetCoreExtensions;
using Rebus.Config;
using Rebus.Manager.Testing.Contracts.Messages;
using Rebus.Pipeline;
using Rebus.Pipeline.Send;
using Rebus.RabbitMq;
using Rebus.Routing.TransportMessages;
using Rebus.Routing.TypeBased;
using Rebus.ServiceProvider;
using Rebus.Timeouts;
using Untilities.Api;

namespace Rebus.ServiceHost.Api.Testing
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
            services.AddMvc();
            services.AddRebus(configurer =>
            {
                configurer.Options(optionsConfigurer =>
                {
                    optionsConfigurer.SetMaxParallelism(1);
                    optionsConfigurer.SetNumberOfWorkers(1);
                    optionsConfigurer.LogPipeline(true);

                    optionsConfigurer.Decorate<IPipeline>(context =>
                    {
                        var pipeline = context.Get<IPipeline>();
                        var outgoingStep = new AspNetCorrelationIdStep(services);
                        return new PipelineStepInjector(pipeline)
                            .OnSend(outgoingStep, PipelineRelativePosition.Before, typeof(SendOutgoingMessageStep));
                    });
                });

                configurer.Routing(standardConfigurer => standardConfigurer
                    .TypeBased()
                    .MapAssemblyOf<GetExternalIpAddressMessage>("TestWorkFlow"));

                configurer.Transport(standardConfigurer =>
                {
                    standardConfigurer.UseRabbitMqAsOneWayClient(new List<ConnectionEndpoint>
                    {
                        new ConnectionEndpoint
                        {
                            ConnectionString = "amqp://rabbitmq"
                        }
                    });
                });


                return configurer;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRebus();

            app.UseMiddleware<CorrelationIdMiddleware>();

            app.UseMvc();
        }
    }
}
