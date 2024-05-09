using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using fizzbuzz.dal;
using fizzbuzz.models;
using NSubstitute;

namespace fizzbuzz.tests.Unit
{
    public class HistorizedFizzbuzTests
    {
        [Fact]
        public async void TestCompute()
        {
            // Given
            var i = new FizzBuzzInput(1, 2, "test", "retest", 100);
            var expected = new string[0];

            var fizz = Substitute.For<IFizzbuzz>();
            fizz.Compute(i).Returns(new FizzbuzzOutput(expected));

            var history = Substitute.For<IHistory>();

            var f = new HistorizedFizzbuzz(history, fizz);

            // When

            var res = await f.Compute(i);

            // Then

            Assert.Equal(res.Values, expected);
            await history.Received().Count(i);
        }
    }
}