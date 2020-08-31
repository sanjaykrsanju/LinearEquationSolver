using System;
using System.Collections.Generic;

namespace LinearEquationSolver.Helper
{

    [Serializable]
    public class Sparse2DMatrix<TKey0, TKey1, TValue> : IEnumerable<KeyValuePair<ComparableTuple2<TKey0, TKey1>, TValue>>
        where TKey0 : IComparable<TKey0>
        where TKey1 : IComparable<TKey1>
    {
        private Dictionary<ComparableTuple2<TKey0, TKey1>, TValue> m_dictionary;

        /// <summary>
        /// This property stores the default value that is returned if the keys don't exist in the array.
        /// </summary>
        public TValue DefaultValue
        {
            get;
            set;
        }

        /// <summary>
        /// Property to get the count of items in the sparse array.
        /// </summary>
        public int Count
        {
            get
            {
                return m_dictionary.Count;
            }
        }

        #region Constructors
        /// <summary>
        /// Constructor - creates an empty sparse array instance.
        /// </summary>
        public Sparse2DMatrix()
        {
            InitializeDictionary();
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="defaultValue">A default value to return if the key is not present.</param>
        public Sparse2DMatrix(TValue defaultValue)
        {
            InitializeDictionary();
            DefaultValue = defaultValue;
        }

        /// <summary>
        /// Copy constructor.
        /// </summary>
        /// <param name="sparse2DMatrix">The sparse array instance to be copied</param>
        public Sparse2DMatrix(Sparse2DMatrix<TKey0, TKey1, TValue> sparse2DMatrix)
        {
            InitializeDictionary();
            Initialize(sparse2DMatrix);
            DefaultValue = sparse2DMatrix.DefaultValue;
        }

        #endregion

        /// <summary>
        /// Initialize the dictionary to compare items based on key values.
        /// </summary>
        private void InitializeDictionary()
        {
            ComparableTuple2EqualityComparer<TKey0, TKey1> equalityComparer = new ComparableTuple2EqualityComparer<TKey0, TKey1>();
            m_dictionary = new Dictionary<ComparableTuple2<TKey0, TKey1>, TValue>(equalityComparer);
        }

        /// <summary>
        /// Method to copy the data in another Sparse2DMatrix instance to this instance.
        /// </summary>
        /// <param name="sparse2DMatrix">An instance of the Sparse2DMatrix class.</param>
        private void Initialize(Sparse2DMatrix<TKey0, TKey1, TValue> sparse2DMatrix)
        {
            m_dictionary.Clear();

            // Copy each key value pair to the dictionary.
            foreach (KeyValuePair<ComparableTuple2<TKey0, TKey1>, TValue> pair in sparse2DMatrix)
            {
                ComparableTuple2<TKey0, TKey1> newCombinedKey = new ComparableTuple2<TKey0, TKey1>(pair.Key);
                m_dictionary.Add(newCombinedKey, pair.Value);
            }
        }

        /// <summary>
        /// Method to copy the data in this Sparse2DMatrix instance to another instance.
        /// </summary>
        /// <param name="sparse2DMatrix">An instance of the Sparse2DMatrix class.</param>
        public void CopyTo(Sparse2DMatrix<TKey0, TKey1, TValue> sparse2DMatrix)
        {
            sparse2DMatrix.m_dictionary.Clear();

            // Copy each key value pair to the dictionary.
            foreach (KeyValuePair<ComparableTuple2<TKey0, TKey1>, TValue> pair in m_dictionary)
            {
                ComparableTuple2<TKey0, TKey1> newCombinedKey = new ComparableTuple2<TKey0, TKey1>(pair.Key);
                sparse2DMatrix.m_dictionary.Add(newCombinedKey, pair.Value);
            }
        }

        /// <summary>
        /// Property []
        /// </summary>
        /// <param name="key0">The first key used to index the value</param>
        /// <param name="key1">The second key used to index the value</param>
        /// <returns>The 'get' property returns the value at the current key</returns>
        public TValue this[TKey0 key0, TKey1 key1]
        {
            get
            {
                TValue value;

                if (!m_dictionary.TryGetValue(CombineKeys(key0, key1), out value))
                {
                    value = DefaultValue;
                }

                return value;
            }

            set
            {
                m_dictionary[CombineKeys(key0, key1)] = value;
            }
        }
        /// <summary>
        /// Determines whether this matrix contains the specified keys.
        /// </summary>
        /// <param name="key0">The first key used to index the value</param>
        /// <param name="key1">The second key used to index the value</param>
        /// <returns>Returns the value 'true' if and only if the keys exists in this matrix</returns>
        public bool ContainsKey(TKey0 key0, TKey1 key1)
        {
            return m_dictionary.ContainsKey(CombineKeys(key0, key1));
        }


        public bool ContainsValue(TValue value)
        {
            return m_dictionary.ContainsValue(value);
        }


        public bool TryGetValue(TKey0 key0, TKey1 key1, out TValue value)
        {
            return m_dictionary.TryGetValue(CombineKeys(key0, key1), out value);
        }


        public bool Remove(TKey0 key0, TKey1 key1)
        {
            return m_dictionary.Remove(CombineKeys(key0, key1));
        }

        /// <summary>
        /// Method to clear all values in the sparse array.
        /// </summary>
        public void Clear()
        {
            m_dictionary.Clear();
        }

        public ComparableTuple2<TKey0, TKey1> CombineKeys(TKey0 key0, TKey1 key1)
        {
            return new ComparableTuple2<TKey0, TKey1>(key0, key1);
        }

        public void SeparateCombinedKeys(ComparableTuple2<TKey0, TKey1> combinedKey, ref TKey0 key0, ref TKey1 key1)
        {
            key0 = combinedKey.Item0;
            key1 = combinedKey.Item1;
        }

        #region IEnumerable<KeyValuePair<ComparableTuple2<TKey0, TKey1>, TValue>> Members


        public IEnumerator<KeyValuePair<ComparableTuple2<TKey0, TKey1>, TValue>> GetEnumerator()
        {
            return this.m_dictionary.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.m_dictionary.GetEnumerator();
        }

        #endregion
    }
}
