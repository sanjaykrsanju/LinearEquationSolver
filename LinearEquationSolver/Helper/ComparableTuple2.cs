using System;
using System.Collections.Generic;

namespace LinearEquationSolver.Helper
{
    /// <summary>
    /// This class implements a group of 2 items.
    /// </summary>
    /// <typeparam name="TItem0">The type of the first item</typeparam>
    /// <typeparam name="TItem1">The type of the second item</typeparam>
    [Serializable]
    public class ComparableTuple2<TItem0, TItem1> : IComparable<ComparableTuple2<TItem0, TItem1>>
        where TItem0 : IComparable<TItem0>
        where TItem1 : IComparable<TItem1>
    {
        /// <summary>
        /// The first item
        /// </summary>
        public TItem0 Item0
        {
            get;
            private set;
        }

        /// <summary>
        /// The second item.
        /// </summary>
        public TItem1 Item1
        {
            get;
            private set;
        }

        #region Constructors

        /// <summary>
        /// Constructor
        /// </summary>
        public ComparableTuple2()
        {
        }

        /// <summary>
        /// Constructs a new instance with the same item values as this instance.
        /// </summary>
        /// <param name="group2"></param>
        public ComparableTuple2(ComparableTuple2<TItem0, TItem1> group)
        {
            Item0 = group.Item0;
            Item1 = group.Item1;
        }

        /// <summary>
        /// Constructs a new instance with the specified item values.
        /// </summary>
        /// <param name="item0">The first item</param>
        /// <param name="item1">The second item</param>
        public ComparableTuple2(TItem0 item0, TItem1 item1)
        {
            Item0 = item0;
            Item1 = item1;
        }

        #endregion

        #region IComparable<ComparableTuple2> implementation
        /// <summary>
        /// This methods implements the IComparable<ComparableTuple2<TItem0, TItem1>> interface.
        /// </summary>
        /// <param name="group">The group being compared to this group</param>
        /// <returns>
        /// The value -1 if this groups is less than the passed group.
        /// The value 1 if this group is greater than the passed group.
        /// The value 0 if this group and the passed groups are equal.
        /// </returns>
        public int CompareTo(ComparableTuple2<TItem0, TItem1> group)
        {
            int result = this.Item0.CompareTo(group.Item0);

            if (result == 0)
            {
                result = this.Item1.CompareTo(group.Item1);
            }

            return result;
        }

        #endregion
    }


    [Serializable]
    public class ComparableTuple2EqualityComparer<TItem0, TItem1> : IEqualityComparer<ComparableTuple2<TItem0, TItem1>>
        where TItem0 : IComparable<TItem0>
        where TItem1 : IComparable<TItem1>
    {
        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        public ComparableTuple2EqualityComparer()
        {
        }

        #endregion

        /// IEqualityComparer.Equals compares the items in this group for equality.
        public bool Equals(ComparableTuple2<TItem0, TItem1> groupA,
                           ComparableTuple2<TItem0, TItem1> groupB)
        {
            return ((groupA.Item0.Equals(groupB.Item0))
                && (groupA.Item1.Equals(groupB.Item1)));
        }


        public int GetHashCode(ComparableTuple2<TItem0, TItem1> group)
        {
            int hash0 = group.Item0.GetHashCode();
            int hash1 = group.Item1.GetHashCode();
            int hash = 577 * hash0 + 599 * hash1;
            return hash.GetHashCode();
        }
    }
}
