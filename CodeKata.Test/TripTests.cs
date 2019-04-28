using NUnit.Framework;

namespace CodeKata.Test
{
    [TestFixture]
    public class TripTests
    {
        [Test]
        public void happy_path()
        {
            Trip trip = new Trip("Ron", "07:15", "07:30", 1);
            Assert.AreEqual(15, trip.Elapsed.Minutes);
            Assert.AreEqual(1, trip.Miles);
        }

        [Test] // internal data is not rounded
        public void only_round_on_report()
        {
            Trip trip = new Trip("Ron", "07:15", "07:30", 0.9m);
            Assert.AreEqual(0.9m, trip.Miles);
        }
    }
}
