using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Rebus.Bus;
using Rebus.Messages;
using Rebus.Pipeline;

namespace Rebus.Workflow
{
    [StepDocumentation("Start the next step in the flow if the last one completed successfully.")]
    public class OnWorkflowItemCompletedStep : IIncomingStep
    {
        private readonly IBus m_bus;

        public OnWorkflowItemCompletedStep(IBus bus)
        {
            m_bus = bus;
        }

        public async Task Process(IncomingStepContext context, Func<Task> next)
        {
            await next();
         
            var messageContext = MessageContext.Current;
            
            var key = messageContext.GetKey();

            var nextMessage = messageContext.Get<object>(key);

            if (nextMessage == null)
            {
                return;
            }

            var headers = new Dictionary<string, string>()
            {
                {
                    Headers.CorrelationId,
                    messageContext.Headers[Headers.CorrelationId]
                },
                {
                    Headers.MessageId,
                    messageContext.Headers[Headers.MessageId]
                }
            };

            foreach (var headerKeyValue in messageContext.Headers)
            {
                if (headerKeyValue.Key.StartsWith(MessageContextHelpers.DataKey))
                {
                    headers.Add(headerKeyValue.Key, headerKeyValue.Value);
                }
            }

            await m_bus.Send(nextMessage, headers);
        }
    }
}
