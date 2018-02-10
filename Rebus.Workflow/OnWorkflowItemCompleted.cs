using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Rebus.Bus;
using Rebus.Messages;
using Rebus.Pipeline;

namespace Rebus.Workflow
{
    public class OnWorkflowItemCompleted : IIncomingStep
    {
        private readonly MessageContext m_messageContext;
        private readonly IBus m_bus;

        public OnWorkflowItemCompleted(MessageContext messageContext, IBus bus)
        {
            m_messageContext = messageContext;
            m_bus = bus;
        }

        public async Task Process(IncomingStepContext context, Func<Task> next)
        {
            await next();
            
            var key = m_messageContext.GetKey();

            var nextMessage = m_messageContext.Get(key);

            if (nextMessage == null)
            {
                return;
            }

            await m_bus.Send(nextMessage, new Dictionary<string, string>()
            {
                {
                    Headers.CorrelationId,
                    m_messageContext.Headers[Headers.CorrelationId]
                },
                {
                    Headers.MessageId,
                    m_messageContext.Headers[Headers.MessageId]
                }
            });
        }
    }
}
