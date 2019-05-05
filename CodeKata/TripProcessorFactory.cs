using System.IO;
using CodeKata.Filters;
using CodeKata.Parsers;

namespace CodeKata
{
    /// <summary>
    /// Creates commonly used processor configurations
    /// </summary>
    public static class TripProcessorFactory
    {
        public static TripProcessor CreateWithMinMaxFilter(string path, int tripMinMph, int tripMaxMph)
        {
            var minMaxFilter = new MinMaxMilesPerHour(tripMinMph, tripMaxMph);
            var source = new StringSplitTripParser(File.ReadLines(path), minMaxFilter);
            return new TripProcessor(source);
        }
    }
}
