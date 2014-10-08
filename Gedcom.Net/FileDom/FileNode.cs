using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gedcom.Net.FileDom
{

    public class FileNode : IEnumerable<FileNode>
    {
        private List<FileNode> _children;
        internal FileNode()
        {
            _children = new List<FileNode>();
        }

        public string Tag { get; internal set; }

        int _level;
        public int Level
        {
            get
            {
                if (Parent == null)
                    return _level;
                else
                    return Parent.Level + 1;
            }
            internal set
            {
                _level = value;
            }
        }

        public int LineNumber { get; internal set; }

        public FileNode Parent { get; internal set; }

        public string Value { get; internal set; }

        public string Reference { get; internal set; }

        internal void AddChild(FileNode node)
        {
            if (node.Parent != null)
            {
                node.Parent.RemoveChild(node);
            }
            node.Parent = this;
            _children.Add(node);
        }

        internal void RemoveChild(FileNode node)
        {
            if (node.Parent == this)
            {
                node.Parent = null;
                _children.Remove(node);
            }
        }

        public IEnumerator<FileNode> GetEnumerator()
        {
            return _children.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _children.GetEnumerator();
        }

        public override string ToString()
        {
            return string.Join(" ", new string[] {
                Level.ToString(),
                Reference,
                Tag,
                Value
            }.Where(x => !string.IsNullOrWhiteSpace(x)));
        }
    }
}
