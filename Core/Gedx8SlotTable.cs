namespace Gedx8MusicDriver.Core
{
    internal sealed class Gedx8SlotTable<T> where T : class
    {
        private readonly List<T?> _slots;

        internal Gedx8SlotTable(int initialCapacity)
        {
            _slots = new List<T?>(Math.Max(1, initialCapacity));
            EnsureCapacity(Math.Max(1, initialCapacity));
        }

        internal int LiveCount { get; private set; }

        internal int HighWaterMark { get; private set; }

        internal T Add(T value)
        {
            int index = FindFirstFreeIndex();
            if (index < 0)
            {
                Grow();
                index = FindFirstFreeIndex();
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

            _slots[index] = null;
            if (LiveCount > 0)
            {
                LiveCount--;
            }

            TrimHighWaterMark();
            return true;
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

            TrimHighWaterMark();
            return true;
        }

        internal int IndexOf(T value)
        {
            return _slots.IndexOf(value);
        }

        internal IReadOnlyList<T> GetLiveObjects()
        {
            List<T> result = new();
            for (int i = 0; i < _slots.Count; i++)
            {
                T? value = _slots[i];
                if (value != null)
                {
                    result.Add(value);
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

        private void Grow()
        {
            int current = _slots.Count;
            int delta = current > 0x40 ? ((current + (current & 0x3)) >> 2) : (current <= 8 ? 4 : 0x0C);
            int target = current + Math.Max(4, delta);
            EnsureCapacity(target);
        }

        private void EnsureCapacity(int target)
        {
            while (_slots.Count < target)
            {
                _slots.Add(null);
            }
        }

        private void TrimHighWaterMark()
        {
            int highWaterMark = 0;
            for (int i = _slots.Count - 1; i >= 0; i--)
            {
                if (_slots[i] != null)
                {
                    highWaterMark = i + 1;
                    break;
                }
            }

            HighWaterMark = highWaterMark;
        }
    }
}
