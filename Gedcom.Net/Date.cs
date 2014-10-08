using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gedcom.Net.FileDom;

namespace Gedcom.Net
{
    public class Date
    {
        private FileNode _node;

        static Dictionary<string, int> _monthLookUp = new Dictionary<string, int>();

        static Date()
        {
            for (var i = 1; i <= 12; i++)
            {
                var d = new DateTime(2000, i, 1);
                var nameShort = d.ToString("MMM", CultureInfo.InvariantCulture).ToUpperInvariant();
                _monthLookUp.Add(nameShort, i);
            }
        }

        internal Date(string value)
        {
            value = value.ToUpper().Trim();
            if (value.StartsWith("BEF"))
            {
                value = value.Substring(value.IndexOf(' ')).Trim();

                From = DateTime.MinValue;
                FromAccuracy = Accuracy.Unknown;
                var to = new Date(value);
                ToAccuracy = to.FromAccuracy;
                To = to.From;
            }
            else if (value.StartsWith("FROM"))
            {
                value = value.Substring(4);
                var parts = value.Split(new[] { "TO" }, StringSplitOptions.RemoveEmptyEntries);

                var startDate = new Date(parts[0]);

                From = startDate.From;
                FromAccuracy = startDate.FromAccuracy;
                To = DateTime.MaxValue;
                ToAccuracy = Accuracy.Unknown;

                if (parts.Length == 2)
                {
                    var end = new Date(parts[1]);
                    To = end.To;
                    ToAccuracy = startDate.ToAccuracy;
                }
            }
            else
            {
                var parts = value.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).ToList();

                int startDay = -1;

                var day = parts
                    .Where(x => x.All(char.IsDigit) && x.Length <= 2)
                    .FirstOrDefault();

                if (day != null)
                {
                    parts.Remove(day);
                    startDay = int.Parse(day);
                }

                var month = parts
                    .Where(x => !x.All(char.IsDigit))
                    .FirstOrDefault();

                int startMonth = 1;
                int endMonth = 12;
                if (month != null)
                {
                    parts.Remove(month);
                    var m = month.ToUpperInvariant().Substring(0, 3);
                    if (_monthLookUp.ContainsKey(m))
                    {

                        startMonth = endMonth = _monthLookUp[m];
                    }
                    else
                    {
                        throw new FormatException("Not convertable to a month");
                    }
                }

                var year = parts
                    .Where(x => x.All(char.IsDigit) && x.Length <= 4 && x.Length >= 2)
                    .FirstOrDefault();

                int startYear = DateTime.MinValue.Year;
                int endYear = DateTime.MaxValue.Year;
                if (year != null)
                {
                    parts.Remove(year);

                    endYear = startYear = int.Parse(year);
                }

                if (startDay > 0)
                {
                    From = new DateTime(startYear, startMonth, startDay);
                    To = new DateTime(endYear, endMonth, startDay);

                    FromAccuracy = ToAccuracy = Accuracy.Day;
                }
                else
                {
                    if (month != null)
                    {
                        FromAccuracy = ToAccuracy = Accuracy.Month;
                    }
                    else
                    {
                        FromAccuracy = ToAccuracy = Accuracy.Year;
                    }

                    From = new DateTime(startYear, startMonth, 1);
                    To = new DateTime(endYear, endMonth, 1)
                        .AddMonths(1)
                        .AddDays(-1);
                }
            }
        }

        public Date(FileDom.FileNode node) : this(node.Value)
        {
            _node = node;
        }

        public Accuracy FromAccuracy { get; private set; }
        public DateTime From { get; private set; }

        public Accuracy ToAccuracy { get; private set; }
        public DateTime To { get; private set; }

        public override string ToString()
        {

            if (From != To)
            {

                if (FromAccuracy == Accuracy.Unknown && ToAccuracy != Accuracy.Unknown)
                {
                    return string.Format("BEF {1:" + DateFormatFromAccuracy(ToAccuracy) + "}", From, To);
                }
                else
                {
                    return string.Format("FROM {0:" + DateFormatFromAccuracy(FromAccuracy) + "} TO {1:" + DateFormatFromAccuracy(ToAccuracy) + "}", From, To);
                }
            }
            else
            {
                return To.ToString(DateFormatFromAccuracy(FromAccuracy));
            }
        }
        private string DateFormatFromAccuracy(Accuracy acc)
        {
            switch (acc)
            {
                case Accuracy.Day:
                    return "dd MMM yyyy";
                case Accuracy.Month:
                    return "MMM yyyy";
                case Accuracy.Year:
                    return "yyyy";
                default:
                    return "...";
            }
        }
        public enum Accuracy
        {
            Unknown,
            Day,
            Month,
            Year
        }
    }
}
