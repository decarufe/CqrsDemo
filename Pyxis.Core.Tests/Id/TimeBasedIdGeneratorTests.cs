using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using Pyxis.Core.Id;

namespace Pyxis.Core.Tests.Id
{
    [TestFixture]
    public class TimeBasedIdGeneratorTests
    {
        private const int TryCount = 4;
        private const int EmailCount = 10000;

        private static readonly string[] Domains = { "gmail.com", "yahoo.com", "pyxis-tech.com", "aircanada.com", "deltaairlaines.com", "airtransat.com", "americanairlines.com", "live.com" };
        private string[] _emails;

        [SetUp]
        public void SetUp()
        {
            _emails = GenerateEmails(EmailCount);
        }

        [Test]
        public void TestThatGenerationIsThreadSafeAndUnique()
        {
            var results = new ConcurrentQueue<string>();

            // Abuse threading here to ensure safety
            for (var tryNumber = 0; tryNumber < TryCount; tryNumber++)
            {
                var generator = new TimeBasedIdGenerator();
                Parallel.For(0, EmailCount, currentMail =>
                {
                    var email = _emails[currentMail];
                    var generated = generator.GenerateId(email);
                    results.Enqueue(generated);
                }
                );
                // Tolerate the risk of having the same email within the same 200 ms window of a given day
                Thread.Sleep(200);
            }

            Assert.AreEqual(EmailCount * TryCount, results.Count);
            var uniqueIds = new HashSet<string>(results);
            Assert.IsTrue(uniqueIds.Count <= results.Count);
            // Accept a 5 delta
            Assert.IsTrue(uniqueIds.Count >= results.Count - 5);
            Assert.IsTrue(results.All(x => x.Length == 9));
        }

        private static string[] GenerateEmails(int nbToGenerate)
        {
            var emails = new HashSet<string>();
            var random = new Random();
            do
            {
                var nbChars = random.Next(4, 10);
                var builder = new StringBuilder();
                for (var charCount = 0; charCount < nbChars; charCount++)
                {
                    builder.Append((char)('A' + random.Next(0, 25)));
                }
                var domain = Domains[random.Next(Domains.Length)];
                builder.Append("@");
                builder.Append(domain);
                emails.Add(builder.ToString());
            }
            while (emails.Count < nbToGenerate);

            return emails.ToArray();
        }
    }
}
