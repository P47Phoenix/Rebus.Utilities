using System;
using System.Collections.Generic;
using System.Text;
using Rebus.Bus;
using Rebus.Messages;
using Rebus.Pipeline;
using Rebus.Serialization;

namespace Rebus.Workflow
{
    public static class MessageContextHelpers
    {
        private static readonly ObjectSerializer m_objectSerializer = new ObjectSerializer();

        public static object Get(this IMessageContext messageContext, string key)
        {
            if (messageContext.Headers.ContainsKey(key) == false)
            {
                return null;
            }

            var messageString = messageContext.Headers[key];

            return m_objectSerializer.DeserializeFromString(messageString);
        }

        public static void Set<T>(this IMessageContext messageContext, string key, T data)
        {
            messageContext.Headers[key] = m_objectSerializer.SerializeToString(data);
        }

        public static string GetKey(this IMessageContext messageContext)
        {
            var messsageId = messageContext.Message.GetMessageId();

            var correlationId = messageContext.Headers[Headers.CorrelationId];

            return $"{correlationId}_{messsageId}";
        }
    }
}
