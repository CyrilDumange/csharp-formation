using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Common.Validators;

namespace Common.Tests.Validators
{
    public class NonZeroTests
    {
        [Theory]
        [InlineData(1, true)]
        [InlineData(0, false)]
        public void TestNonZeroInt(int value, bool expected)
        {
            var v = new TestValueInt(value);
            var result = new List<ValidationResult>();
            Assert.Equal(expected, Validator.TryValidateObject(v, new ValidationContext(v), result, true));
        }
    }

    internal record TestValueInt
    {
        public TestValueInt(int value)
        {
            Value = value;
        }

        [NonZero<int>]
        public int Value { get; init; }
    };
}