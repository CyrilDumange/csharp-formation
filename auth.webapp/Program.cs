using auth.models;
using auth.webapp.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

builder.Services.AddDbContext<AuthContext>(
    options =>
    {
        options.UseNpgsql(builder.Configuration.GetConnectionString("auth"));
        options.UseOpenIddict();
    });


builder.Services.AddOpenIddict().AddCore(options =>
{
    // Configure OpenIddict to use the Entity Framework Core stores and models.
    // Note: call ReplaceDefaultEntities() to replace the default entities.
    options.UseEntityFrameworkCore()
            .UseDbContext<DbContext>();
});

builder.Services.AddOpenIddict()

    // Register the OpenIddict server components.
    .AddServer(options =>
    {
        // Enable the token endpoint.
        options.SetTokenEndpointUris("connect/token");
        options.SetAuthorizationEndpointUris("connect/authorization");

        // Enable the client credentials flow.
        options.AllowClientCredentialsFlow();
        options.AllowRefreshTokenFlow();
        options.AllowAuthorizationCodeFlow();

        // Register the signing and encryption credentials.
        options.AddDevelopmentEncryptionCertificate()
               .AddDevelopmentSigningCertificate();

        // Register the ASP.NET Core host and configure the ASP.NET Core options.
        options.UseAspNetCore()
               .EnableTokenEndpointPassthrough();
    });

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = IdentityConstants.BearerScheme;
    options.DefaultChallengeScheme = IdentityConstants.BearerScheme;
    options.DefaultScheme = IdentityConstants.BearerScheme;
}).AddCookie(IdentityConstants.BearerScheme);

builder.Services.AddAuthorization();
builder.Services.AddIdentityCore<AuthUser>()
    .AddEntityFrameworkStores<AuthContext>()
    .AddDefaultTokenProviders()
    .AddApiEndpoints();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.MapIdentityApi<AuthUser>();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.Run();
