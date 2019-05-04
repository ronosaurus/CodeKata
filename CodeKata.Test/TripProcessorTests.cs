using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CodeKata.Filters;
using CodeKata.Parsers;
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
                expectedOutput:
                @"Alex: 42 miles @ 34 mph
                Dan: 39 miles @ 47 mph
                Bob: 0 miles");
        }

        [Test]
        public void null_parser()
        {
            Assert.That(() => new TripProcessor(null).Process(), Throws.ArgumentNullException);
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
                expectedOutput:
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
                expectedOutput:
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
                expectedOutput: // 0.1 miles * 60 minutes = 6 mph
                @"Alex: 0 miles @ 6 mph");
        }

        [Test]
        public void long_trip()
        {
            ProcessAndAssertReport(
                input: // 16 hour drive at 60 mph = 960 miles
                @"Driver Alex
                Trip Alex 07:00 23:00 960",
                expectedOutput:
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
                expectedOutput:
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
                expectedOutput:
                @"Dan: 17 miles @ 17 mph");

            ProcessAndAssertReport( // .0 no rounding
                input:
                @"Driver Dan
                Trip Dan 07:15 08:15 17.0",
                expectedOutput:
                @"Dan: 17 miles @ 17 mph");

            ProcessAndAssertReport( // no rounding
                input:
                @"Driver Dan
                Trip Dan 07:15 08:15 17",
                expectedOutput:
                @"Dan: 17 miles @ 17 mph");
        }

        [Test]
        public void round_up()
        {
            ProcessAndAssertReport( // .9 rounds up
                input:
                @"Driver Dan
                Trip Dan 07:15 08:15 17.9",
                expectedOutput:
                @"Dan: 18 miles @ 18 mph");

            ProcessAndAssertReport( // .5 rounds up
                input:
                @"Driver Dan
                Trip Dan 07:15 08:15 17.5",
                expectedOutput:
                @"Dan: 18 miles @ 18 mph");
        }

        [Test] // make sure the general design works ok with things other than StringSplitTripParser
        public void alternate_implementation()
        {
            /*
                Driver Dan
                Driver Alex
                Driver Bob
                Trip Dan 07:15 07:45 17.3
                Trip Dan 06:12 06:32 21.8
                Trip Alex 12:01 13:16 42.0
            */
            var expected = Trim(@"
                Alex: 42 miles @ 34 mph
                Dan: 39 miles @ 47 mph
                Bob: 0 miles");

            var filter = new MinMaxMilesPerHour(5, 100);
            AltTripParser parser = new AltTripParser(filter)
            {
                AltDrivers = {"Dan", "Alex", "Bob"},
                AltTrips =
                {
                    new Trip("Dan", "07:15", "07:45", 17.3m),
                    new Trip("Dan", "06:12", "06:32", 21.8m),
                    new Trip("Alex", "12:01", "13:16", 42.0m)
                }
            };
            var actual = new TripProcessor(parser).Process().ToReport().TrimEnd();

            Assert.AreEqual(expected, actual);
        }

        private class AltTripParser : BaseFilteringParser
        {
            public HashSet<string> AltDrivers { get; } = new HashSet<string>();
            public List<Trip> AltTrips { get; } = new List<Trip>();

            public AltTripParser(params IFilter<Trip>[] tripFilters) : base(tripFilters)
            {
                // empty
            }

            protected override void Parse()
            {
                AltDrivers.ToList().ForEach(AddDriver);
                AltTrips.ForEach(AddTrip);
            }
        }

        private void ProcessAndAssertReport(string input, string expectedOutput)
        {
            var result = Process(input, new MinMaxMilesPerHour(5, 100));
            Assert.AreEqual(Trim(expectedOutput), result.ToReport().TrimEnd());
        }
    }
}