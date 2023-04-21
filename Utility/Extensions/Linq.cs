namespace Utility
{
    public static class Linq
    {
        public static IEnumerable<(int, T)> Indexed<T>(this IEnumerable<T> inItems)
        {
            int i = 0;
            foreach (T item in inItems)
                yield return (i++, item);
        }

        public static void ForEach<T>(this IEnumerable<T> inItems, Action<T> inAction)
        {
            foreach (T item in inItems)
                inAction(item);
        }

        public static bool IsSequence(this IEnumerable<int> inItems)
        {
            if (!inItems.Any())
                return true;

            var index = inItems.First();
            foreach (var item in inItems.Skip(1))
            {
                if (item - index != 1)
                    return false;
            }
            return true;
        }

        public static void RemoveRange<T>(this IList<T> inItems, int inIndex, int inCount)
        {
            if (inItems is List<T> list)
            {
                list.RemoveRange(inIndex, inCount);
                return;
            }

            Enumerable.Range(0, inCount).ForEach(_ => inItems.RemoveAt(inIndex));
        }

        public static IEnumerable<IEnumerable<T>> Chunk<T>(this IEnumerable<T> inItems, int inChunkSize)
        {
            var items = inItems;
            while (items.Any())
            {
                yield return items.Take(inChunkSize);
                items = items.Skip(inChunkSize);
            }
        }

        public static void DisposeItems<T>(this IEnumerable<T> inItems) where T : IDisposable
        {
            foreach (var item in inItems)
                item?.Dispose();
        }
    }
}
