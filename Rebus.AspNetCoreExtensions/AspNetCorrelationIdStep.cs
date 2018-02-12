using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Rebus.Messages;
using Rebus.Pipeline;

namespace Rebus.AspNetCoreExtensions
{
    [StepDocumentation("This step sets the correlation id on the message to match the TraceIdentifier on the incoming request")]
    public class AspNetCorrelationIdStep : IOutgoingStep
    {
        private readonly IHttpContextAccessor m_httpContextAccessor;

        public AspNetCorrelationIdStep(IHttpContextAccessor httpContextAccessor)
        {
            m_httpContextAccessor = httpContextAccessor;
        }

        public async Task Process(OutgoingStepContext outgoingStepContext, Func<Task> next)
        {
            HttpContext httpContext = m_httpContextAccessor.HttpContext;

            if (httpContext != null)
            {
                var messsage = outgoingStepContext.Load<Message>();

                messsage.Headers[Headers.CorrelationId] = httpContext.TraceIdentifier;
            }

            await next();
        }
    }
}
