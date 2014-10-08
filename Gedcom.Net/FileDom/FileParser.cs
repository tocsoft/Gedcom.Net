using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Gedcom.Net.FileDom
{
    internal class GedcomNodeParser
    {
        private TextReader _reader;
        private bool _parsed = false;
        private FileDocument _doc;

        public GedcomNodeParser(TextReader reader)
        {
            _reader = reader;
        }

        public FileDocument Document
        {
            get
            {
                if (!_parsed)
                {
                    //ensure its parsed
                    Parse();
                }

                return _doc;
            }
        }

        public FileDocument Parse()
        {
            if (_parsed)
            {
                throw new InvalidOperationException("Parse can only be called once");
            }

            var rootNode = new FileNode()
            {
                Level = -1,
                Parent = null,
                Tag = "ROOT"
            };

            var currentNode = rootNode;

            FileNode node = null;

            while ((node = GetNode()) != null)
            {
                while (currentNode.Level >= node.Level)
                {
                    if (currentNode.Parent == null)
                    {
                        throw new InvalidFileFormatException(_lineNumber, "invalid hierarchy structure");
                    }
                    currentNode = currentNode.Parent;
                }
                //found the parent node
                currentNode.AddChild(node);
                currentNode = node; //move to the new node
            }

            var rootNodes = rootNode.ToList();
            foreach (var n in rootNodes)
            {
                rootNode.RemoveChild(n);
            }

            _doc = new FileDocument(rootNodes);
            //do parsing
            _parsed = true;
            return Document;
        }

        Regex lineParser = new Regex(@"^(\d*)\s(?:(@.*@)\s+)?(_?\w{1,4})(?:\s+(.*))?$");

        int _lineNumber = 0;
        private FileNode GetNode()
        {
            _lineNumber++;
            //Trace.WriteLine("Parsing Line " + _lineNumber);

            var line = _reader.ReadLine();

            if (line == null)
            {
                return null;
            }

            var m = lineParser.Match(line);

            if (!m.Success)
            {
                throw new InvalidFileFormatException(_lineNumber, "Invalid format");
            }

            var node = new FileNode()
            {
                Level = int.Parse(m.Groups[1].Value),
                Tag = m.Groups[3].Value,
                Reference = m.Groups[2].Value,
                LineNumber = _lineNumber,
                Value = m.Groups[4].Value
            };

            return node;
        }
    }
}