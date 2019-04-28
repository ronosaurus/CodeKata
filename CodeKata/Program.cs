using System;
using log4net.Appender;
using log4net.Config;
using log4net.Layout;

namespace CodeKata
{
    public class Program
    {
        static void Main(string[] args)
        {
            ConsoleAppender appender = new ConsoleAppender {Layout = new PatternLayout()};
            appender.ActivateOptions();
            // BasicConfigurator.Configure(appender);

            var result = new TripProcessor().Process(@"..\\..\\..\\ExampleInput.txt");
            Console.WriteLine(result.ToReport());

            Console.WriteLine("Finished running CodeKata.Program.Main...");
            Console.ReadLine();
        }
    }
}
