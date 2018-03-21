using System.Net;
using System.Threading.Tasks;
using Rebus.Handlers;
using Rebus.Manager.Testing.Contracts.Messages;
using Rebus.Manager.Testing.Contracts.StateData;
using Rebus.Manager.Testing.Controller;
using Rebus.Pipeline;
using Rebus.Workflow;

namespace Rebus.Manager.Testing.Handlers
{
    public class SetExternalIpAddressMessageHandleMessages : IHandleMessages<SetExternalIpAddressMessage>
    {
        public async Task Handle(SetExternalIpAddressMessage message)
        {
            var ipAddressString = MessageContext.Current.GetStateData<string>(StateDataKeys.IpAddress);

            if (IPAddress.TryParse(ipAddressString, out IPAddress ipAddress))
            {
                IpAddressController.ExternalIpAddress = ipAddress;
            }
        }
    }
}
