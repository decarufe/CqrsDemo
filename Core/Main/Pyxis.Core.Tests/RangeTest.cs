//using Sanity;

using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace Pyxis.Core.Tests
{
    /// <summary>
    ///This is a test class for RangeTest and is intended
    ///to contain all RangeTest Unit Tests
    ///</summary>
    [TestFixture()]
    public class RangeTest
    {
        #region ctor
        [Test()]
        public void ctor()
        {
            Range<int> range = Range.Create<int>(0, 2);
            Assert.AreEqual(0, range.LowerBound);
            Assert.AreEqual(2, range.UpperBound);
        }

        [Test(), ExpectedException(typeof(ArgumentException))]
        public void ctorReverse()
        {
            Range<int> range = Range.Create<int>(2, 0);
        }

        [Test()]
        public void ctorNullStart()
        {
            Assert.Throws<ArgumentNullException>(() => Range.Create<string>(null, "Hello"));
        }

        [Test(), ExpectedException(typeof(ArgumentNullException))]
        public void ctorNullEnd()
        {
            Range<string> range = Range.Create<string>("Hello", null);
        }
        #endregion

        #region Comparisons
        [Test]
        public void Comparisons()
        {
            Assert.IsTrue(Range.Create(1, 2) > Range.Create(0, 3));                  // {1->2} > {0->3}
            Assert.IsTrue(Range.Create(1, 2) == Range.Create(1, 3));                 // {1->2} == {1->3}
            Assert.IsTrue(Range.Create(0, 3) < Range.Create(1, 2));                  // {0->3} < {1->2}

            Assert.IsTrue(Range.Create(1, 2).CompareTo(Range.Create(0, 3)) > 0);     // {1->2} > {0->3}
            Assert.IsTrue(Range.Create(1, 2).CompareTo(Range.Create(1, 3)) == 0);    // {1->2} == {1->3}
            Assert.IsTrue(Range.Create(0, 3).CompareTo(Range.Create(1, 2)) < 0);     // {0->3} < {1->2}
        }
        #endregion

        #region Contains
        /// <summary>
        /// Test contains.
        /// </summary>
        [Test]
        public void Contains()
        {
            Assert.IsTrue(Range.Create(0, 9).Contains(0));   // 0-9 contains 0
            Assert.IsTrue(Range.Create(0, 9).Contains(1));   // 0-9 contains 1
            Assert.IsTrue(Range.Create(0, 9).Contains(9));   // 0-9 contains 9
            Assert.IsTrue(Range.Create(0, 9).Contains(8));   // 0-9 contains 8
            Assert.IsFalse(Range.Create(0, 9).Contains(-1)); // 0-9 !contains -1
            Assert.IsFalse(Range.Create(0, 9).Contains(10)); // 0-9 !contains 10
        }
        
        /// <summary>
        /// Test contains.
        /// </summary>
        [Test]
        public void ContainsRange()
        {
            Assert.IsTrue(Range.Create(0, 9).Contains(Range.Create(0, 9)));      // 0-9 contains 0-9
            Assert.IsFalse(Range.Create(0, 9).Contains(Range.Create(0, 10)));    // 0-9 !contains 0-10
            Assert.IsFalse(Range.Create(0, 9).Contains(Range.Create(-1, 9)));    // 0-9 !contains -1-9
            Assert.IsFalse(Range.Create(0, 9).Contains(Range.Create(-1, 10)));   // 0-9 !contains -1-10
            Assert.IsFalse(Range.Create(0, 9).Contains(Range.Create(-2, 15)));   // 0-9 !contains -2-15

            Assert.IsTrue(Range.Create(0, 10).Contains(Range.Create(0, 9)));     // 0-10 contains 0-9
            Assert.IsTrue(Range.Create(-1, 9).Contains(Range.Create(0, 9)));     // -1-9 contains 0-9
            Assert.IsTrue(Range.Create(-1, 10).Contains(Range.Create(0, 9)));    // -1-10 contains 0-9
            Assert.IsTrue(Range.Create(-2, 15).Contains(Range.Create(0, 9)));    // -2-15 contains 0-9
        }
        
        /// <summary>
        /// Test contains null.
        /// </summary>
        [Test, ExpectedException(typeof(ArgumentNullException))]
        public void ContainsNull()
        {
            Range.Create("Test", "Test2").Contains((string) null);
        }

        [Test]
        public void ContainsRanges()
        {
            var ranges = new List<Range<int>>();
            ranges.Add(Range.Create(0, 9));
            ranges.Add(Range.Create(9, 14));
            ranges.Add(Range.Create(14, 16));
            ranges.Add(Range.Create(16, 19));

            Assert.IsTrue(Range.Contains(ranges, Range.Create(6, 13)));
            Assert.IsTrue(Range.Contains(ranges, Range.Create(6, 17)));
            Assert.IsFalse(Range.Contains(ranges, Range.Create(6, 20)));
        }
        #endregion

        #region Overlaps
        [Test]
        public void Overlaps()
        {
            Assert.IsTrue(Range.Create(0, 9).Overlaps(Range.Create(-10, 1)));        // 0-9 overlaps -10-1
            Assert.IsTrue(Range.Create(0, 9).Overlaps(Range.Create(-10, 0)));        // 0-9 overlaps -10-0
            Assert.IsFalse(Range.Create(0, 9).Overlaps(Range.Create(-10, -1)));      // 0-9 !overlaps -10--1

            Assert.IsTrue(Range.Create(-10, 1).Overlaps(Range.Create(0, 0)));        // -10-1 overlaps 0-9
            Assert.IsTrue(Range.Create(-10, 0).Overlaps(Range.Create(0, 0)));        // -10-0 overlaps 0-9
            Assert.IsFalse(Range.Create(-10, -1).Overlaps(Range.Create(0, 0)));      // -10--1 !overlaps 0-9
        }
        #endregion

        #region Intersect
        [Test]
        public void Intersect()
        {
            Assert.AreEqual("{5->9}", Range.Create(0, 9).Intersect(Range.Create(5, 15)).ToString());
            Assert.AreEqual("{0->9}", Range.Create(0, 9).Intersect(Range.Create(-2, 15)).ToString());
            Assert.AreEqual("{9->9}", Range.Create(0, 9).Intersect(Range.Create(9, 12)).ToString());

            Assert.AreEqual("{5->9}", Range.Create(5, 15).Intersect(Range.Create(0, 9)).ToString());
            Assert.AreEqual("{0->9}", Range.Create(-2, 15).Intersect(Range.Create(0, 9)).ToString());
            Assert.AreEqual("{9->9}", Range.Create(9, 12).Intersect(Range.Create(0, 9)).ToString());
        }

        [Test, ExpectedException(typeof(ArgumentException))]
        public void IntersectNonContiguous()
        {
            Range.Create(0, 9).Intersect(Range.Create(10, 12));
        }
        #endregion

        #region Union
        [Test]
        public void Union()
        {
            Assert.AreEqual("{0->15}", Range.Create(0, 9).Union(Range.Create(5, 15)).ToString());
            Assert.AreEqual("{-2->15}", Range.Create(0, 9).Union(Range.Create(-2, 15)).ToString());
            Assert.AreEqual("{0->12}", Range.Create(0, 9).Union(Range.Create(9, 12)).ToString());

            Assert.AreEqual("{0->15}", Range.Create(0, 15).Union(Range.Create(0, 0)).ToString());
            Assert.AreEqual("{-2->15}", Range.Create(-2, 15).Union(Range.Create(0, 9)).ToString());
            Assert.AreEqual("{0->12}", Range.Create(9, 12).Union(Range.Create(0, 9)).ToString());
        }
        #endregion

        #region Complement
        [Test]
        public void Complement()
        {
            Assert.AreEqual("{3->9}", Range.Create(0, 9).Complement(Range.Create(-5, 3)).ToString());
            Assert.AreEqual("{0->6}", Range.Create(0, 9).Complement(Range.Create(6, 12)).ToString());
            Assert.AreEqual("{0->9}", Range.Create(0, 9).Complement(Range.Create(11, 12)).ToString());

            Assert.AreEqual("{-5->0}", Range.Create(-5, 3).Complement(Range.Create(0, 9)).ToString());
            Assert.AreEqual("{9->12}", Range.Create(6, 12).Complement(Range.Create(0, 9)).ToString());
            Assert.AreEqual("{11->12}", Range.Create(11, 12).Complement(Range.Create(0, 9)).ToString());

        }
        #endregion

        #region IsContiguousWith
        [Test]
        public void IsContiguousWith()
        {
            Assert.IsTrue(Range.Create(0, 9).IsContiguousWith(Range.Create(-1, 10)));
            Assert.IsTrue(Range.Create(0, 9).IsContiguousWith(Range.Create(5, 10)));
            Assert.IsTrue(Range.Create(0, 9).IsContiguousWith(Range.Create(5, 6)));
            Assert.IsTrue(Range.Create(0, 9).IsContiguousWith(Range.Create(9, 10)));
            Assert.IsTrue(Range.Create(0, 9).IsContiguousWith(Range.Create(9, 10)));
            Assert.IsTrue(Range.Create(0, 9).IsContiguousWith(Range.Create(-3, 0)));
            Assert.IsTrue(Range.Create(0, 9).IsContiguousWith(Range.Create(-3, 0)));
            Assert.IsFalse(Range.Create(0, 9).IsContiguousWith(Range.Create(10, 11)));
            Assert.IsFalse(Range.Create(0, 9).IsContiguousWith(Range.Create(-3, -1)));
        }
        #endregion

        #region Coalesce
        [Test]
        public void Coalesce()
        {
            var ranges = new List<Range<int>>();
            ranges.Add(Range.Create(0, 5));
            ranges.Add(Range.Create(5, 6));
            ranges.Add(Range.Create(6, 7));
            ranges.Add(Range.Create(9, 10));
            ranges.Add(Range.Create(10, 11));

            var result = new List<Range<int>>(Range.Coalesce(ranges));
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual("{0->7}", result[0].ToString());
            Assert.AreEqual("{9->11}", result[1].ToString());

            ranges.Clear();
            ranges.Add(Range.Create(5, 5));
            ranges.Add(Range.Create(5, 6));
            ranges.Add(Range.Create(9, 10));
            ranges.Add(Range.Create(10, 10));
            result = new List<Range<int>>(Range.Coalesce(ranges));
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual("{5->6}", result[0].ToString());
            Assert.AreEqual("{9->10}", result[1].ToString());
        }
        #endregion

        #region Overlapped
        [Test]
        public void Overlapped()
        {
            var ranges = new List<Range<int>>();
            ranges.Add(Range.Create(0, 10));
            ranges.Add(Range.Create(13, 16));
            ranges.Add(Range.Create(15, 22));
            ranges.Add(Range.Create(24, 26));

            var result = new List<Range<int>>(Range.Overlapped(ranges, Range.Create(9, 21)));
            Assert.AreEqual(3, result.Count);
            Assert.AreEqual("{0->10}", result[0].ToString());
            Assert.AreEqual("{13->16}", result[1].ToString());
            Assert.AreEqual("{15->22}", result[2].ToString());
        }
        #endregion

        #region Compare

        [Test]
        public void Copmare()
        {
            Assert.AreEqual(Range.Create(1, 3), Range.Create(1, 3));
            Assert.AreNotEqual(Range.Create(1, 5), Range.Create(1, 3));
            Assert.AreNotEqual(Range.Create(2, 3), Range.Create(1, 3));
            Assert.AreNotEqual(Range.Create(1, 3), 2);
        }

        #endregion

        #region Operator

        [Test]
        public void OperatorTests()
        {
            Assert.AreEqual("{3->9}", (Range.Create(0, 9) ^ Range.Create(-5, 3)).ToString());
            Assert.IsTrue(Range.Create(5, 9) > 3);
            Assert.IsTrue(Range.Create(5, 9) < 15);
            Assert.IsTrue(Range.Create(5, 9) >= 5);
            Assert.IsTrue(Range.Create(5, 9) <= 9);
            Assert.IsTrue(Range.Create(0, 9) >= Range.Create(0, 15));
            Assert.IsFalse(Range.Create(0, 9) >= Range.Create(5, 15));
            Assert.IsTrue(Range.Create(-5, 9) <= Range.Create(0, 9));
            Assert.IsFalse(Range.Create(0, 9) <= Range.Create(-5, 5));
            Assert.AreEqual("{5->9}", (Range.Create(0, 9) & Range.Create(5, 15)).ToString());
        }

        #endregion

    }
}