namespace Todo.NET.Extensions;

public static class EnumerableExtensions
{
    public static IEnumerable<(T item, int index)> WithIndex<T>(this IEnumerable<T> input)
        => input.Select((item, idx) => (item, idx));
}