using System;
using System.IO;
using System.Linq;
using System.Text;

namespace CodeKata.Test
{
    public abstract class BaseTest
    {
        protected TripResult Process(string value)
        {
            using (var ms = new MemoryStream(Encoding.Default.GetBytes(Trim(value))))
            {
                TripProcessor processor = new TripProcessor();
                return processor.Process(ms);
            }
        }

        // test helper, doesn't need to be efficient
        protected string Trim(string value)
        {
            // https://stackoverflow.com/questions/14205645/how-to-trim-a-multi-line-string
            return string.Join(Environment.NewLine, value.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries).AsEnumerable().Select(l => l.Trim()));
        }
    }
}
