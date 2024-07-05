using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using System.Text.Json.Serialization;
using auth.models;
using auth.webapp.Services;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using OpenIddict.Abstractions;
using OpenIddict.EntityFrameworkCore.Models;
using OpenIddict.Server.AspNetCore;

namespace auth.webapp.Controllers
{
    public record User(
        [property: JsonPropertyName("client_id")] string ClientID,
        [property: JsonPropertyName("client_secret")] string ClientSecret);

    public class AuthController(
        IOpenIddictApplicationManager appManager,
        IOpenIddictAuthorizationManager authManager,
        IAuthService auth) : Controller
    {
        [HttpPost("/connect/token"), IgnoreAntiforgeryToken]
        public async Task<ActionResult> GenerateToken()
        {
            var request = HttpContext.GetOpenIddictServerRequest();
            if (request is null)
            {
                return BadRequest();
            }

            var cred = await auth.GetToken(request);

            if (cred.Succeeded)
            {

                return SignIn(cred.Data, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
            }

            return BadRequest();
        }

        [HttpPost("/create"), IgnoreAntiforgeryToken]
        public async Task<ActionResult> CreateClient([FromBody] User user)
        {
            var existing = await appManager.FindByClientIdAsync(user.ClientID);
            if (existing != null)
            {
                return BadRequest();
            }

            var client = (OpenIddictEntityFrameworkCoreApplication)await appManager.CreateAsync(new OpenIddictApplicationDescriptor
            {
                ClientId = user.ClientID,
                ClientSecret = user.ClientSecret,
                Permissions =
                    {
                        OpenIddictConstants.Permissions.Endpoints.Token,
                        OpenIddictConstants.Permissions.GrantTypes.ClientCredentials,
                        OpenIddictConstants.Permissions.GrantTypes.AuthorizationCode,
                        OpenIddictConstants.Permissions.GrantTypes.RefreshToken,

                        OpenIddictConstants.Permissions.Prefixes.Scope + "api"
                    },
            });

            var token = await auth.GetToken(new OpenIddictRequest
            {
                ClientId = client.ClientId,
                ClientSecret = client.ClientSecret
            });

            var a = (OpenIddictEntityFrameworkCoreAuthorization)await authManager.CreateAsync(new OpenIddictAuthorizationDescriptor
            {
                ApplicationId = client.Id,
                CreationDate = DateTime.Now,
                Status = OpenIddictConstants.Statuses.Valid,
                Subject = "claim.me.read",
                Type = OpenIddictConstants.AuthorizationTypes.Permanent,
            });

            return Ok(client);
        }
    }
}