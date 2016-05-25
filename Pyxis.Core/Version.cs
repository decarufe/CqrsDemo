using System;

namespace Pyxis.Core
{
    public class Version
    {
        protected bool Equals(Version other)
        {
            return Value == other.Value;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((Version) obj);
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        private const string FormatMask = "yyyyMMddhhmmss";

        public Version() : this(DateTime.Now)
        {
        }

        public Version(string value)
        {
            Value = Convert.ToInt64(value);
        }

        public Version(long value)
        {
            Value = value;
        }

        public Version(DateTime source)
        {
            Value = Convert.ToInt64(source.ToString(FormatMask));
        }

        public long Value { get; private set; }

        public static string ToString(DateTime version)
        {
            return version.ToString(FormatMask);
        }

        public static string ToLong(DateTime version)
        {
            return version.ToString(FormatMask);
        }

        public static bool operator >(Version c1, Version c2)
        {
            return c1.Value > c2.Value;
        }

        public static bool operator <(Version c1, Version c2)
        {
            return c1.Value < c2.Value;
        }
    }
}
