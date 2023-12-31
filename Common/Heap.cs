
using System.Collections;
using System.Diagnostics;

public class Heap<T> : IReadOnlyList<T>
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

    public T this[int index] => heap[index].Item;


    public void RemoveAt(int index)
    {
        heap[index] = heap[^1];
        heap.RemoveAt(heap.Count - 1);

        BubbleUp(index);
        SinkDown(index);
        EnsureValid();
    }

    public void Enqueue(T item, int priority)
    {
        heap.Add(new HeapItem<T>(item, priority));

        BubbleUp();
        EnsureValid();
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

        EnsureValid();
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


    private void BubbleUp(int index = int.MaxValue)
    {
        index = Math.Min(index, heap.Count - 1);
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

    private void SinkDown(int parentIndex = 0)
    {
        var heapLength = heap.Count;
        if (parentIndex >= heapLength)
        {
            return;
        }

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
                    (!swap && comparer.Compare(node.Priority, rightChild.Priority) > 0))
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

    [Conditional("DEBUG")]
    private void EnsureValid()
    {
        if (heap.Count == 0)
        {
            return;
        }

        Stack<int> queue = new();
        queue.Push(0);
        while (queue.Count > 0)
        {
            var parent = queue.Pop();
            var parentPriority = heap[parent].Priority;

            int leftIndex = 2 * parent + 1;
            if (leftIndex < heap.Count)
            {
                var leftPriority = heap[leftIndex].Priority;
                if (comparer.Compare(parentPriority, leftPriority) > 0)
                {
                    throw new InvalidDataException();
                }

                queue.Push(leftIndex);
            }


            int rightIndex = 2 * parent + 2;
            if (rightIndex < heap.Count)
            {
                var rightPrioity = heap[rightIndex].Priority;
                if (comparer.Compare(parentPriority, rightPrioity) > 0)
                {
                    throw new InvalidDataException();
                }
                queue.Push(rightIndex);
            }

        }
    }

    public IEnumerator<T> GetEnumerator() => heap.Select(h => h.Item).GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public record struct HeapItem<T>(T Item, int Priority);

    private class MaxHeapComparer : IComparer<int>
    {
        public int Compare(int x, int y)
        {
            return y.CompareTo(x);
        }
    }


}
