using System.ComponentModel.DataAnnotations;
using Common.Validators;

namespace fizzbuzz.models;

public class FizzBuzzInput
{

    [Required]
    public int Int1 { get; set; }
    [NonZero<int>]
    public int Int2 { get; set; }
    public string Str1 { get; set; }
    public string Str2 { get; set; }
    public int Limit { get; set; }
}


public record FizzbuzzOutput(string[] Values) { }