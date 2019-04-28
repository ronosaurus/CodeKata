using System;
using System.Diagnostics;

namespace CodeKata
{
    [DebuggerDisplay("Driver = {Driver}, StartTime = {StartTime}, StopTime = {StopTime}, Miles = {Miles}, Elapsed = {Elapsed}, MilesPerHour = {MilesPerHour}")]
    public class Trip
    {
        public string Driver { get; }
        public string StartTime { get; }
        public string StopTime { get; }
        public decimal Miles { get; }
        public decimal MilesPerHour { get; }
        public TimeSpan Elapsed { get; }

        // https://stackoverflow.com/questions/77639/when-is-it-right-for-a-constructor-to-throw-an-exception
        public Trip(string driver, string startTime, string stopTime, decimal miles)
        {
            Driver = driver;
            StartTime = startTime;
            StopTime = stopTime;
            Miles = miles;
            Elapsed = TimeSpan.Parse(StopTime) - TimeSpan.Parse(StartTime);
            MilesPerHour = Miles / (decimal)Elapsed.TotalHours;
        }

        public string ToLogString()
        {
            return $"Driver = {Driver}, StartTime = {StartTime}, StopTime = {StopTime}, Miles = {Miles}, Elapsed = {Elapsed}, MilesPerHour = {MilesPerHour}";
        }
    }
}