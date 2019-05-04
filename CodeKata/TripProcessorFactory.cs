using System.IO;
using CodeKata.Filters;
using CodeKata.Parsers;

namespace CodeKata
{
    /// <summary>
    /// Create commonly used configurations
    /// </summary>
    public static class TripProcessorFactory
    {
        public static TripProcessor CreateWithMinMaxFilter(string file, int tripMinMph, int tripMaxMph)
        {
            var minMaxFilter = new MinMaxMilesPerHour(tripMinMph, tripMaxMph);
            var source = new StringSplitTripParser(File.ReadLines(file), minMaxFilter);
            return new TripProcessor(source);
        }
    }
}
