using System.Collections.Generic;
using System.Linq;
using CodeKata.Filters;

namespace CodeKata.Parsers
{
    // code review: not sure if this should be base class or wrap an implementation; goal was to filter during first pass
    public abstract class BaseFilteringParser : ITripParser
    {
        private readonly ISet<string> _drivers = new HashSet<string>();
        private readonly IList<Trip> _trips = new List<Trip>();

        protected readonly IEnumerable<IFilter<Trip>> TripFilters;
        protected readonly IEnumerable<IFilter<string>> DriverFilters;

        protected BaseFilteringParser(IEnumerable<IFilter<Trip>> tripFilters = null, IEnumerable<IFilter<string>> driverFilters = null)
        {
            TripFilters = tripFilters;
            DriverFilters = driverFilters;
        }

        public void Parse(out ISet<string> drivers, out IList<Trip> trips)
        {
            Parse();
            drivers = _drivers;
            trips = _trips;
        }

        protected abstract void Parse();

        protected void AddDriver(string driver)
        {
            if (Match(DriverFilters, driver))
            {
                _drivers.Add(driver);
            }
        }

        protected void AddTrip(Trip trip)
        {
            if (Match(TripFilters, trip))
            {
                _trips.Add(trip);
            }
        }

        private bool Match<T>(IEnumerable<IFilter<T>> items, T item)
        {
            return items == null || items.All(x => x.Match(item));
        }
    }
}
