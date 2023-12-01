
public static class Extensions
{
    public static IEnumerable<IEnumerable<T>> AsBatchesOf<T>(this IEnumerable<T> items, int batchSize)
    {

        return items.Select((item, index) => new { 
                        Item = item, 
                        Batch = index / batchSize 
                    })
                    .GroupBy(i => i.Batch)
                    .Select(g => g.Select(i => i.Item));

    }

}