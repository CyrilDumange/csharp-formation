using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace fizzbuzz.dal
{
    public interface IHistory
    {
        Task Count(FizzBuzzInput input);
        Task<int> GetCount(FizzBuzzInput input);
    }
}