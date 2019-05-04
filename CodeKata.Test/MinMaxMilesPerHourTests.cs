using CodeKata.Filters;
using NUnit.Framework;

namespace CodeKata.Test
{
    [TestFixture]
    public class MinMaxMilesPerHourTests
    {
        private readonly MinMaxMilesPerHour _defaultMinMax = new MinMaxMilesPerHour(5, 100);

        [Test]
        public void happy_path()
        {
            Assert.AreEqual(true, _defaultMinMax.Match(MilesPerHour(60)));
        }

        [Test]
        public void too_fast()
        {
            Assert.AreEqual(false, _defaultMinMax.Match(MilesPerHour(110)));
        }

        [Test]
        public void too_slow()
        {
            Assert.AreEqual(false, _defaultMinMax.Match(MilesPerHour(2)));
        }

        [Test]
        public void upper_bound()
        {
            Assert.AreEqual(true, _defaultMinMax.Match(MilesPerHour(100)));
        }

        [Test]
        public void lower_bound()
        {
            Assert.AreEqual(true, _defaultMinMax.Match(MilesPerHour(5)));
        }

        private Trip MilesPerHour(int mph)
        {
            return new Trip("Ron", "07:15", "08:15", mph);
        }
    }
}