using System;
using System.Diagnostics;
using System.Text;

namespace Pyxis.Core.Id
{
    public class TimeBasedIdGenerator : IIdGenerator
    {
        private const int Base = 26;
        private readonly Stopwatch _stopwatch = new Stopwatch();
        
        public TimeBasedIdGenerator()
        {
            _stopwatch.Start();
        }

        private static string ConvertToBase26(ulong i)
        {
            var result = new StringBuilder();

            while (i > 0)
            {
                ulong remainder = i % Base;
                i /= Base;
                result.Insert(0, (char)((char)remainder + 'A'));
            };

            return result.ToString();
        }

        public string GenerateId(string seed)
        {
            if (string.IsNullOrWhiteSpace(seed)) return seed;
            var casted = (uint)seed.GetHashCode();
            casted += GetTimeVariant();
            var stringId = casted.ToString().PadLeft(9, '0');
            var prefixString = stringId.Substring(0, stringId.Length - 6);
            var prefix = Convert.ToUInt64(prefixString);
            var partOne = ConvertToBase26(prefix);
            var partTwo = stringId.Substring(prefixString.Length);
            return partOne.PadLeft(3, 'A') + partTwo.PadLeft(6,'0');
        }
        
        private uint GetTimeVariant()
        {
            return (uint)DateTime.Now.TimeOfDay.TotalMilliseconds / 10;
        }
    }
}
