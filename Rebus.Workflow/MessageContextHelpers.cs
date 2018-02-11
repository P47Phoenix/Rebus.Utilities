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

        internal const string
            DataKey = nameof(DataKey);


        public static T GetStateData<T>(this IMessageContext messageContext, string key) where T : class
        {
            var stateDataKey = GetStateDataKey(key);

            return messageContext.Get<T>(stateDataKey);            
        }

        internal static T Get<T>(this IMessageContext messageContext, string key) where T : class
        {
            if (messageContext.Headers.ContainsKey(key) == false)
            {
                return null;
            }

            var messageString = messageContext.Headers[key];

            var data = m_objectSerializer.DeserializeFromString(messageString);

            if (data is T dataTyped)
            {
                return dataTyped;
            }

            return null;
        }

        public static void SetStateData<T>(this IMessageContext messageContext, string key, T data)
        {
            var stateDataKey = GetStateDataKey(key);

            messageContext.Set(stateDataKey, data);
        }

        internal static void Set<T>(this IMessageContext messageContext, string key, T data)
        {
            messageContext.Headers[key] = SerializeToString(data);
        }

        public static string SerializeToString<T>(T data)
        {
            return m_objectSerializer.SerializeToString(data);
        }

        internal static string GetKey(this IMessageContext messageContext)
        {
            var messsageId = messageContext.Message.GetMessageId();


            return $"{messsageId}";
        }

        internal static string SerializeDataToString<T>(T data)
        {
            return m_objectSerializer.SerializeToString(data);
        }

        internal static T DeserializeDataFromString<T>(string dataString)
        {
            var data = m_objectSerializer.DeserializeFromString(dataString);

            if (data is T value)
            {
                return value;
            }

            throw new ArgumentException($"data is not the expected type {typeof(T).Name} and {data?.GetType()?.Name ?? "Unknown"}");
        }

        internal static string GetStateDataKey(string key)
        {
            return $"{DataKey}_{key}";
        }
    }
}
