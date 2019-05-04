using System;
using System.Collections.Generic;
using CodeKata.Filters;

namespace CodeKata.Parsers
{
    // code review: not sure if this should be base class or wrap an implementation; goal was to filter during first pass
    public abstract class BaseFilteringParser : ITripParser
    {
        protected HashSet<string> Drivers = new HashSet<string>();
        protected List<Trip> Trips = new List<Trip>();

        // https://stackoverflow.com/questions/2550892/design-patterns-recommendation-for-filtering-option
        // https://gist.github.com/craigles/8553239
        protected readonly IFilter<Trip>[] TripFilters;
        protected readonly IFilter<string>[] DriverFilters;

        protected BaseFilteringParser(IFilter<Trip>[] tripFilters, IFilter<string>[] driverFilters = null)
        {
            TripFilters = tripFilters;
            DriverFilters = driverFilters;
        }

        public void Parse(out ISet<string> drivers, out IList<Trip> trips)
        {
            Parse();
            drivers = Drivers;
            trips = Trips;
        }

        protected abstract void Parse();

        protected void AddDriver(string driver)
        {
            if (Match(DriverFilters, driver))
            {
                Drivers.Add(driver);
            }
        }

        protected void AddTrip(Trip trip)
        {
            if (Match(TripFilters, trip))
            {
                Trips.Add(trip);
            }
        }

        private bool Match<T>(IFilter<T>[] array, T item)
        {
            return array == null || Array.TrueForAll(array, x => x.Match(item));
        }
    }
}
