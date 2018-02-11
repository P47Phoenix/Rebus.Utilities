using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Rebus.Bus;

namespace Rebus.Workflow
{

    public class FlowBuilder : IFlowBuilder
    {
        private readonly IBus m_bus;
        public Queue<object> m_messages = new Queue<object>();
        public IDictionary<string, string> m_WorkFlowStateData = new Dictionary<string, string>();

        public FlowBuilder(IBus bus)
        {
            m_bus = bus;
        }

        public void AddStep(object message)
        {
            m_messages.Enqueue(message);
        }

        public void AddStateData<T>(string key, T data)
        {
            var dataKey = MessageContextHelpers.GetStateDataKey(key);

            var dataString = MessageContextHelpers.SerializeDataToString(data);

            m_WorkFlowStateData.Add(dataKey, dataString);
        }

        public async Task Send()
        {

        }
    }
}
