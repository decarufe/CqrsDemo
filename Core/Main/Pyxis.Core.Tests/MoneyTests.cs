using NUnit.Framework;

namespace Pyxis.Core.Tests
{
    [TestFixture]
    public class MoneyTests
    {
        [Test]
        public void TestConstructorSetValueTo0()
        {
            var baseAmount = new Money();
            Assert.AreEqual(0, baseAmount.Value);
        }

        [Test]
        public void TestConstructorReceivesMoneyAsString()
        {
            var baseAmount = new Money("12.25");
            Assert.AreEqual(12.25m, baseAmount.Value);
        }

        [Test]
        public void TestThatTwoAmountsCanBeAdded()
        {
            var baseAmount = new Money("5");
            var result = baseAmount + new Money("10");
            Assert.AreEqual(15m, result.Value);
        }

        [Test]
        public void TestThatAMoneyCanAddAString()
        {
            var baseAmount = new Money("5");
            var result = baseAmount + "10";
            Assert.AreEqual(15m, result.Value);
        }

        [Test]
        public void TestThatTwoAmountsCanBeSubstracted()
        {
            var baseAmount = new Money("10");
            var result = baseAmount - new Money("5");
            Assert.AreEqual(5m, result.Value);
        }

        [Test]
        public void TestThatAMoneyCanBeSubstractedFromAString()
        {
            var baseAmount = new Money("10");
            var result = baseAmount - "5";
            Assert.AreEqual(5m, result.Value);
        }
        
        [Test]
        public void TestThatTwoAmountsCanBeMutipliedAndRoundedAboveFive()
        {
            var baseAmount = new Money("5");
            var result = baseAmount * "1.333";
            Assert.AreEqual(6.67m, result.Value);
            Assert.AreEqual("6.67", result.ToString());
        }
        
        [Test]
        public void TestThatTwoAmountsCanBeMutipliedAndRoundedBelowFive()
        {
            var baseAmount = new Money("5");
            var result = baseAmount * "1.2345";
            Assert.AreEqual(6.17m, result.Value);
            Assert.AreEqual("6.17", result.ToString());
        }

        [Test]
        public void TestThatTwoAmountsCanBeDividedAndRoundedAboveFive()
        {
            var baseAmount = new Money("1233");
            var result = baseAmount / "1000";
            Assert.AreEqual(1.23m, result.Value);
            Assert.AreEqual("1.23", result.ToString());
        }

        [Test]
        public void TestThatTwoAmountsCanBeDividedAndRoundedBelowFive()
        {
            var baseAmount = new Money("1235");
            var result = baseAmount / 1000;
            Assert.AreEqual(1.24m, result.Value);
            Assert.AreEqual("1.24", result.ToString());
        }

        [Test]
        public void TestConstructorCanRound()
        {
            var amount = new Money("12,357", ",", true);
            Assert.AreEqual(new decimal(12.36), amount.Value);
        }

        [Test]
        public void TestValuecanBeSet()
        {
            var amount = new Money();
            amount.Value = 123;
            Assert.AreEqual(123, amount.Value);
        }

        [Test]
        public void TestThatTwoAmountsAreEqualsAsString()
        {
            var amount1 = new Money("1235");
            var amount2 = new Money("1235");
            Assert.AreEqual(amount1, amount2);
        }

        [Test]
        public void TestThatTwoAmountsAreEqualsAsDecimal()
        {
            var amount1 = new Money(new decimal(1235));
            var amount2 = new Money(new decimal(1235));
            Assert.AreEqual(amount1, amount2);
        }

        [Test]
        public void TestThatTwoAmountsAreEqualsAsCombination()
        {
            var amount1 = new Money("1235");
            var amount2 = new Money(new decimal(1235));
            Assert.AreEqual(amount1, amount2);
            Assert.AreEqual(amount2, amount1);
        }

        [Test]
        public void TestThatMoneyCanBeMultiplied()
        {
            var value = new Money("2") * 2;
            Assert.IsTrue(new Money(4).Equals(value));
        }

        [Test]
        public void TestThatDecimalSeparatorCanBeSupplied()
        {
            var amount = new Money("12,35", ",");
            Assert.AreEqual(new decimal(12.35), amount.Value);
            amount = new Money("12,350,000.25", ".");
            Assert.AreEqual(new decimal(12350000.25), amount.Value);
        }

        [Test]
        public void TestThatDecimalAreAlwaysRepresentedBy2Digits()
        {
            var amount = new Money("12,3", ",");
            Assert.AreEqual(new decimal(12.30), amount.Value);
            Assert.AreEqual("12.30", amount.ToString());
        }

        [Test]
        public void TestThatZeroIsFormattedCorrectly()
        {
            var amount = new Money(0);
            Assert.AreEqual("0.00", amount.ToString());
        }

        [Test]
        public void TestThatNegativeAmountIsFormattedCorrectly()
        {
            var amount = new Money(-10);
            Assert.AreEqual("-10.00", amount.ToString());
            amount = new Money(-.10m);
            Assert.AreEqual("-0.10", amount.ToString());
        }
    }
}
