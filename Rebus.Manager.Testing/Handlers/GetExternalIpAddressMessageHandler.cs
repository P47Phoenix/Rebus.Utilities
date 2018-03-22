using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Rebus.Handlers;
using Rebus.Manager.Testing.Contracts.Messages;
using Rebus.Manager.Testing.Contracts.StateData;
using Rebus.Pipeline;
using Rebus.Workflow;

namespace Rebus.Manager.Testing.Handlers
{
    public class GetExternalIpAddressMessageHandler : IHandleMessages<GetExternalIpAddressMessage>
    {
        private readonly ILogger<GetExternalIpAddressMessageHandler> m_logger;

        public GetExternalIpAddressMessageHandler(ILoggerFactory loggerFactory)
        {
            m_logger = loggerFactory.CreateLogger<GetExternalIpAddressMessageHandler>();
        }

        public async Task Handle(GetExternalIpAddressMessage message)
        {
            var stackTrace = new System.Diagnostics.StackTrace();

            var frames = stackTrace.GetFrames();

            var stack = frames.Select(f => new
            {
                MethodName = f.GetMethod().Name,
                ClassName = f.GetMethod().DeclaringType.Name
            }).ToList();

            m_logger.LogDebug(JArray.FromObject(stack).ToString(Formatting.Indented));

            var result = await new HttpClient().GetAsync("http://bot.whatismyipaddress.com/");

            var body = await result.Content.ReadAsStringAsync();

            if (IPAddress.TryParse(body, out IPAddress ipAddress))
            {
                MessageContext.Current.SetStateData(StateDataKeys.IpAddress, ipAddress.ToString());
            }
        }
    }
}
