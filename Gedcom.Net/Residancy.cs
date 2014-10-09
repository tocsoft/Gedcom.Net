using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gedcom.Net.FileDom;

namespace Gedcom.Net
{
    public class Residancy : Event
    {
        private readonly FileNode _node;

        public Residancy(Individual person, GedcomDocument document, FileNode node) : base(person, document, node)
        {
            _node = node;
        }

        private string Profession
        {
            get
            {
                return _node.Value;
            }
        }

        public override string ToString()
        {
            return string.Format("{0} - {1}", Date, Place);
        }
    }
}
