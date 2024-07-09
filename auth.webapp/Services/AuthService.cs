using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Common.Results;
using JOS.Result;
using Microsoft.IdentityModel.Tokens;
using OpenIddict.Abstractions;
using OpenIddict.EntityFrameworkCore.Models;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace auth.webapp.Services
{

    public interface IAuthService
    {
        Task<Result<ClaimsPrincipal>> GetToken(OpenIddictRequest req);
    }

    public class AuthService(
        IOpenIddictApplicationManager appManager,
        IOpenIddictAuthorizationManager authManager
        ) : IAuthService
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


            var appID = await appManager.GetIdAsync(client);
            if (appID is null)
            {
                return Result.Failure<ClaimsPrincipal>(new Error(ErrorTypes.Internal, "client has no ID ?"));
            }

            var identity = new ClaimsIdentity(TokenValidationParameters.DefaultAuthenticationType, Claims.Name, Claims.Role);

            identity.SetClaim(Claims.Subject, req.ClientId);
            identity.SetClaim(Claims.Name, "test");
            identity.SetClaim(Claims.Audience, "test");

            string claimValue = string.Empty;

            await foreach (var c in authManager.FindByApplicationIdAsync(appID))
            {
                claimValue += ((OpenIddictEntityFrameworkCoreAuthorization)c).Subject;
            }
            identity.SetClaim(Claims.Scope, claimValue);

            foreach (Claim claim in identity.Claims)
            {
                claim.SetDestinations(OpenIddictConstants.Destinations.AccessToken, OpenIddictConstants.Destinations.IdentityToken);
            }

            return Result.Success(new ClaimsPrincipal(identity));
        }
    }
}