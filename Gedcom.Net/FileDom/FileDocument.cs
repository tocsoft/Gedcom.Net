using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gedcom.Net.FileDom
{
    public class FileDocument : IEnumerable<FileNode>
    {
        internal FileDocument(IEnumerable<FileNode> nodes)
        {
            _nodes.AddRange(nodes);
        }


        private List<FileNode> _nodes = new List<FileNode>();

        public IEnumerator<FileNode> GetEnumerator()
        {
            return _nodes.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _nodes.GetEnumerator();
        }

        public static FileDocument Parse(TextReader reader)
        {
            var p = new GedcomNodeParser(reader);

            return p.Parse();
        }

        public static FileDocument Parse(string contents)
        {
            return Parse(new StringReader(contents));
        }

        public static FileDocument Load(string filename)
        {
            using (var reader = File.OpenText(filename))
            {
                return Parse(reader);
            }
        }

    }
}
