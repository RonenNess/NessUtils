using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;


namespace Ness.Utils
{
    /// <summary>
    /// A fast list that upon removal just nullify the value instead of shifting the entire array.
    /// Used for when you want fast addition, fast removal AND fast iteration (as long as you don't carry too many holes).
    /// Based on code by Jared Thirsk, from here:
    /// https://stackoverflow.com/questions/8761238/collection-with-very-fast-iterating-and-good-addition-and-remove-speeds
    /// 
    /// Notice: this list can't hold nulls, as the enumerator would skip them.
    /// </summary>
    public class SparseList<T> : IList<T>
    where T : class
    {
        // internal list we use to store values
        List<T> _list = new List<T>();

        // stack of empty places to populate
        Stack<int> _freeIndices = new Stack<int>();

        // to break enumerators if changed while iterating externally.
        int _version = 0;

        /// <summary>
        /// If not zero, will automatically compact if got more holes than this given value.
        /// </summary>
        public int MaxHolesAllowed = 256;

        /// <summary>
        /// Set / get list capacity.
        /// </summary>
        public int Capacity { get { return _list.Capacity; } set { _list.Capacity = value; } }

        /// <summary>
        /// Get list count (of actual values without nulls).
        /// </summary>
        public int Count { get { return NonNullCount; } }

        /// <summary>
        /// Clear all null holes from list, to make iteration faster.
        /// </summary>
        public void Compact()
        {
            var sortedIndices = _freeIndices.ToList();
            foreach (var i in sortedIndices.OrderBy(x => x).Reverse())
            {
                _list.RemoveAt(i);
            }
            _freeIndices.Clear();
            _list.Capacity = _list.Count;
            _version++; // breaks open enumerators
        }

        /// <summary>
        /// Get index of item.
        /// </summary>
        public int IndexOf(T item)
        {
            return _list.IndexOf(item);
        }

        /// <summary>
        /// Slow (forces a compact) insert, not recommended
        /// </summary>
        public void Insert(int index, T item)
        {
            // One idea: remove index from freeIndices if it's in there.  Stack doesn't support this though.
            Compact(); // breaks the freeIndices list, so apply it before insert
            _list.Insert(index, item);
        }

        /// <summary>
        /// Remove at position by nullify value.
        /// </summary>
        /// <param name="index"></param>
        public void RemoveAt(int index)
        {
            // if its last index, just remove
            if (index == CountWithNulls - 1)
            {
                _list.RemoveAt(index);
            }
            // if its not last index, create free index
            else
            {
                _list[index] = null;
                _freeIndices.Push(index);
                CompactIfRequired();
            }
        }

        /// <summary>
        /// Get / set by index.
        /// Note: can hit null values.
        /// </summary>
        public T this[int index]
        {
            get
            {
                return _list[index];
            }
            set
            {
                if (value == null) { throw new ArgumentNullException(); }
                _list[index] = value;
            }
        }

        /// <summary>
        /// Add item to list.
        /// </summary>
        public void Add(T item)
        {
            if (item == null) throw new ArgumentNullException();
            if (_freeIndices.Count == 0) { _list.Add(item); return; }
            _list[_freeIndices.Pop()] = item;
        }

        /// <summary>
        /// Clear list.
        /// </summary>
        public void Clear()
        {
            _list.Clear();
            _freeIndices.Clear();
            _version++;
        }

        /// <summary>
        /// Return if contains a value.
        /// </summary>
        public bool Contains(T item)
        {
            if (item == null) return false;
            return _list.Contains(item);
        }

        /// <summary>
        /// Result may contain nulls.
        /// </summary>
        public void CopyTo(T[] array, int arrayIndex)
        {
            foreach (var val in _list)
            {
                if (val != null)
                {
                    array[arrayIndex++] = val;
                }
            }
        }

        /// <summary>
        /// Use this for iterating via for loop.
        /// Note: can contain nulls.
        /// </summary>
        public int CountWithNulls { get { return _list.Count; } }

        /// <summary>
        /// Don't use this for for loops!  Use Count.
        /// </summary>
        public int NonNullCount
        {
            get { return _list.Count - _freeIndices.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        /// <summary>
        /// Removes an item from list.
        /// </summary>
        public bool Remove(T item)
        {
            // find item index
            int i = _list.IndexOf(item);
            if (i < 0) return false;

            // if index is last, remove it since there's no array shifting
            if (i == _list.Count - 1)
            {
                _list.RemoveAt(i);
            }
            // if its index that's not last, nullify and add to free indices
            else
            {
                _list[i] = null;
                _freeIndices.Push(i);
                CompactIfRequired();
            }

            return true;
        }

        /// <summary>
        /// Compact the list if 'MaxHolesAllowed' is set and there are currently too many holes.
        /// </summary>
        private void CompactIfRequired()
        {
            if (MaxHolesAllowed != 0 && _freeIndices.Count > MaxHolesAllowed)
            {
                Compact();
            }
        }

        /// <summary>
        /// Get enumerator.
        /// Note: will skip null values.
        /// </summary>
        public IEnumerator<T> GetEnumerator()
        {
            return new SparseListEnumerator(this);
        }

        /// <summary>
        /// Get enumerator.
        /// Note: will skip null values.
        /// </summary>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Enumerator for the sparse list.
        /// </summary>
        private class SparseListEnumerator : IEnumerator<T>
        {
            SparseList<T> list;
            int version;
            int index = -1;

            /// <summary>
            /// Create the enumerator.
            /// </summary>
            public SparseListEnumerator(SparseList<T> list)
            {
                this.list = list;
                this.version = list._version;
            }

            /// <summary>
            /// Get current value.
            /// </summary>
            public T Current
            {
                get
                {
                    return list[index];
                }
            }

            /// <summary>
            /// Get current value.
            /// </summary>
            object IEnumerator.Current
            {
                get { return Current; }
            }

            /// <summary>
            /// Dispose enumerator.
            /// </summary>
            public void Dispose()
            {
                list = null;
            }

            /// <summary>
            /// Move to next value.
            /// </summary>
            public bool MoveNext()
            {
                do
                {
                    // values changed during iteration
                    // if (version != list._version) { throw new InvalidOperationException("Collection modified"); }

                    // move to next index until we get a valud value
                    while (++index < list.CountWithNulls && list[index] == null)
                    {
                    }

                    // end enumeration
                    return index < list.CountWithNulls;

                } while (Current == null);
            }

            /// <summary>
            /// Reset enumerator.
            /// </summary>
            public void Reset()
            {
                index = -1;
                version = list._version;
            }

            /// <summary>
            /// Accessing Current after RemoveCurrent may throw a NullReferenceException or return null.
            /// </summary>
            public void RemoveCurrent()
            {
                list.RemoveAt(index);
            }
        }
    }
}