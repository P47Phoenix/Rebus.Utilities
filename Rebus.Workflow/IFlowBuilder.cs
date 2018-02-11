using System.Threading.Tasks;

namespace Rebus.Workflow
{
    public interface IFlowBuilder
    {
        void AddStep<T>(T message);
        void AddStateData<T>(string key, T data);
        Task Send();
    }
}
