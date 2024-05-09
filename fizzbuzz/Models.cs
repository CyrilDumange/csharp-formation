using Microsoft.VisualBasic;

namespace fizzbuzz;

public record FizzBuzzInput(
    int Int1,
    int Int2,
    string Str1,
    string Str2,
    int Limit
)
{ }


public record FizzbuzzOutput(string[] Values) { }