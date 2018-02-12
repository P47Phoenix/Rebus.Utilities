using Rebus.Bus;

namespace Rebus.Workflow
{
    public class FlowBuilderFactory : IFlowBuilderFactory
    {
        private readonly IBus m_bus;

        public FlowBuilderFactory(IBus bus)
        {
            m_bus = bus;
        }

        public IFlowBuilder Create()
        {
            return new FlowBuilder(m_bus);
        }
    }
}
