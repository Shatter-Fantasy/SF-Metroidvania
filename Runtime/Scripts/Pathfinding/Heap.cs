using System;

namespace SF
{
    public interface IHeapItem<TItemData> : IComparable<TItemData>
    {
        public int HeapIndex { get; set; }
    }

    public class Heap<TItemData> where TItemData : IHeapItem<TItemData>
    {
        private TItemData[] _items;
        int _currentItemCount;

        public Heap(int maxHeapSize)
        {
            _items = new TItemData[maxHeapSize];
        }

        public void Add(TItemData itemData)
        {
            itemData.HeapIndex = _currentItemCount;
            _items[_currentItemCount] = itemData;
            SortUp(itemData);
            _currentItemCount++;
        }

        public TItemData RemoveFirst()
        {
            TItemData firstItem = _items[0];
            _currentItemCount--;
            _items[0] = _items[_currentItemCount];
            _items[0].HeapIndex = 0;
            SortDown(_items[0]);
            return firstItem;
        }

        public void UpdateItem(TItemData itemData)
        {
            SortUp(itemData);
        }

        public int Count
        {
            get => _currentItemCount;
        }

        public bool Contains(TItemData itemData)
        {
            return Equals(_items[itemData.HeapIndex], itemData);
        }
        private void SortDown(TItemData itemData)
        {
            while(true)
            {
                int childIndexLeft = itemData.HeapIndex * 2 + 1;
                int childIndexRight = itemData.HeapIndex * 2 + 2;
                int swapIndex = 0;
                
                if(childIndexLeft < _currentItemCount)
                {
                    swapIndex = childIndexLeft;

                    if(childIndexRight < _currentItemCount)
                    {
                        if(_items[childIndexLeft].CompareTo(_items[childIndexRight]) < 0)
                        {
                            swapIndex = childIndexRight;
                        }
                    }

                    if(itemData.CompareTo(_items[swapIndex]) < 0)
                    {
                        Swap(itemData, _items[swapIndex]);
                    }
                    else // If the parent is already in the correct position just exit out of loop.
                        return;
                }
                else // If the parent has no children just exit out of loop.
                    return;

            } // End of SortDown while loop
        }

        private void SortUp(TItemData itemData)
        {
            // Do a binary tree sort.
            int parentIndex = (itemData.HeapIndex - 1) / 2;

            while(true)
            {
                TItemData parentItem = _items[parentIndex];

                if(itemData.CompareTo(parentItem) > 0)
                {
                    Swap(itemData, parentItem);
                }
                else
                    break;

                parentIndex = (itemData.HeapIndex - 1) / 2;
            }
        }

        private void Swap(TItemData itemDataA, TItemData itemDataB)
        {
            _items[itemDataA.HeapIndex] = itemDataB;
            _items[itemDataB.HeapIndex] = itemDataA;

            int itemAIndex = itemDataA.HeapIndex;
            itemDataA.HeapIndex = itemDataB.HeapIndex;
            itemDataB.HeapIndex = itemAIndex;

        }
    }
}
