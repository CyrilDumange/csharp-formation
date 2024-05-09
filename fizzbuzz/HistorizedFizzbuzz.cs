using fizzbuzz.dal;
using fizzbuzz.models;

namespace fizzbuzz
{
    public class HistorizedFizzbuzz(IHistory history, IFizzbuzz fizzbuzz) : IHistorizedFizzbuzz
    {
        public async Task<FizzbuzzOutput> Compute(FizzBuzzInput input)
        {
            await history.Count(input);
            return fizzbuzz.Compute(input);
        }
    }
}