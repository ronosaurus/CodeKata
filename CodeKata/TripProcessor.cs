using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using log4net;

namespace CodeKata
{
    /// <summary>
    /// Processes Driver and Trip commands to produce a report based on driver.
    /// </summary>
    public class TripProcessor{
        private static readonly ILog Log = LogManager.GetLogger(typeof(TripProcessor));

        /// <summary>
        /// Processes input file for Driver and Trip commands
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown when input file is null</exception>
        /// <exception cref="InvalidDataException">Thrown when input file contains malformed records that prevent report from being generated</exception>
        public TripResult Process(string file)
        {
            var fileInfo = new FileInfo(file); // throws ArgumentNullException
            if (fileInfo.Exists == false || fileInfo.Length == 0)
            {
                // https://www.owasp.org/index.php/Error_Handling - "Information such as paths on the local file system is considered privileged information"
                throw new ArgumentException("File must exist and have a length greater than zero", nameof(file));
            }

            using (var fs = new FileStream(fileInfo.FullName, FileMode.Open))
            {
                return Process(fs);
            }
        }

        /// <summary>
        /// Processes text stream for Driver and Trip commands
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown when input stream is null</exception>
        /// <exception cref="InvalidDataException">Thrown when input stream contains malformed records that prevent report from being generated</exception>
        public TripResult Process(Stream file)
        {
            if (file == null)
            {
                throw new ArgumentNullException(nameof(file));
            }

            var drivers = new HashSet<string>();
            var trips = new List<Trip>();
            Parse(file, drivers, trips, t =>
            {
                if (t.MilesPerHour < 5 || t.MilesPerHour > 100)
                {
                    Log.Debug($"discarding trip because mph: {t.MilesPerHour} is <5 or >100"); // TODO: IsDebugEnabled
                    return false; // design decision: filter is small enough for two return statements
                }
                return true;
            });

            // main processing code, small enough to fix on the screen without scrolling
            var result = new TripResult();
            var totalsByDriver = trips.ToLookup(d => d.Driver)
                .Where(x => drivers.Contains(x.Key)) // ignore_trips_without_drivers
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

        protected void Parse(Stream file, HashSet<string> drivers, List<Trip> trips, Predicate<Trip> tripFilter)
        {
            int lineCount = 0;
            using (var sr = new StreamReader(file))
            {
                // design decision: no catch block to continue processing because the report will be inaccurate if a bad row is encountered
                while (sr.EndOfStream == false)
                {
                    lineCount++;
                    string line = sr.ReadLine()?.Trim();
                    if (string.IsNullOrEmpty(line) == false)
                    {
                        Log.Debug($"line {lineCount}: {line}"); // TODO: IsDebugEnabled
                        string[] data = line.Split(new[] { ' ' }, StringSplitOptions.None);
                        if (data[0].StartsWith("Driver"))
                        {
                            // example: Driver Alex
                            drivers.Add(data[1]);
                        }
                        else if (data[0].StartsWith("Trip"))
                        {
                            try
                            {
                                // example: Trip Alex 12:01 13:16 42.0
                                Trip trip = new Trip(data[1], data[2], data[3], Convert.ToDecimal(data[4]));
                                if (tripFilter(trip))
                                {
                                    trips.Add(trip);
                                }
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