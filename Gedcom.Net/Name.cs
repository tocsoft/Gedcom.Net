using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gedcom.Net.FileDom;

namespace Gedcom.Net
{
    public class Name
    {
        private FileNode node;

        public Name(FileNode node)
        {
            this.node = node;
            var raw = node.Value;
            var parts = raw.Split(new char[] { ' ', '/' }, StringSplitOptions.RemoveEmptyEntries);

            FullName = string.Join(" ", parts);


            var startOfSurname = raw.IndexOf('/') + 1;
            if (startOfSurname > -1)
            {
                //surname found
                var surnameWithSufix = raw.Substring(startOfSurname);

                var endOfSurname = surnameWithSufix.IndexOf('/');

                if (endOfSurname == -1)
                {
                    endOfSurname = surnameWithSufix.Length;
                }

                Surname = surnameWithSufix.Substring(0, endOfSurname);
            }
        }

        public string FullName { get; private set; }

        public string Surname { get; private set; }

        public override string ToString()
        {
            return FullName;
        }
    }
}
