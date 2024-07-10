using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using System.Text.Json.Serialization;
using auth.models;
using auth.webapp.Services;
using Common.AuthMiddleware;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authorization;
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
        [AuthorizeClaim(Key = "scope", Value = "user.write")]
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

            return Ok(client);
        }

        [HttpPost("/add-scope")]
        [AuthorizeClaim(Key = "scope", Value = "user.write")]
        public async Task<ActionResult> AddAuthToClient([FromBody] ClientAuth auth)
        {
            var app = (OpenIddictEntityFrameworkCoreApplication)await appManager.FindByClientIdAsync(User.Identity.Name);
            if (app is null)
            {
                return NotFound();
            }

            var ret = await authManager.CreateAsync(new OpenIddictAuthorizationDescriptor
            {
                ApplicationId = app.Id,
                Status = OpenIddictConstants.Statuses.Valid,
                Subject = auth.Scope,
                Type = OpenIddictConstants.AuthorizationTypes.Permanent
            });
            return Ok(ret);
        }

        [HttpPost("/init")]
        public async Task<ActionResult> InitUser()
        {

            var client = (OpenIddictEntityFrameworkCoreApplication?)appManager.FindByClientIdAsync("test").Result;
            if (client is null)
            {
                client = (OpenIddictEntityFrameworkCoreApplication)await appManager.CreateAsync(new OpenIddictApplicationDescriptor
                {
                    ClientId = "test",
                    ClientSecret = "test",
                    DisplayName = "admin test",
                    Permissions = {
                        OpenIddictConstants.Permissions.Endpoints.Token,
                    OpenIddictConstants.Permissions.GrantTypes.ClientCredentials,
                    OpenIddictConstants.Permissions.GrantTypes.AuthorizationCode,
                    OpenIddictConstants.Permissions.GrantTypes.RefreshToken,

                    OpenIddictConstants.Permissions.Prefixes.Scope + "api"}
                });

                var res = await authManager.CreateAsync(new OpenIddictAuthorizationDescriptor
                {
                    ApplicationId = client.Id,
                    CreationDate = DateTime.Now,
                    Status = OpenIddictConstants.Statuses.Valid,
                    Subject = "user.write",
                    Type = OpenIddictConstants.AuthorizationTypes.Permanent,
                });
                return Ok();
            }
            return NoContent();
        }


        public record ClientAuth
        {
            [JsonPropertyName("client_id")]
            public required string ClientID;

            [JsonPropertyName("scope")]
            public required string Scope;
        }

    }
}