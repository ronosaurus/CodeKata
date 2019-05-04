namespace CodeKata.Filters
{
    // design decision: created interface so implementations can log; alternative was to use Predicates directly
    public interface IFilter<in T>
    {
        bool Match(T value);
    }
}
