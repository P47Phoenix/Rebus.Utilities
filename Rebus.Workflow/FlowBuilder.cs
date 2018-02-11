using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Rebus.Bus;
using Rebus.Messages;

namespace Rebus.Workflow
{

    public class FlowBuilder : IFlowBuilder
    {
        private readonly IBus m_bus;
        public Queue<MessageWrapper> m_messages = new Queue<MessageWrapper>();
        public Dictionary<string, string> m_MessageHeaders = new Dictionary<string, string>();

        public FlowBuilder(IBus bus)
        {
            m_bus = bus;
        }

        public void AddStep<T>(T message)
        {
            m_messages.Enqueue(new MessageWrapper
            {
                Message = message
            });
        }

        public void AddStateData<T>(string key, T data)
        {
            var dataKey = MessageContextHelpers.GetStateDataKey(key);

            var dataString = MessageContextHelpers.SerializeDataToString(data);

            m_MessageHeaders.Add(dataKey, dataString);
        }

        public async Task Send()
        {
            if (m_messages.Count == 0)
            {
                throw new InvalidOperationException("No messages have been added to flow to send");
            }

            var firstMessage = m_messages.Dequeue();

            string key = firstMessage.MesssageId.ToString();

            m_MessageHeaders.Add(Headers.MessageId, key);
            while (m_messages.Count != 0)
            {
                var item = m_messages.Dequeue();

                var message = MessageContextHelpers.SerializeDataToString(item.Message);
                m_MessageHeaders.Add(key, message);

                key = item.MesssageId.ToString();
            }

            await m_bus.Send(firstMessage.Message, m_MessageHeaders);
        }
    }
}
