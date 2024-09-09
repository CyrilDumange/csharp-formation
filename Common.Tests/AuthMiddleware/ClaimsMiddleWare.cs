using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Common.AuthMiddleware;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace Common.Tests.AuthMiddleware
{
    public class ClaimsMiddleWareTests
    {
        public async Task TestSimple()
        {
            bool called = false;
            var builder = new WebHostBuilder()
                .UseEnvironment("Development")
                .ConfigureServices(services =>
                {
                    services.AddRouting();
                    services.AddAuthentication(options =>
                    {
                        options.DefaultAuthenticateScheme = "Bearer";
                        options.DefaultChallengeScheme = "Bearer";
                    });
                    services.AddAuthorization();
                }).Configure(app =>
                {
                    app.UseRouting();
                    app.UseAuthentication();
                    app.UseAuthorization();
                    app.UseMiddleware<ClaimInjector>("test", "test");
                    app.UseMiddleware<AuthorizeClaimMiddleware>();
                    app.UseEndpoints(endpoints =>
                    {
                        endpoints.MapGet("/test",
                        [AuthorizeClaim(Key = "test", Value = "test")] () =>
                        {
                            called = true;
                        }).AllowAnonymous();
                    });
                });
            using var server = new TestServer(builder);
            var client = server.CreateClient();

            var req = new HttpRequestMessage(HttpMethod.Get, "/test");
            req.Headers.Add(
                "Authorization",
                "Bearer eyJhbGciOiJSUzI1NiIsImtpZCI6IkIyMzE5QzgwNTdDMUVGRUNGRjNGM0ZCMUMxNTE1RjUxODY4OEFENEMiLCJ4NXQiOiJzakdjZ0ZmQjctel9Qei14d1ZGZlVZYUlyVXciLCJ0eXAiOiJhdCtqd3QifQ.eyJpc3MiOiJodHRwczovL2xvY2FsaG9zdDo3MjU2LyIsImV4cCI6MTcyNTU0NTk0MiwiaWF0IjoxNzI1NTQyMzQyLCJqdGkiOiI3YzIxZjEyNS0zNThjLTQ3MzgtYWIwYy0yOTExZTZiNDlhMjgiLCJzdWIiOiJ0ZXN0IiwibmFtZSI6InRlc3QiLCJhdWQiOiJ0ZXN0Iiwic2NvcGUiOiJ1c2VyLndyaXRlIiwib2lfcHJzdCI6InRlc3QiLCJjbGllbnRfaWQiOiJ0ZXN0Iiwib2lfdGtuX2lkIjoiMmZkMTM5YjUtNjcwZC00YjNjLTlkMGQtZTgzNDNhMzNjMWZhIn0.i60iA4v7zgzm4b9IlfHCvcRooCi-AAxQUpaS6sFgSZhtOiz-y3IORycOD6RhosDi-McZOREYNgSBwSXqObFJRB_jX9mZgMJSKEA0wNcRdcc4JyPkG-Fs52rVQiLayBwsc_qOK-bAl50bJX6REq_Vgl6oeOl9jC-I6NWqIuby6QnFR4bA_yGO4RBdvNrq5z82IJnHVsjODXnhynIB-iK__VoDN6L0v88ax1bHSiUDpOzHciytCyKvEG3Ocbj_oH-2cIm5fa12q7u5J008KWYiW5U7lndnl9sBz1LidpT0RYNL5a-OVun52-TXA_Mi9_FhD0wYKHNYgXlG5o0k8XLdjw"
                );

            var res = await client.SendAsync(req);
            Assert.Equal(System.Net.HttpStatusCode.OK, res.StatusCode);
            Assert.True(called);
        }

        public async Task TestLocked()
        {
            bool called = false;
            var builder = new WebHostBuilder()
                .UseEnvironment("Development")
                .ConfigureServices(services =>
                {
                    services.AddRouting();
                    services.AddAuthorization();
                    services.AddAuthentication();
                }).Configure(app =>
                {
                    app.UseRouting();
                    app.UseMiddleware<ClaimInjector>("test", "test");
                    app.UseAuthorization();
                    app.UseAuthentication();
                    app.UseMiddleware<AuthorizeClaimMiddleware>();
                    app.UseEndpoints(endpoints =>
                    {
                        endpoints.MapGet("/test",
                        [AuthorizeClaim(Key = "retest", Value = "test")] () =>
                        {
                            called = true;
                        });
                    });
                });
            using var server = new TestServer(builder);
            var client = server.CreateClient();

            var res = await client.GetAsync("/test");
            Assert.Equal(System.Net.HttpStatusCode.Forbidden, res.StatusCode);
            Assert.False(called);
        }

        public async Task TestMultiple()
        {
            bool called = false;
            var builder = new WebHostBuilder()
                .UseEnvironment("Development")
                .ConfigureServices(services =>
                {
                    services.AddRouting();
                    services.AddAuthorization();
                    services.AddAuthentication();
                }).Configure(app =>
                {
                    app.UseRouting();
                    app.UseAuthorization();
                    app.UseAuthentication();
                    app.UseMiddleware<ClaimInjector>("test", "test");
                    app.UseMiddleware<AuthorizeClaimMiddleware>();
                    app.UseEndpoints(endpoints =>
                    {
                        endpoints.MapGet("/test",
                        [AuthorizeClaim(Key = "retest", Value = "retest")]
                        [AuthorizeClaim(Key = "test", Value = "test")]
                        () =>
                        {
                            called = true;
                        });
                    });
                });
            using var server = new TestServer(builder);
            var client = server.CreateClient();

            var res = await client.GetAsync("/test");
            Assert.Equal(System.Net.HttpStatusCode.OK, res.StatusCode);
            Assert.True(called);
        }

        public async Task TestContains()
        {
            bool called = false;
            var builder = new WebHostBuilder()
                .UseEnvironment("Development")
                .ConfigureServices(services =>
                {
                    services.AddRouting();
                    services.AddAuthorization();
                    services.AddAuthentication();
                }).Configure(app =>
                {
                    app.UseRouting();
                    app.UseAuthorization();
                    app.UseAuthentication();
                    app.UseMiddleware<ClaimInjector>("test", "test retest");
                    app.UseMiddleware<AuthorizeClaimMiddleware>();
                    app.UseEndpoints(endpoints =>
                    {
                        endpoints.MapGet("/test",
                        [AuthorizeClaim(Key = "test", Value = "test")]
                        () =>
                        {
                            called = true;
                        });
                    });
                });
            using var server = new TestServer(builder);
            var client = server.CreateClient();

            var res = await client.GetAsync("/test");
            Assert.Equal(System.Net.HttpStatusCode.OK, res.StatusCode);
            Assert.True(called);
        }
    }

    public class ClaimInjector(RequestDelegate _next, string key, string value)
    {
        public async Task Invoke(HttpContext httpContext)
        {
            var identity = new ClaimsIdentity(new List<Claim> {
                new Claim(key, value)
            });

            httpContext.User.AddIdentity(identity);
            await _next(httpContext);
        }
    }
}