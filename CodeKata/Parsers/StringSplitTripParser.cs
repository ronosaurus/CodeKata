using System;
using System.Collections.Generic;
using System.IO;
using CodeKata.Filters;
using log4net;

namespace CodeKata.Parsers
{
    // code review: filtering in base class to encourage immediate filtering and not another loop afterwards, could be switched?
    public class StringSplitTripParser : BaseFilteringParser
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(StringSplitTripParser));

        private readonly IEnumerable<string> _source;

        public StringSplitTripParser(IEnumerable<string> source, params IFilter<Trip>[] tripFilters) : base(tripFilters)
        {
            _source = source;
        }

        protected override void Parse()
        {
            int lineCount = 0;
            foreach (var line in _source)
            {
                lineCount++;
                if (string.IsNullOrEmpty(line) == false)
                {
                    Log.Debug($"line {lineCount}: {line}"); // TODO: IsDebugEnabled
                    string[] data = line.Split(new[] { ' ' }, StringSplitOptions.None);
                    if (data[0].StartsWith("Driver"))
                    {
                        // example: Driver Alex
                        AddDriver(data[1]);
                    }
                    else if (data[0].StartsWith("Trip"))
                    {
                        try
                        {
                            // example: Trip Alex 12:01 13:16 42.0
                            Trip trip = new Trip(data[1], data[2], data[3], Convert.ToDecimal(data[4]));
                            AddTrip(trip);
                        }
                        catch (FormatException e)
                        {
                            // https://stackoverflow.com/questions/16265247/printing-all-contents-of-array-in-c-sharp
                            Log.ErrorFormat("Unable to parse Trip command on line {0}, caught FormatException with e.Message: [{1}] throwing InvalidDataException; data: [{2}]",
                                lineCount,
                                e.Message,
                                string.Join(", ", data));
                            throw new InvalidDataException($"Error parsing Trip command on line {lineCount}", e);
                        }
                    }
                    else
                    {
                        throw new InvalidDataException($"Unknown command, line {lineCount} must start with Driver or Trip");
                    }
                }
                else
                {
                    Log.Warn($"line {lineCount} is null/empty, skipping");
                }
            }
        }
    }
}