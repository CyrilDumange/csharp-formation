using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Common.Results;
using JOS.Result;
using Microsoft.IdentityModel.Tokens;
using OpenIddict.Abstractions;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace auth.webapp.Services
{

    public interface IAuthService
    {
        Task<Result<ClaimsPrincipal>> GetToken(OpenIddictRequest req);
    }

    public class AuthService(IOpenIddictApplicationManager appManager) : IAuthService
    {
        public async Task<Result<ClaimsPrincipal>> GetToken(OpenIddictRequest req)
        {
            if (req.ClientId is null)
            {
                return Result.Failure<ClaimsPrincipal>(new Error(ErrorTypes.BadRequest, "client_id was not passed"));
            }

            var client = await appManager.FindByClientIdAsync(req.ClientId);
            if (client is null)
            {
                return Result.Failure<ClaimsPrincipal>(new Error(ErrorTypes.NoData, "could not find client"));
            }
            var identity = new ClaimsIdentity(TokenValidationParameters.DefaultAuthenticationType, Claims.Name, Claims.Role);

            identity.SetClaim(Claims.Subject, req.ClientId);
            identity.SetClaim(Claims.Name, "test");

            return Result.Success(new ClaimsPrincipal(identity));
        }
    }
}