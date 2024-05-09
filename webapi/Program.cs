using System.Data;
using fizzbuzz;
using fizzbuzz.dal;
using fizzbuzz.dal.Migrations;
using fizzbuzz.models;
using Npgsql;
using webapi.Handlers;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IFizzbuzz, BasicFizzbuzz>();
builder.Services.AddTransient<IHistorizedFizzbuzz, HistorizedFizzbuzz>();
builder.Services.AddScoped<IHistory, History>();
builder.Services.AddTransient<IDbConnection>((sp) =>
    new NpgsqlConnection(builder.Configuration.GetConnectionString("public"))
);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var app = builder.Build();

FizzHistoryMigration.Apply(app.Services.GetRequiredService<IDbConnection>()).Wait();

app.RegisterEndpoints();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();


app.Run();
