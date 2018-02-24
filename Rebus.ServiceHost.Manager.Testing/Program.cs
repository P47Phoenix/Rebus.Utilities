using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Sinks.Elasticsearch;

namespace Rebus.ServiceHost.Manager.Testing
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseSerilog((context, configuration) =>
                {
                    var elasticsearchSinkOptions = new ElasticsearchSinkOptions(new Uri("http://elasticsearch:9200"))
                    {
                        IndexFormat = "logs-{0:yyyy-MM-dd}"
                    };
                    configuration
                        .WriteTo.Elasticsearch(elasticsearchSinkOptions);
                })
                .UseStartup<Startup>()
                .Build();
    }
}
