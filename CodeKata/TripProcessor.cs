using System;
using System.IO;
using System.Linq;
using CodeKata.Parsers;
using log4net;

namespace CodeKata
{
    /// <summary>
    /// Processes Driver and Trip commands to produce a report based on driver.
    /// </summary>
    public class TripProcessor
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(TripProcessor));

        private readonly ITripParser _parser;

        public TripProcessor(ITripParser parser)
        {
            _parser = parser ?? throw new ArgumentNullException(nameof(parser));
        }
        
        public TripResult Process()
        {
            // if requirements specific a slightly different data format we could have yielded each item
            _parser.Parse(out var drivers, out var trips);

            var result = new TripResult();
            var totalsByDriver = trips.ToLookup(d => d.Driver)
                .Where(x => drivers.Contains(x.Key)) // trips without drivers are ZeroMilesDrivers
                .Select(x => new TripResult.TripTotal
                {
                    Driver = x.Key,
                    Hours = x.Sum(h => (decimal) h.Elapsed.TotalHours),
                    Miles = x.Sum(m => m.Miles),
                });
            foreach (var total in totalsByDriver)
            {
                Log.Debug($"total: [{total.ToLogString()}]"); // TODO: IsDebugEnabled
                result.Successful.Add(total);
                drivers.Remove(total.Driver);
            }
            foreach (var remainingDriver in drivers)
            {
                result.ZeroMilesDrivers.Add(remainingDriver);
            }

            return result;
        }
    }
}

/*
Use any .Net language; use any project type.  The goal of this exercise is to get a better glimpse into your thought process.  While this is a simple exercise, think of it as a large project, so put whatever patterns you think would be necessary.
Create a ReadMe file to explain any details.  Do what you think would help verify your work.   

The code will process an input file. 

Each line in the input file will start with a command. There are two possible commands.
The first command is Driver, which will register a new Driver in the app. Example:
Driver Dan
The second command is Trip, which will record a trip attributed to a driver. The line will be space delimited with the following fields: the command (Trip), driver name, start time, stop time, miles driven. Times will be given in the format of hours:minutes. We'll use a 24-hour clock and will assume that drivers never drive past midnight (the start time will always be before the end time). Example:
Trip Dan 07:15 07:45 17.3
Discard any trips that average a speed of less than 5 mph or greater than 100 mph.
Generate a report containing each driver with total miles driven and average speed. Sort the output by most miles driven to least. Round miles and miles per hour to the nearest integer.

Example input:
Driver Dan
Driver Alex
Driver Bob
Trip Dan 07:15 07:45 17.3
Trip Dan 06:12 06:32 21.8
Trip Alex 12:01 13:16 42.0
Expected output:
Alex: 42 miles @ 34 mph
Dan: 39 miles @ 47 mph
Bob: 0 miles
*/