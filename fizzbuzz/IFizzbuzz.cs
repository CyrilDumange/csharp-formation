
using fizzbuzz.models;

namespace fizzbuzz
{
    public interface IFizzbuzz
    {
        public FizzbuzzOutput Compute(FizzBuzzInput input);
    }

    public interface IHistorizedFizzbuzz
    {
        public Task<FizzbuzzOutput> Compute(FizzBuzzInput input);
    }
}