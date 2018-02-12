using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Rebus.Handlers;
using Rebus.Manager.Testing.Contracts.Messages;
using Rebus.Manager.Testing.Contracts.StateData;
using Rebus.Pipeline;
using Rebus.Workflow;

namespace Rebus.Manager.Testing.Handlers
{
    public class GetExternalIpAddressMessageHandler : IHandleMessages<GetExternalIpAddressMessage>
    {
        public async Task Handle(GetExternalIpAddressMessage message)
        {
            var result = await new HttpClient().GetAsync("http://bot.whatismyipaddress.com/");

            var body = await result.Content.ReadAsStringAsync();

            if (IPAddress.TryParse(body, out IPAddress ipAddress))
            {
                MessageContext.Current.SetStateData(StateDataKeys.IpAddress, ipAddress.ToString());
            }
        }
    }
}
