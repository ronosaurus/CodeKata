using CodeKata.Filters;
using CodeKata.Parsers;
using NUnit.Framework;

namespace CodeKata.Test
{
    [TestFixture]
    public class StringSplitTripParsersTests : BaseTest
    {
        [Test]
        public void happy_path()
        {
            var parser = new StringSplitTripParser(AsEnumerableTrim(
                @"Driver Alex
                Driver Bob
                Driver Dan
                Trip Alex 12:01 13:16 42.0"));
            parser.Parse(out var drivers, out var trips);
            Assert.AreEqual(3, drivers.Count);
            Assert.AreEqual(1, trips.Count);
        }

        [Test]
        public void with_filters()
        {
            var parser = new StringSplitTripParser(AsEnumerableTrim(
                @"Driver Alex
                Driver Bob
                Driver Dan
                Trip Alex 12:01 13:16 42.0
                Trip Bob 12:01 13:16 42.0"),
                new ExcludeDriver("Alex"),
                new ExcludeDriver("Bob"));
            parser.Parse(out var drivers, out var trips);
            Assert.AreEqual(3, drivers.Count);
            Assert.AreEqual(0, trips.Count);
        }

        public class ExcludeDriver : IFilter<Trip>
        {
            private readonly string _driver;

            public ExcludeDriver(string driver)
            {
                _driver = driver;
            }

            public bool Match(Trip value)
            {
                return value.Driver != _driver;
            }
        }
    }
}
