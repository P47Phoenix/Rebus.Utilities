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
        private readonly MessageContext m_messageContext;

        public GetExternalIpAddressMessageHandler(MessageContext messageContext)
        {
            m_messageContext = messageContext;
        }

        public async Task Handle(GetExternalIpAddressMessage message)
        {
            var result = await new HttpClient().GetAsync("http://bot.whatismyipaddress.com/");

            var body = await result.Content.ReadAsStringAsync();

            if (IPAddress.TryParse(body, out IPAddress ipAddress))
            {
                var stateDataKey = MessageContextHelpers.GetStateDataKey(StateDataKeys.IpAddress);
                m_messageContext.Set(stateDataKey, ipAddress.ToString());
            }
        }
    }
}
