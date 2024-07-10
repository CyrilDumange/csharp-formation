using System.Net;
using System.Runtime.ExceptionServices;
using Microsoft.AspNetCore.Http;

namespace Common.AuthMiddleware
{
    public class AuthorizeClaimMiddleware(RequestDelegate _next)
    {
        public async Task Invoke(HttpContext httpContext)
        {
            var endpoint = httpContext.GetEndpoint();
            var attributes = endpoint?.Metadata.GetOrderedMetadata<ClaimAttribute>();

            if (attributes == null || attributes.Count == 0)
            {
                await _next(httpContext);
                return;
            }

            foreach (var att in attributes)
            {
                var c = httpContext.User.Claims.FirstOrDefault(x => x.Type == att.Key);
                if (c is not null && c.Value.Contains(att.Value))
                {
                    await _next(httpContext);
                    return;
                }
            }

            httpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;
            return;
        }

    }
}