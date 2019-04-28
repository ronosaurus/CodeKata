using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace CodeKata
{
    [DebuggerDisplay("Successful = {Successful.Count}, ZeroMilesDrivers = {ZeroMilesDrivers.Count}")]
    public class TripResult
    {
        // expert: expose underlying data for custom reporting
        public List<TripTotal> Successful { get; } = new List<TripTotal>();
        public List<string> ZeroMilesDrivers { get; } = new List<string>();

        // happy path
        public string ToReport()
        {
            var sb = new StringBuilder();
            Successful.OrderByDescending(x => x.Miles).ToList().ForEach(x => sb.AppendLine($"{x.Driver}: {x.Miles:0} miles @ {x.MilesPerHour:0} mph"));
            ZeroMilesDrivers.ForEach(x => sb.AppendLine($"{x}: 0 miles"));
            return sb.ToString();
        }

        // design decision: inner class because TripTotal is rarely used externally
        [DebuggerDisplay("Driver = {Driver}, Hours = {Hours}, Miles = {Miles}, MilesPerHour = {MilesPerHour}")]
        public class TripTotal
        {
            // TODO: add constructor and remove setters, setter exists for linq query in TripProcessor#Process
            public string Driver { get; set; }
            public decimal Hours { get; set; }
            public decimal Miles { get; set; }
            public decimal MilesPerHour => Miles / Hours; // code review: ok to throw if Hours are 0?

            public string ToLogString()
            {
                // can't share this constant layout with DebuggerDisplay, no particular reason for ":" vs "="
                return $"Driver: {Driver}, Hours: {Hours}, Miles: {Miles}, MilesPerHour: {MilesPerHour}";
            }
        }
    }
}
