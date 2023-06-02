namespace Genesis.Common.Extensions
{
    public static class EnumerableExtensions
    {
        public static bool TryGetValue<T>(this IEnumerable<T> enumerable, Predicate<T> condition, out T value)
        {
            ArgumentNullException.ThrowIfNull(enumerable);

            value = enumerable.FirstOrDefault(item => condition(item));

            return value is not null;
        }

        public static bool TryGetSingleValue<T>(this IEnumerable<T> enumerable, Predicate<T> condition, out T value)
        {
            ArgumentNullException.ThrowIfNull(enumerable);

            if (enumerable.Count(item => condition(item)) > 1)
            {
                value = default;
                return false;
            }

            return enumerable.TryGetValue(condition, out value);
        }
    }
}
