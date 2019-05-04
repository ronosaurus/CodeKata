using NUnit.Framework;

namespace CodeKata.Test
{
    [TestFixture]
    class TripResultTests : BaseTest
    {
        [Test]
        public void zero_miles()
        {
            var result = Process("Driver Bob");
            Assert.AreEqual(1, result.ZeroMilesDrivers.Count);

            result = Process(@"
                Driver Bob1
                Driver Bob2");
            Assert.AreEqual(2, result.ZeroMilesDrivers.Count);
        }

        [Test]
        public void miles_per_hour()
        {
            TripResult.TripTotal total = new TripResult.TripTotal {Driver = "Ron", Miles = 60, Hours = 1};
            Assert.AreEqual(60, total.MilesPerHour);
        }

        [Test]
        public void very_slow()
        {
            TripResult.TripTotal total = new TripResult.TripTotal { Driver = "Ron", Miles = 1, Hours = 10 };
            Assert.AreEqual(1/(decimal)10, total.MilesPerHour);
        }
    }
}
