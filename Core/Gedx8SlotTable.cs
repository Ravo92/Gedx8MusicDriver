namespace Gedx8MusicDriver.Core
{
    internal sealed class Gedx8SlotTable<T> where T : class
    {
        private readonly List<T?> _slots;

        internal Gedx8SlotTable(int initialCapacity)
        {
            _slots = new List<T?>(Math.Max(1, initialCapacity));
            EnsureCapacityExact10002330(Math.Max(1, initialCapacity));
        }

        internal int LiveCount { get; private set; }

        internal int HighWaterMark { get; private set; }

        internal int Capacity => _slots.Count;

        internal T Add(T value)
        {
            int index = FindFirstFreeIndex();
            if (index < 0)
            {
                EnsureGrowth100022C0();
                index = FindFirstFreeIndex();
            }

            if (index < 0)
            {
                index = _slots.Count;
                EnsureCapacityExact10002330(index + 1);
            }

            _slots[index] = value;
            LiveCount++;

            if (index + 1 > HighWaterMark)
            {
                HighWaterMark = index + 1;
            }

            return value;
        }

        internal bool Remove(T value)
        {
            int index = _slots.IndexOf(value);
            if (index < 0)
            {
                return false;
            }

            return RemoveAt(index);
        }

        internal bool RemoveAt(int index)
        {
            if (index < 0 || index >= _slots.Count)
            {
                return false;
            }

            if (_slots[index] == null)
            {
                return false;
            }

            _slots[index] = null;

            if (LiveCount > 0)
            {
                LiveCount--;
            }

            return true;
        }

        internal int IndexOf(T value)
        {
            return _slots.IndexOf(value);
        }

        internal T? GetAt(int index)
        {
            if (index < 0 || index >= _slots.Count)
            {
                return null;
            }

            return _slots[index];
        }

        internal bool SetAt(int index, T? value)
        {
            if (index < 0 || index >= _slots.Count)
            {
                return false;
            }

            _slots[index] = value;
            return true;
        }

        internal IReadOnlyList<T> GetLiveObjects()
        {
            List<T> result = [];

            for (int i = 0; i < HighWaterMark; i++)
            {
                T? entry = _slots[i];
                if (entry != null)
                {
                    result.Add(entry);
                }
            }

            return result;
        }

        internal void Clear()
        {
            for (int i = 0; i < _slots.Count; i++)
            {
                _slots[i] = null;
            }

            LiveCount = 0;
            HighWaterMark = 0;
        }

        internal void EnsureGrowth100022C0()
        {
            int currentCapacity = _slots.Count;
            int delta;

            if (currentCapacity > 0x40)
            {
                delta = (currentCapacity + (currentCapacity & 0x3)) >> 2;
            }
            else if (currentCapacity <= 8)
            {
                delta = 4;
            }
            else
            {
                delta = 0x10;
            }

            int targetCapacity = currentCapacity + delta;
            if (targetCapacity <= currentCapacity)
            {
                return;
            }

            EnsureCapacityExact10002330(targetCapacity);
        }

        internal void EnsureCapacityExact10002330(int targetCapacity)
        {
            if (targetCapacity <= _slots.Count)
            {
                return;
            }

            while (_slots.Count < targetCapacity)
            {
                _slots.Add(null);
            }
        }

        private int FindFirstFreeIndex()
        {
            for (int i = 0; i < _slots.Count; i++)
            {
                if (_slots[i] == null)
                {
                    return i;
                }
            }

            return -1;
        }
    }
}