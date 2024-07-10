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
                if (httpContext.User.HasClaim(att.Key, att.Value))
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