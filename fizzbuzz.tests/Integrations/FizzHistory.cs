
using fizzbuzz.dal;
using fizzbuzz.models;

namespace fizzbuzz.tests.Integrations
{
    public class FizzHistory : IAsyncLifetime
    {
        private readonly DBFixture docker = new("fizzHistory");
        public async Task DisposeAsync()
        {
            await docker.DisposeAsync();
        }

        [Fact]
        public async void TestUpsert()
        {
            var h = new History(await docker.GetConnection());
            var i = new FizzBuzzInput(1, 2, "test", "retest", 100);

            await h.Count(i);

            Assert.Equal(1, await h.GetCount(i));
        }

        [Fact]
        public async void TestUpsertOnlyOneLine()
        {
            var h = new History(await docker.GetConnection());
            var i1 = new FizzBuzzInput(1, 2, "test", "retest", 100);
            var i2 = new FizzBuzzInput(1, 3, "test", "retest", 100);


            await h.Count(i1);
            await h.Count(i2);
            await h.Count(i2);

            Assert.Equal(1, await h.GetCount(i1));
            Assert.Equal(2, await h.GetCount(i2));
        }

        public async Task InitializeAsync()
        {
            await docker.InitContainer();
        }
    }
}