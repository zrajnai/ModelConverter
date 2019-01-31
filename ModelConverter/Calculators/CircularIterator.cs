using System.Collections;
using System.Collections.Generic;

namespace ModelConverter.Calculators {
    internal class CircularIterator : IEnumerator<int>
    {

        #region Constructors

        public CircularIterator(int count, int startIndex = 0)
        {
            Current = startIndex % count;
            Count = count;
        }

        #endregion

        #region Public Properties

        public int Current { get; private set; }

        public int Count { get; }

        #endregion

        object IEnumerator.Current => Current;

        #region Public Methods

        public void Dispose() { }

        public bool MoveNext()
        {
            Current = (Current + 1 + Count) % Count;
            return true;
        }

        public void Reset()
        {
            Current = 0;
        }

        #endregion

    }
}