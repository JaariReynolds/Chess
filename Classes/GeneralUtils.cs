namespace ChessLogic.Classes
{
    public static class GeneralUtils
    {
        public static bool Some<T>(this IEnumerable<T> source, Func<T, bool> predicate, int count)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));

            int matches = 0;
            foreach (var item in source)
            {
                if (predicate(item))
                {
                    matches++;
                    if (matches >= count) return true;
                }
            }

            return false;
        }
    }
}
