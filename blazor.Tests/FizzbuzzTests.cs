using blazor.Components.Pages;
using Bunit;
using fizzbuzz;
using fizzbuzz.models;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Xunit;

public class FizzbuzzTests
{
    [Fact]
    public void Simple()
    {
        var output = new FizzbuzzOutput(new string[] {
            "1"
        });
        using var ctx = new TestContext();
        var mockHistorized = Substitute.For<IHistorizedFizzbuzz>();
        mockHistorized.Compute(Arg.Any<FizzBuzzInput>())
            .Returns(Task.FromResult(output));

        ctx.Services.AddSingleton(mockHistorized);

        var rendered = ctx.RenderComponent<Fizzbuzz>();
        var bt = rendered.Find("#fetchfizz");

        bt.Click();

        var res = rendered.Find("ul");
        Assert.Equal(res.Children.Length, output.Values.Length);
    }
}

