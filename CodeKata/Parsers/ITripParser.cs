using System.Collections.Generic;

namespace CodeKata.Parsers
{
    // code review: better name? ITripDataSource.Bind, IDataSource.Bind, ITripSupplier.Get
    public interface ITripParser
    {
        void Parse(out ISet<string> drivers, out IList<Trip> trips);
    }
}
