using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using fizzbuzz;
using fizzbuzz.dal;
using fizzbuzz.dal.Migrations;
using fizzbuzz.models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Npgsql;
using OpenIddict.Abstractions;
using OpenIddict.Validation.AspNetCore;
using webapi.Handlers;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IFizzbuzz, BasicFizzbuzz>();
builder.Services.AddTransient<IHistorizedFizzbuzz, HistorizedFizzbuzz>();
builder.Services.AddScoped<IHistory, History>();
builder.Services.AddTransient<IDbConnection>((sp) =>
    new NpgsqlConnection(builder.Configuration.GetConnectionString("public"))
);


/*builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = OpenIddictConstants.Schemes.Bearer;
            options.DefaultChallengeScheme = OpenIddictConstants.Schemes.Bearer;
            options.DefaultScheme = OpenIddictConstants.Schemes.Bearer;
        }).AddJwtBearer(options =>
        {
            options.BackchannelHttpHandler = new HttpClientHandler()
            {
                ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true
            };

            options.Authority = "https://localhost:7256/";
            options.Audience = "test";

            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = "https://localhost:7256/",
                ValidAudience = "test"
            };

        });*/
builder.Services.AddHttpClient("CustomHttpClient")
            .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
            });


builder.Services.AddOpenIddict()
    .AddValidation(options =>
    {

        options.SetIssuer("https://localhost:7256/");

        options.UseSystemNetHttp((config =>
        {
            config.ConfigureHttpClientHandler(c =>
            {
                c.ServerCertificateCustomValidationCallback = (HttpRequestMessage requestMessage,
                                                   X509Certificate2 certificate,
                                                   X509Chain chain,
                                                   SslPolicyErrors sslPolicyErrors) => true;
            });

        }));
        options.UseAspNetCore();
    });

builder.Services.AddAuthentication(OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme);

builder.Services.AddAuthorization(options =>
{
    options.DefaultPolicy = new AuthorizationPolicyBuilder(OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme)
                .RequireAuthenticatedUser()
                .RequireClaim("test", "claim.me.write")
                .Build();
});


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var app = builder.Build();

FizzHistoryMigration.Apply(app.Services.GetRequiredService<IDbConnection>()).Wait();

app.UseAuthentication();
app.UseAuthorization();

app.RegisterEndpoints();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();


app.Run();
