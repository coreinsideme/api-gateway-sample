using Microsoft.AspNetCore.Http;
using ApiGateway.Models;
using System.Threading.Tasks;
using System;
using Microsoft.Extensions.Primitives;

namespace ApiGateway.Middlewares
{
    public class CorrelationIdMiddleware
    {
        private readonly CorrelationIdOptions _correlationIdOptions = new CorrelationIdOptions();
        private readonly RequestDelegate _next;

        public CorrelationIdMiddleware(RequestDelegate next)
        {
            ArgumentNullException.ThrowIfNull(next, nameof(next));
            _next = next;
        }

        public Task Invoke(HttpContext context)
        {
            if (context.Request.Headers.TryGetValue(_correlationIdOptions.Header, out StringValues correlationId))
            {
                context.TraceIdentifier = correlationId;
            }

            if (_correlationIdOptions.InResponse)
            {
                // apply the correlation ID to the response header for client side tracking
                context.Response.OnStarting(() =>
                {
                    context.Response.Headers.Add(_correlationIdOptions.Header, new[] { context.TraceIdentifier });
                    return Task.CompletedTask;
                });
            }

            return _next(context);
        }
    }
}
