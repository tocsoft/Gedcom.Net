using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Gedcom.Net.Tests
{
    [TestFixture]
    public class DateTests
    {
        IEnumerable<object[]> TestCases
        {
            get
            {
                yield return new object[] { "1989", new DateTime(1989, 1, 1), new DateTime(1989, 12, 31) };
                yield return new object[] { "JAN 1989", new DateTime(1989, 1, 1), new DateTime(1989, 1, 31) };
                yield return new object[] { "21 JAN 1989", new DateTime(1989, 1, 21), new DateTime(1989, 1, 21) };
                yield return new object[] { "21 1989", new DateTime(1989, 1, 21), new DateTime(1989, 12, 21) };
                yield return new object[] { "21", new DateTime(0001, 1, 21), new DateTime(9999, 12, 21) };

                // cases insensitive
                yield return new object[] { "Jan 1989", new DateTime(1989, 1, 1), new DateTime(1989, 1, 31) };
                yield return new object[] { "21 jan 1989", new DateTime(1989, 1, 21), new DateTime(1989, 1, 21) };

                //date ranges
                yield return new object[] { "FROM Jan 1989 TO JAN 1996", new DateTime(1989, 1, 1), new DateTime(1996, 1, 31) };

                yield return new object[] { "FROM 1989 TO 4 FEB 1996", new DateTime(1989, 1, 1), new DateTime(1996, 2, 4) };


                yield return new object[] { "BEF 1828", new DateTime(0001, 1, 1), new DateTime(1828, 01, 1) };
                yield return new object[] { "BEFor 1828", new DateTime(0001, 1, 1), new DateTime(1828, 01, 1) };
            }
        }

        [Test]
        [TestCaseSource("TestCases")]
        public void CanParseDates(string valueString, DateTime expectedFrom, DateTime expectedTo)
        {
            var result = new Date(valueString);

            Assert.AreEqual(expectedFrom, result.From);
            Assert.AreEqual(expectedTo, result.To);
        }

        [Test]
        public void ThrowsWhenUsingFrenchDatesForMonthFragments()
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("fr-FR");//set to french

            Assert.Throws<System.FormatException>(() =>
            {
                var result = new Date("01 MAI 2009");
            });
        }

        [Test]
        public void DoesntThrowWhenUsingEnglishDatesForMonthFragments()
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("fr-FR");//set to french

            Assert.DoesNotThrow(() =>
            {
                var result = new Date("01 MAY 2009");
            });
        }
    }
}
