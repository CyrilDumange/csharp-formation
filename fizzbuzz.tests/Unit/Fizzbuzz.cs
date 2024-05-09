namespace fizzbuzz.tests;
using fizzbuzz;

public class UnitTest1
{
    [Theory]
    [InlineData(2, 3, "fizz", "buzz", 6, new string[]{"1","fizz", "buzz", "fizz", "5", "fizzbuzz"})]
    public void Test_Ok(int Int1, int Int2, string String1, string String2, int limit, string[] expected)
    {
        var fizz = new BasicFizzbuzz();
        var output = fizz.Compute(new FizzBuzzInput(Int1, Int2, String1, String2, limit));

        Assert.Equal(expected, output.Values);
    }
}