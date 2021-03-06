using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Pyxis.Core
{
    /// <summary>
    /// Represents a range of items.
    /// </summary>
    /// <typeparam name="T">The range type.</typeparam>
    public class Range<T> : IComparable<Range<T>>, IComparable<T>, IComparable where T : IComparable<T>
    {

        #region Declarations
        private readonly T _lowerBound;
        private readonly T _upperBound;
        #endregion

        #region Constructors
        /// <summary>
        /// Creates the range.
        /// </summary>
        /// <param name="lowerBound">The lower bound of the range.</param>
        /// <param name="upperBound">The upper bound of the range.</param>
        internal Range(T lowerBound, T upperBound)
        {
            Verify.NotNull("lowerBound", (object)lowerBound);
            Verify.NotNull("upperBound", (object)upperBound);
            Verify.IsTrue("lowerBound", lowerBound.CompareTo(upperBound) <= 0);

            _lowerBound = lowerBound;
            _upperBound = upperBound;
        }
        #endregion

        #region Properties
        /// <summary>
        /// The start of the range.
        /// </summary>
        public T LowerBound
        {
            get
            {
                return _lowerBound;
            }
        }

        /// <summary>
        /// The upper bound of the range.
        /// </summary>
        public T UpperBound
        {
            get
            {
                return _upperBound;
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Indicates if the range contains <code>value</code>.
        /// </summary>
        /// <param name="value">The value to look for.</param>
        /// <returns>true if the range contains <code>value</code>, false otherwise.</returns>
        /// <exception cref="System.ArgumentNullException"><code>value</code> is null.</exception>
        public bool Contains(T value)
        {
            Verify.NotNull("value", (object)value);

            return ((LowerBound.CompareTo(value) <= 0) && (UpperBound.CompareTo(value) >= 0));
        }

        /// <summary>
        /// Indicates if the range contains <code>value</code>.
        /// </summary>
        /// <param name="value">A range to test.</param>
        /// <returns>true if the entire range in <code>value</code> is within this range.</returns>
        /// <exception cref="System.ArgumentNullException"><code>value</code> is null.</exception>
        public bool Contains(Range<T> value)
        {
            Verify.NotNull("value", value);

            int i1 = LowerBound.CompareTo(value.LowerBound);
            int i2 = UpperBound.CompareTo(value.UpperBound);
            return ((LowerBound.CompareTo(value.LowerBound) <= 0) && (UpperBound.CompareTo(value.UpperBound) >= 0));
        }

        /// <summary>
        /// Indicates if the range overlaps <code>value</code>.
        /// </summary>
        /// <param name="value">A range to test.</param>
        /// <returns>true if any of the range in <code>value</code> is within this range.</returns>
        /// <exception cref="System.ArgumentNullException"><code>value</code> is null.</exception>
        public bool Overlaps(Range<T> value)
        {
            Verify.NotNull("value", value);

            return (Contains(value.LowerBound) || Contains(value.UpperBound) || value.Contains(LowerBound) || value.Contains(UpperBound));
        }

        /// <summary>
        /// Returns the range that represents the intersection of this range and <code>value</code>.
        /// </summary>
        /// <param name="value">The range to intersect with.</param>
        /// <returns>A range that contains the values that are common in both ranges, or null if there is no intersection.</returns>
        /// <exception cref="System.ArgumentNullException"><code>value</code> is null.</exception>
        /// <exception cref="System.ArgumentException"><code>value</code> does not overlap the range.</exception>
        public Range<T> Intersect(Range<T> value)
        {
            Verify.NotNull("value", value);
            Verify.IsTrue("value", Overlaps(value));    // Intersect makes no sense unless there is an overlap

            T start;
            if (LowerBound.CompareTo(value.LowerBound) > 0)
            {
                start = LowerBound;
            }
            else
            {
                start = value.LowerBound;
            }

            if (UpperBound.CompareTo(value.UpperBound) < 0)
            {
                return new Range<T>(start, UpperBound);
            }
            else
            {
                return new Range<T>(start, value.UpperBound);
            }
        }

        /// <summary>
        /// Returns the range that represents the union of this range and <code>value</code>.
        /// </summary>
        /// <param name="value">The range to union with.</param>
        /// <returns>A range that contains both ranges, or null if there is no union.</returns>
        /// <exception cref="System.ArgumentNullException"><code>value</code> is null.</exception>
        /// <exception cref="System.ArgumentException"><code>value</code> is not contiguous with the range.</exception>
        public Range<T> Union(Range<T> value)
        {
            Verify.NotNull("value", value);
            Verify.IsTrue("value", IsContiguousWith(value));    // Union makes no sense unless there is a contiguous border

            // If either one is a subset of the other, then is it the union
            if (this.Contains(value))
            {
                return this;
            }
            else if (value.Contains(this))
            {
                return value;
            }
            else
            {
                T start;
                if (LowerBound.CompareTo(value.LowerBound) < 0)
                {
                    start = LowerBound;
                }
                else
                {
                    start = value.LowerBound;
                }

                if (UpperBound.CompareTo(value.UpperBound) > 0)
                {
                    return new Range<T>(start, UpperBound);
                }
                else
                {
                    return new Range<T>(start, value.UpperBound);
                }
            }
        }

        /// <summary>
        /// Returns a range which contains the current range, minus <code>value</code>.
        /// </summary>
        /// <param name="value">The value to complement the range by.</param>
        /// <returns>The complemented range.</returns>
        /// <exception cref="System.ArgumentNullException"><code>value</code> is null.</exception>
        /// <exception cref="System.ArgumentException">
        /// <code>value</code> is contained by this range, complementing would lead to a split range.
        /// </exception>
        public Range<T> Complement(Range<T> value)
        {
            Verify.NotNull("value", value);
            Verify.IsFalse("value", Contains(value));

            if (Overlaps(value))
            {
                T start;

                // If value's start and end straddle our start, move our start up to be values end.
                if ((LowerBound.CompareTo(value.LowerBound) > 0) && (LowerBound.CompareTo(value.UpperBound) < 0))
                {
                    start = value.UpperBound;
                }
                else
                {
                    start = LowerBound;
                }

                // If value's start and end straddle our end, move our end back down to be values start.
                if ((UpperBound.CompareTo(value.LowerBound) > 0) && (UpperBound.CompareTo(value.UpperBound) < 0))
                {
                    return new Range<T>(start, value.LowerBound);
                }
                else
                {
                    return new Range<T>(start, UpperBound);
                }
            }
            else
            {
                return this;
            }
        }

        /// <summary>
        /// Indicates if this range is contiguous with <code>range</code>.
        /// </summary>
        /// <param name="range">The range to check.</param>
        /// <returns>true if the two ranges are contiguous, false otherwise.</returns>
        /// <remarks>Contiguous can mean containing, overlapping, or being next to.</remarks>
        public bool IsContiguousWith(Range<T> range)
        {
            if (Overlaps(range) || range.Overlaps(this) || range.Contains(this) || Contains(range))
            {
                return true;
            }

            // Once we remove overlapping and containing, only touching if available
            return ((UpperBound.Equals(range.LowerBound)) || (LowerBound.Equals(range.UpperBound)));
        }
        #endregion

        #region Overrides
        /// <summary>
        /// See <see cref="System.Object.ToString"/>.
        /// </summary>
        public override string ToString()
        {
            return "{" + LowerBound + "->" + UpperBound + "}";
        }

        /// <summary>
        /// See <see cref="System.Object.Equals(object)"/>.
        /// </summary>
        public override bool Equals(object obj)
        {
            if (obj is Range<T>)
            {
                Range<T> other = (Range<T>)obj;
                return ((CompareTo(other) == 0) && (UpperBound.CompareTo(other.UpperBound) == 0));
            }

            return false;
        }

        /// <summary>
        /// See <see cref="System.Object.GetHashCode"/>.
        /// </summary>
        public override int GetHashCode()
        {
            return LowerBound.GetHashCode();
        }
        #endregion

        #region Operators
        /// <summary>
        /// Overrides the equals operator.
        /// </summary>
        /// <param name="left">The left range.</param>
        /// <param name="right">The right range.</param>
        /// <returns>true if the two ranges are equal, false otherwise.</returns>
        public static bool operator ==(Range<T> left, Range<T> right)
        {
            // If both are null, or both are same instance, return true.
            if (Object.ReferenceEquals(left, right))
            {
                return true;
            }

            // If one is null, but not both, return false.
            if (((object)left == null) || ((object)right == null))
            {
                return false;
            }

            return (left.CompareTo(right) == 0);
        }

        /// <summary>
        /// Overrides the not equals operator.
        /// </summary>
        /// <param name="left">The left range.</param>
        /// <param name="right">The right range.</param>
        /// <returns>true if the two ranges are equal, false otherwise.</returns>
        /// <summary>
        /// Overrides the equals operator.
        /// </summary>
        /// <returns>true if the two ranges are equal, false otherwise.</returns>
        public static bool operator !=(Range<T> left, Range<T> right)
        {
            return !(left == right);
        }

        /// <summary>
        /// Overrides the greater than operator.
        /// </summary>
        /// <param name="left">The left range.</param>
        /// <param name="right">The right range.</param>
        /// <returns>true if the <code>left</code> is greater than <code>right</code>, false otherwise.</returns>
        public static bool operator >(Range<T> left, Range<T> right)
        {
            return (left.CompareTo(right) > 0);
        }

        /// <summary>
        /// Overrides the less than operator.
        /// </summary>
        /// <param name="left">The left range.</param>
        /// <param name="right">The right range.</param>
        /// <returns>true if the <code>left</code> is less than <code>right</code>, false otherwise.</returns>
        public static bool operator <(Range<T> left, Range<T> right)
        {
            return (left.CompareTo(right) < 0);
        }

        /// <summary>
        /// Overrides the greater than operator.
        /// </summary>
        /// <param name="left">The left range.</param>
        /// <param name="right">The right range.</param>
        /// <returns>true if the <code>left</code> is greater than <code>right</code>, false otherwise.</returns>
        public static bool operator >(Range<T> left, T right)
        {
            return (left.CompareTo(right) > 0);
        }

        /// <summary>
        /// Overrides the less than operator.
        /// </summary>
        /// <param name="left">The left range.</param>
        /// <param name="right">The right range.</param>
        /// <returns>true if the <code>left</code> is less than <code>right</code>, false otherwise.</returns>
        public static bool operator <(Range<T> left, T right)
        {
            return (left.CompareTo(right) < 0);
        }

        /// <summary>
        /// Overrides the greater than or equal operator.
        /// </summary>
        /// <param name="left">The left range.</param>
        /// <param name="right">The right range.</param>
        /// <returns>true if the <code>left</code> is greater than or equal to <code>right</code>, false otherwise.</returns>
        public static bool operator >=(Range<T> left, Range<T> right)
        {
            return (left.CompareTo(right) >= 0);
        }

        /// <summary>
        /// Overrides the less than or equal to operator.
        /// </summary>
        /// <param name="left">The left range.</param>
        /// <param name="right">The right range.</param>
        /// <returns>true if the <code>left</code> is less than or equal to <code>right</code>, false otherwise.</returns>
        public static bool operator <=(Range<T> left, Range<T> right)
        {
            return (left.CompareTo(right) <= 0);
        }

        /// <summary>
        /// Overrides the greater than or equals operator.
        /// </summary>
        /// <param name="left">The left range.</param>
        /// <param name="right">The right range.</param>
        /// <returns>true if the <code>left</code> is greater than or equal to <code>right</code>, false otherwise.</returns>
        public static bool operator >=(Range<T> left, T right)
        {
            return (left.CompareTo(right) >= 0);
        }

        /// <summary>
        /// Overrides the less than or equals operator.
        /// </summary>
        /// <param name="left">The left range.</param>
        /// <param name="right">The right range.</param>
        /// <returns>true if the <code>left</code> is less than or equal to <code>right</code>, false otherwise.</returns>
        public static bool operator <=(Range<T> left, T right)
        {
            return (left.CompareTo(right) <= 0);
        }

        /// <summary>
        /// The complement operator.
        /// </summary>
        /// <param name="left">The left operand.</param>
        /// <param name="right">The right operand.</param>
        /// <returns>The complement of <code>left</code> and <code>right</code>.</returns>
        public static Range<T> operator ^(Range<T> left, Range<T> right)
        {
            return left.Complement(right);
        }

        /// <summary>
        /// The union operator.
        /// </summary>
        /// <param name="left">The left operand.</param>
        /// <param name="right">The right operand.</param>
        /// <returns>The union of <code>left</code> and <code>right</code>.</returns>
        public static Range<T> operator |(Range<T> left, Range<T> right)
        {
            return left.Union(right);
        }

        /// <summary>
        /// The intersection operator.
        /// </summary>
        /// <param name="left">The left operand.</param>
        /// <param name="right">The right operand.</param>
        /// <returns>The intersection of <code>left</code> and <code>right</code>.</returns>
        public static Range<T> operator &(Range<T> left, Range<T> right)
        {
            return left.Intersect(right);
        }
        #endregion

        #region IComparable<Range<T>> Members
        /// <summary>
        /// See <see cref="System.IComparable{T}.CompareTo"/>.
        /// </summary>
        public int CompareTo(Range<T> other)
        {
            return LowerBound.CompareTo(other.LowerBound);
        }
        #endregion

        #region IComparable<T> Members
        /// <summary>
        /// See <see cref="System.IComparable{T}.CompareTo"/>.
        /// </summary>
        public int CompareTo(T other)
        {
            return LowerBound.CompareTo(other);
        }
        #endregion

        #region IComparable Members
        /// <summary>
        /// See <see cref="System.IComparable.CompareTo"/>.
        /// </summary>
        public int CompareTo(object obj)
        {
            if (obj is Range<T>)
            {
                Range<T> other = (Range<T>)obj;
                return CompareTo(other);
            }
            else if (obj is T)
            {
                T other = (T)obj;
                return CompareTo(other);
            }

            throw new InvalidOperationException(string.Format("Cannot compare to {0}", obj));
        }
        #endregion

    }

    /// <summary>
    /// Contains a set of range methods.
    /// </summary>
    public static class Range
    {

        #region Creation
        /// <summary>
        /// Creates an inclusive range.
        /// </summary>
        /// <typeparam name="T">The type of range.</typeparam>
        /// <param name="from">The from value.</param>
        /// <param name="to">The to value.</param>
        /// <returns>The range.</returns>
        /// <exception cref="System.ArgumentNullException"><code>from</code> or <code>to</code> is null.</exception>
        public static Range<T> Create<T>(T from, T to) where T : IComparable<T>
        {
            Verify.NotNull("from", (object)from);
            Verify.NotNull("to", (object)to);

            return new Range<T>(from, to);
        }

        #endregion

        #region Enumerables
        /// <summary>
        /// Indicates if the ranges contain the range.
        /// </summary>
        /// <typeparam name="T">The range type.</typeparam>
        /// <param name="ranges">The ranges to look in.</param>
        /// <param name="range">The item to look for.</param>
        /// <returns>true if <code>ranges</code> contain <code>range</code>, false otherwise.</returns>
        /// <exception cref="System.ArgumentNullException"><code>ranges</code> or <code>range</code> is null.</exception>
        public static bool Contains<T>(this IEnumerable<Range<T>> ranges, Range<T> range) where T : IComparable<T>
        {
            Verify.NotNull("ranges", ranges);
            Verify.NotNull("item", (object)range);

            foreach (Range<T> item in ranges.Overlapped(range).Coalesce())
            {
                if (item.Contains(range))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Sorts the ranges.
        /// </summary>
        /// <typeparam name="T">The type of range.</typeparam>
        /// <param name="ranges">The sorted ranges.</param>
        /// <returns>The sorted ranges.</returns>
        /// <exception cref="System.ArgumentNullException"><code>ranges</code> is null.</exception>
        public static IEnumerable<Range<T>> Sort<T>(this IEnumerable<Range<T>> ranges) where T : IComparable<T>
        {
            Verify.NotNull("ranges", ranges);

            List<Range<T>> list = new List<Range<T>>(ranges);
            list.Sort();
            return list;
        }
        
        /// <summary>
        /// Coaleses the ranges.
        /// </summary>
        /// <typeparam name="T">The range key type.</typeparam>
        /// <param name="ranges">The ranges to coalesce.</param>
        /// <returns>The coalesced ranges.</returns>
        /// <exception cref="System.ArgumentNullException"><code>ranges</code> is null.</exception>
        public static IEnumerable<Range<T>> Coalesce<T>(this IEnumerable<Range<T>> ranges) where T : IComparable<T>
        {
            Verify.NotNull("ranges", ranges);

            Range<T> previous = null;
            foreach (Range<T> item in ranges.Sort())
            {
                if (previous == null)
                {
                    previous = item;
                }
                else
                {
                    // Possible coalescing
                    if (previous.IsContiguousWith(item))
                    {
                        // Intersect the ranges
                        previous = previous | item;
                    }
                    else
                    {
                        yield return previous;
                        previous = item;
                    }
                }
            }

            if (previous != null)
            {
                yield return previous;
            }
        }
        
        /// <summary>
        /// Fetches the ranges which are overlapped by this range.
        /// </summary>
        /// <typeparam name="T">The type of range.</typeparam>
        /// <param name="ranges">The ranges.</param>
        /// <param name="range">The range to test for overlappping.</param>
        /// <returns>The overlapped ranges.</returns>
        /// <exception cref="System.ArgumentNullException"><code>ranges</code> or <code>range</code> is null.</exception>
        public static IEnumerable<Range<T>> Overlapped<T>(this IEnumerable<Range<T>> ranges, Range<T> range) where T : IComparable<T>
        {
            Verify.NotNull("ranges", ranges);
            Verify.NotNull("range", range);

            foreach (Range<T> item in ranges)
            {
                if (item.Overlaps(range))
                {
                    yield return item;
                }
            }
        }
        
        #endregion
    }

    [DebuggerStepThrough]
    [ExcludeFromCodeCoverage]
    internal static class Verify
    {

        #region Methods
        /// <summary>
        /// Asserts that a condition is fase.
        /// </summary>
        /// <param name="paramName">The parameter name.</param>
        /// <param name="condition">The condition result.</param>
        /// <exception cref="System.ArgumentException">If <code>condition</code> is true.</exception>
        public static void IsFalse(string paramName, bool condition)
        {
            if (condition)
                throw new ArgumentException(paramName);
        }

        /// <summary>
        /// Asserts that a condition is true.
        /// </summary>
        /// <param name="paramName">The parameter name.</param>
        /// <param name="condition">The condition result.</param>
        /// <exception cref="System.ArgumentException">If <code>condition</code> is false.</exception>
        public static void IsTrue(string paramName, bool condition)
        {
            if (!condition)
                throw new ArgumentException(paramName);
        }

        /// <summary>
        /// Asserts that <code>value</code> is not null.
        /// </summary>
        /// <param name="paramName">The name of the parameter to assert.</param>
        /// <param name="value">The value to test.</param>
        /// <exception cref="System.ArgumentNullException">If <code>value</code> is null.</exception>
        public static void NotNull(string paramName, object value)
        {
            if (value == null)
                throw new ArgumentNullException(paramName);
        }

        #endregion

    }
}