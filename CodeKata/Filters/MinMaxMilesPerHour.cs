using log4net;

namespace CodeKata.Filters
{
    /// <summary>
    /// Inspects Trip.MilesPerHour to see if falls within min and max bounds
    /// </summary>
    public class MinMaxMilesPerHour : IFilter<Trip>
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(MinMaxMilesPerHour));

        /*
        /// <summary>
        /// Min=5, Max=100
        /// </summary>
        public static MinMaxMilesPerHour Default { get; } = new MinMaxMilesPerHour(5, 100);
        */

        private readonly int _min;
        private readonly int _max;

        public MinMaxMilesPerHour(int min, int max)
        {
            _min = min;
            _max = max;
        }

        public bool Match(Trip trip)
        {
            // https://stackoverflow.com/questions/5343006/is-there-a-c-sharp-type-for-representing-an-integer-range
            if (trip.MilesPerHour < _min || trip.MilesPerHour > _max)
            {
                Log.Debug($"discarding trip because mph: {trip.MilesPerHour} is <{_min} or >{_max}"); // TODO: IsDebugEnabled
                return false; // design decision: filter is small enough for two return statements
            }
            return true;
        }
    }
}
