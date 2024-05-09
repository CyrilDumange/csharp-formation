using fizzbuzz;
using fizzbuzz.models;

namespace webapi.Handlers
{
    public static class HandlerRegister
    {
        public static WebApplication RegisterEndpoints(this WebApplication app)
        {
            app.MapPost("/fizzbuzz", (IFizzbuzz fizz, FizzBuzzInput input) =>
            {
                return fizz.Compute(input);
            });
            return app;
        }
    }
}