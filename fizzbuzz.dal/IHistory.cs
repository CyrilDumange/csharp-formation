using fizzbuzz.models;

namespace fizzbuzz.dal
{
    public interface IHistory
    {
        Task Count(FizzBuzzInput input);
        Task<int> GetCount(FizzBuzzInput input);
    }
}