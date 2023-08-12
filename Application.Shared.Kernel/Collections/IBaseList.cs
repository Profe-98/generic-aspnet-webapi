using Application.Shared.Kernel.Configuration.Const;

namespace Application.Shared.Kernel.Collections
{
    public abstract class IBaseList<T> : IList<T>
    {
        #region Private

        private IList<T> internalList = new List<T>();

        #endregion
        #region Public

        public T this[int i]
        {
            get
            {
                return internalList[i];
            }
            set
            {
                internalList[i] = value;
            }
        }
        public int Count
        {
            get
            {
                return internalList.Count;
            }
        }
        public bool IsReadOnly
        {
            get
            {
                return internalList.IsReadOnly;
            }
        }
        #endregion
        #region Ctor & Dtor
        public IBaseList()
        {

        }
        ~IBaseList()
        {

        }
        #endregion
        #region Methods
        public void Add(T exception)
        {
            internalList.Add(exception);
        }
        public void Insert(int index, T exception)
        {
            internalList.Insert(index, exception);
        }
        public void RemoveAt(int index)
        {
            internalList.RemoveAt(index);
        }
        public bool Remove(T exception)
        {
            return internalList.Remove(exception);
        }
        public int IndexOf(T exception)
        {
            return GeneralDefs.NotFoundResponseValue;
        }
        public void Clear()
        {
            internalList.Clear();
        }
        public bool Contains(T exception)
        {
            return internalList.Contains(exception);
        }
        public void CopyTo(T[] exceptions, int index)
        {
            internalList.CopyTo(exceptions, index);
        }
        private IEnumerator<T> Enumerator()
        {
            return internalList.GetEnumerator();
        }

        public IEnumerator<T> GetEnumerator()
        {
            return Enumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return Enumerator();
        }
        #endregion
        #region Events

        #endregion
        #region EventArgs
        #endregion
    }
    public static class IBaseListExtension
    {
        /// <summary>
        /// Partitioned a list into n-partitions dependend on size for each partition
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public static IEnumerable<List<T>> Partition<T>(this IList<T> source, int size)
        {
            for (int i = 0; i < Math.Ceiling(source.Count / (double)size); i++)
                yield return new List<T>(source.Skip(size * i).Take(size));
        }
    }
}
