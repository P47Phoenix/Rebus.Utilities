using System;

namespace Rebus.Workflow
{
    public class MessageWrapper
    {
        internal MessageWrapper() { }

        public Object Message { get; set; }

        public Guid MesssageId { get; set; } = Guid.NewGuid();
    }
}
