using System.IO;
using log4net.Appender;
using log4net.Config;
using log4net.Layout;
using NUnit.Framework;

namespace CodeKata.Test
{
    [TestFixture]
    public class TripProcessorTests : BaseTest
    {
        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            ConsoleAppender appender = new ConsoleAppender { Layout = new PatternLayout() };
            appender.ActivateOptions();
            BasicConfigurator.Configure(appender);
        }

        [Test]
        public void happy_path()
        {
            ProcessAndAssertReport(
                input:
                @"Driver Dan
                Driver Alex
                Driver Bob
                Trip Dan 07:15 07:45 17.3
                Trip Dan 06:12 06:32 21.8
                Trip Alex 12:01 13:16 42.0",
                output:
                @"Alex: 42 miles @ 34 mph
                Dan: 39 miles @ 47 mph
                Bob: 0 miles");
        }

        [Test]
        public void invalid_file()
        {
            Assert.That(() => new TripProcessor().Process("file does not exist"), Throws.ArgumentException);
        }

        [Test]
        public void null_path()
        {
            Assert.That(() => new TripProcessor().Process((string)null), Throws.ArgumentNullException);
        }

        [Test]
        public void null_stream()
        {
            Assert.That(() => new TripProcessor().Process((Stream)null), Throws.ArgumentNullException);
        }

        [Test] // fun with C# 7 local functions!
        public void garbage_data()
        {
            TripResult P() => Process(
                @"Driver Dan
                Driver Alex
                HELLO
                WORLD
                Trip Dan 07:15 BAR
                Trip Dan FOO
                Trip Alex 12:01 13:16 BAZ");
            Assert.That(P, Throws.TypeOf<InvalidDataException>());
        }

        [Test] // fun with C# 7 local functions!
        public void garbage_trip_data_time()
        {
            TripResult P() => Process(
                @"Driver Alex                
                Trip Alex 12:00 HELLO 10");
            Assert.That(P, Throws.TypeOf<InvalidDataException>());
        }

        [Test] // fun with C# 7 local functions!
        public void garbage_trip_data_miles()
        {
            TripResult P() => Process(
                @"Driver Alex                
                Trip Alex 12:00 12:15 HELLO");
            Assert.That(P, Throws.TypeOf<InvalidDataException>());
        }

        [Test]
        public void too_fast()
        {
            ProcessAndAssertReport(
                input: // Alex's 110 row will be filtered out
                @"Driver Dan
                Driver Alex                
                Trip Alex 07:15 08:15 10
                Trip Alex 08:15 09:15 110
                Trip Alex 09:15 10:15 10
                Trip Dan 07:00 08:00 42.0",
                output:
                @"Dan: 42 miles @ 42 mph
                Alex: 20 miles @ 10 mph");
        }

        [Test]
        public void too_slow()
        {
            ProcessAndAssertReport(
                input: // Alex's 2 row will be filtered out
                @"Driver Dan
                Driver Alex
                Trip Alex 07:15 08:15 10
                Trip Alex 08:15 09:15 2
                Trip Alex 09:15 10:15 10
                Trip Dan 07:00 08:00 42.0",
                output:
                @"Dan: 42 miles @ 42 mph
                Alex: 20 miles @ 10 mph");
        }

        [Test]
        public void short_trip()
        {
            ProcessAndAssertReport(
                input:
                @"Driver Alex
                Trip Alex 07:15 07:16 0.1",
                output: // 0.1 miles * 60 minutes = 6 mph
                @"Alex: 0 miles @ 6 mph");
        }

        [Test]
        public void long_trip()
        {
            ProcessAndAssertReport(
                input: // 16 hour drive at 60 mph = 960 miles
                @"Driver Alex
                Trip Alex 07:00 23:00 960",
                output:
                @"Alex: 960 miles @ 60 mph");
        }

        [Test]
        public void sort_the_output_by_most_miles_drive_to_last()
        {
            ProcessAndAssertReport(
                input:
                @"Driver Alex
                Driver Dan
                Trip Dan 07:15 08:15 50
                Trip Dan 09:15 10:15 100
                Trip Alex 07:15 08:15 50",
                output:
                @"Dan: 150 miles @ 75 mph
                Alex: 50 miles @ 50 mph");
        }

        [Test]
        public void ignore_trips_without_drivers()
        {
            ProcessAndAssertReport(@"Trip Dan1 07:15 07:45 17.3", "");
        }

        [Test]
        public void round_down()
        {
            ProcessAndAssertReport( // .1 rounds down
                input:
                @"Driver Dan
                Trip Dan 07:15 08:15 17.1",
                output:
                @"Dan: 17 miles @ 17 mph");

            ProcessAndAssertReport( // .0 no rounding
                input:
                @"Driver Dan
                Trip Dan 07:15 08:15 17.0",
                output:
                @"Dan: 17 miles @ 17 mph");

            ProcessAndAssertReport( // no rounding
                input:
                @"Driver Dan
                Trip Dan 07:15 08:15 17",
                output:
                @"Dan: 17 miles @ 17 mph");
        }

        [Test]
        public void round_up()
        {
            ProcessAndAssertReport( // .9 rounds up
                input:
                @"Driver Dan
                Trip Dan 07:15 08:15 17.9",
                output:
                @"Dan: 18 miles @ 18 mph");

            ProcessAndAssertReport( // .5 rounds up
                input:
                @"Driver Dan
                Trip Dan 07:15 08:15 17.5",
                output:
                @"Dan: 18 miles @ 18 mph");
        }

        private void ProcessAndAssertReport(string input, string output)
        {
            var result = Process(input);
            Assert.AreEqual(Trim(output), result.ToReport().TrimEnd());
        }
    }
}