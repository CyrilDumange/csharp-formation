using System.ComponentModel.DataAnnotations;
using fizzbuzz;
using fizzbuzz.models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace webapi.Handlers
{
    public static class HandlerRegister
    {
        public static WebApplication RegisterEndpoints(this WebApplication app)
        {
            app.MapPost("/fizzbuzz", IResult (IFizzbuzz fizz, FizzBuzzInput input) =>
            {
                if (!Validator.TryValidateObject(input, new ValidationContext(input), null, true))
                {
                    return Results.BadRequest(null);
                }
                return Results.Ok(fizz.Compute(input));

            });

            app.MapPost("/fizzbuzz/historized", (IHistorizedFizzbuzz fizz, FizzBuzzInput input) =>
            {
                return fizz.Compute(input);
            });
            return app;
        }
    }
}