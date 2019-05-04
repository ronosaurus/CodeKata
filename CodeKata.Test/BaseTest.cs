using System;
using System.Collections.Generic;
using System.Linq;
using CodeKata.Filters;
using CodeKata.Parsers;

namespace CodeKata.Test
{
    public abstract class BaseTest
    {
        protected TripResult Process(string value, params IFilter<Trip>[] filters)
        {
            var trimmed = AsEnumerable(value).Select(l => l.Trim());
            var parser = new StringSplitTripParser(trimmed, filters);
            return new TripProcessor(parser).Process();
        }

        // test helper, doesn't need to be efficient
        protected IEnumerable<string> AsEnumerable(string value)
        {
            return value.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries).AsEnumerable();
        }

        // test helper, doesn't need to be efficient
        protected IEnumerable<string> AsEnumerableTrim(string value)
        {
            return AsEnumerable(value).Select(l => l.Trim());
        }

        // test helper, doesn't need to be efficient
        protected string Trim(string value)
        {
            // https://stackoverflow.com/questions/14205645/how-to-trim-a-multi-line-string
            return string.Join(Environment.NewLine, AsEnumerable(value).Select(l => l.Trim()));
        }
    }
}
