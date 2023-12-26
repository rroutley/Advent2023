
public class Heap<T>
{
    private readonly List<HeapItem<T>> heap = [];
    private readonly IComparer<int> comparer;

    public readonly static IComparer<int> MinHeap = Comparer<int>.Default;
    public readonly static IComparer<int> MaxHeap = new MaxHeapComparer();

    public Heap()
        : this(MinHeap)
    {

    }

    public Heap(IComparer<int> comparer)
    {
        this.comparer = comparer ?? throw new ArgumentNullException(nameof(comparer));
    }

    public int Count => heap.Count;

    public void Enqueue(T item, int priority)
    {
        heap.Add(new HeapItem<T>(item, priority));

        BubbleUp();
    }

    public T Dequeue()
    {
        if (heap.Count == 0)
        {
            throw new IndexOutOfRangeException();
        }

        var item = heap[0].Item;

        heap[0] = heap[^1];
        heap.RemoveAt(heap.Count - 1);

        SinkDown();
        return item;
    }

    public T Peek()
    {
        if (heap.Count == 0)
        {
            throw new IndexOutOfRangeException();
        }
        return heap[0].Item;
    }


    private void BubbleUp()
    {
        var index = heap.Count - 1;
        var element = heap[index];

        var parentIndex = (index - 1) / 2;

        while (parentIndex >= 0 && comparer.Compare(heap[parentIndex].Priority, element.Priority) > 0)
        {
            heap[index] = heap[parentIndex];
            heap[parentIndex] = element;

            index = parentIndex;
            parentIndex = (index - 1) / 2;
        }
    }


    private void SinkDown()
    {
        var heapLength = heap.Count;
        if (heapLength == 0)
        {
            return;
        }

        var parentIndex = 0;

        HeapItem<T> leftChild = new();
        HeapItem<T> rightChild;
        while (true)
        {
            var node = heap[parentIndex];
            var leftIndex = (2 * parentIndex) + 1;
            var rightIndex = (2 * parentIndex) + 2;
            bool swap = false;
            var swapIndex = -1;

            if (leftIndex < heapLength)
            {
                leftChild = heap[leftIndex];
                if (comparer.Compare(node.Priority, leftChild.Priority) > 0)
                {
                    swapIndex = leftIndex;
                    swap = true;
                }
            }

            if (rightIndex < heapLength)
            {
                rightChild = heap[rightIndex];
                if ((swap && comparer.Compare(leftChild.Priority, rightChild.Priority) > 0) ||
                    ((!swap) && comparer.Compare(rightChild.Priority, node.Priority) > 0))
                {
                    swapIndex = rightIndex;
                    swap = true;
                }
            }

            if (!swap)
            {
                break;
            }

            heap[parentIndex] = heap[swapIndex];
            heap[swapIndex] = node;

            parentIndex = swapIndex;
        }

    }

    public record struct HeapItem<T>(T Item, int Priority);

    private class MaxHeapComparer : IComparer<int>
    {
        public int Compare(int x, int y)
        {
            return y.CompareTo(x);
        }
    }
}
