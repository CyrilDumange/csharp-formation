using fizzbuzz;
using fizzbuzz.models;
using Microsoft.AspNetCore.Mvc;

namespace webapi.Handlers
{
    public static class HandlerRegister
    {
        public static WebApplication RegisterEndpoints(this WebApplication app)
        {
            app.MapPost("/fizzbuzz", (IFizzbuzz fizz, [FromBody] FizzBuzzInput input) =>
            {
                return fizz.Compute(input);
            }).RequireAuthorization();

            app.MapPost("/fizzbuzz/historized", (IHistorizedFizzbuzz fizz, FizzBuzzInput input) =>
            {
                return fizz.Compute(input);
            }).RequireAuthorization();
            return app;
        }
    }
}