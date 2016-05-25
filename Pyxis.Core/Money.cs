using System;
using System.Globalization;
using System.Threading;

namespace Pyxis.Core
{
    [Serializable]
    public class Money
    {
        public decimal Value
        {
            get { return Math.Round(_internalValue, 2, MidpointRounding.AwayFromZero); }
            set { _internalValue = value; }
        }

        private decimal _internalValue;
        protected string DecimalSeparator { get; set; }

        public Money()
            : this(0)
        {
        }
        public static Money operator +(Money value1, string value2)
        {
            return new Money(value1._internalValue + ConvertDecimalString(value2, value1.DecimalSeparator));
        }

        public static Money operator +(Money value1, Money value2)
        {
            return new Money(value1._internalValue + value2._internalValue);
        }

        public static Money operator -(Money value1, string value2)
        {
            return new Money(value1._internalValue - ConvertDecimalString(value2, value1.DecimalSeparator));
        }

        public static Money operator -(Money value1, Money value2)
        {
            return new Money(value1._internalValue - value2._internalValue);
        }

        public static Money operator *(Money value1, string value2)
        {
            return new Money(value1._internalValue * ConvertDecimalString(value2, value1.DecimalSeparator));
        }

        public static Money operator *(Money value1, int value2)
        {
            return new Money(value1._internalValue * value2);
        }

        public static Money operator /(Money value1, string value2)
        {
            return new Money(value1._internalValue / ConvertDecimalString(value2, value1.DecimalSeparator));
        }

        public static Money operator /(Money value1, int value2)
        {
            return new Money(value1._internalValue / value2);
        }

        public Money(string value, bool round = false)
            : this(value, Thread.CurrentThread.CurrentCulture.NumberFormat.CurrencyDecimalSeparator, round)
        {
        }

        public Money(string value, string decimalSeparator, bool round = false)
        {
            DecimalSeparator = decimalSeparator;
            _internalValue = ConvertDecimalString(value, DecimalSeparator);
            if (round)
            {
                _internalValue = Math.Round(_internalValue, 2);
            }
        }

        public Money(decimal value, bool round = false)
            : this(value, Thread.CurrentThread.CurrentCulture.NumberFormat.CurrencyDecimalSeparator, round)
        {
        }

        public Money(decimal value, string decimalSeparator, bool round = false)
        {
            DecimalSeparator = decimalSeparator;
            _internalValue = round ? Math.Round(value, 2) : value;
        }

        public static decimal ConvertDecimalString(string currentString, string decimalSeparator)
        {
            var culture = Thread.CurrentThread.CurrentCulture;
            try
            {
                Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("en-US");
                return decimal.Parse(currentString.Replace(decimalSeparator, "."), NumberStyles.Currency);
            }
            finally
            {
                Thread.CurrentThread.CurrentCulture = culture;
            }

        }

        public override string ToString()
        {
            return Value.ToString("#0.00");
        }

        public override bool Equals(object obj)
        {
            if (obj == null || obj.GetType() != typeof(Money))
            {
                return false;
            }
            return Value == ((Money)obj).Value;
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }
    }
}
