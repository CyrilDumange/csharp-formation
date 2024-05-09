using fizzbuzz;
using fizzbuzz.models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IFizzbuzz, BasicFizzbuzz>();
builder.Services.AddScoped<IFizzbuzz, BasicFizzbuzz>();
builder.Services.AddTransient<IFizzbuzz, BasicFizzbuzz>();


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapPost("/fizzbuzz", (IFizzbuzz fizz, FizzBuzzInput input) =>
{
    return fizz.Compute(input);
});

app.Run();
