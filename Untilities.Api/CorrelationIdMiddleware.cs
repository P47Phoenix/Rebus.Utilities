using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace Utilities.Api
{
    public class CorrelationIdMiddleware
    {
        private readonly RequestDelegate _next;

        private const string 
            XCorrelationId = "X-CorrelationId";

        public CorrelationIdMiddleware(RequestDelegate next)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
        }

        public async Task Invoke(HttpContext context)
        {
            if (context.Request.Headers.TryGetValue(XCorrelationId, out StringValues correlationId))
            {
                context.TraceIdentifier = correlationId;
            }
            
            
            // apply the correlation ID to the response header for client side tracking
            context.Response.OnStarting(() =>
            {
                context.Response.Headers.Add(XCorrelationId, new[] { context.TraceIdentifier });
                return Task.CompletedTask;
            });

            await _next(context);            
        }
    }
}
