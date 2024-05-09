

using fizzbuzz.models;

namespace fizzbuzz;


public class BasicFizzbuzz : IFizzbuzz
{
    public virtual FizzbuzzOutput Compute(FizzBuzzInput input)
    {
        if (input.Limit == 0)
        {
            return new FizzbuzzOutput([]);
        }
        var values = new string[input.Limit];

        if (input.Int1 == 0 || input.Int2 == 0)
        {
            throw new Exception("neither int1 nor int2 should be 0");
        }

        for (int i = 1; i <= input.Limit; i++)
        {
            var divisible1 = (i % input.Int1) == 0;
            var divisible2 = (i % input.Int2) == 0;

            if (divisible1 && divisible2)
            {
                values[i - 1] = input.Str1 + input.Str2;
                continue;
            }

            if (divisible1)
            {
                values[i - 1] = input.Str1;
                continue;
            }

            if (divisible2)
            {
                values[i - 1] = input.Str2;
                continue;
            }

            values[i - 1] = i.ToString();

        }
        return new FizzbuzzOutput(values);
    }
}
