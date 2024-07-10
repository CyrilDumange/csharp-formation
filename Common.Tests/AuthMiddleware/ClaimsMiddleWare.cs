using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Common.AuthMiddleware;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Common.Tests.AuthMiddleware
{
    public class ClaimsMiddleWareTests
    {
        [Fact]
        public async Task TestSimple()
        {
            bool called = false;
            var builder = new WebHostBuilder()
                .UseEnvironment("Development")
                .ConfigureServices(services =>
                {
                    services.AddRouting();
                }).Configure(app =>
                {
                    app.UseRouting();
                    app.UseMiddleware<ClaimInjector>("test", "test");
                    app.UseMiddleware<AuthorizeClaimMiddleware>();
                    app.UseEndpoints(endpoints =>
                    {
                        endpoints.MapGet("/test",
                        [Claim(Key = "test", Value = "test")] () =>
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

        [Fact]
        public async Task TestLocked()
        {
            bool called = false;
            var builder = new WebHostBuilder()
                .UseEnvironment("Development")
                .ConfigureServices(services =>
                {
                    services.AddRouting();
                }).Configure(app =>
                {
                    app.UseRouting();
                    app.UseMiddleware<ClaimInjector>("test", "test");
                    app.UseMiddleware<AuthorizeClaimMiddleware>();
                    app.UseEndpoints(endpoints =>
                    {
                        endpoints.MapGet("/test",
                        [Claim(Key = "retest", Value = "test")] () =>
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

        [Fact]
        public async Task TestMultiple()
        {
            bool called = false;
            var builder = new WebHostBuilder()
                .UseEnvironment("Development")
                .ConfigureServices(services =>
                {
                    services.AddRouting();
                }).Configure(app =>
                {
                    app.UseRouting();
                    app.UseMiddleware<ClaimInjector>("test", "test");
                    app.UseMiddleware<AuthorizeClaimMiddleware>();
                    app.UseEndpoints(endpoints =>
                    {
                        endpoints.MapGet("/test",
                        [Claim(Key = "retest", Value = "retest")]
                        [Claim(Key = "test", Value = "test")]
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

        [Fact]
        public async Task TestContains()
        {
            bool called = false;
            var builder = new WebHostBuilder()
                .UseEnvironment("Development")
                .ConfigureServices(services =>
                {
                    services.AddRouting();
                }).Configure(app =>
                {
                    app.UseRouting();
                    app.UseMiddleware<ClaimInjector>("test", "test retest");
                    app.UseMiddleware<AuthorizeClaimMiddleware>();
                    app.UseEndpoints(endpoints =>
                    {
                        endpoints.MapGet("/test",
                        [Claim(Key = "test", Value = "test")]
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