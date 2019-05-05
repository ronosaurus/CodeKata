namespace CodeKata.Filters
{
    // design decision: created interface so implementations could be testable and more complex than Func<T, bool>
    public interface IFilter<in T>
    {
        bool Match(T value);
    }
}
