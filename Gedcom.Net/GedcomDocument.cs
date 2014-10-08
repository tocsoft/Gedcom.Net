using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gedcom.Net.FileDom;

namespace Gedcom.Net
{
    public class GedcomDocument
    {
        private Lazy<FileDocument> _lazyFile;
        private FileDocument _file { get { return _lazyFile.Value; } }

        public GedcomDocument(FileDom.FileDocument file)
        {
            _lazyFile = new Lazy<FileDocument>(() => file);
        }

        public GedcomDocument(TextReader reader, bool lazyload)
        {
            if (lazyload)
            {
                _lazyFile = new Lazy<FileDocument>(() => FileDom.FileDocument.Parse(reader));
            }
            else
            {
                var doc = FileDom.FileDocument.Parse(reader);
                _lazyFile = new Lazy<FileDocument>(() => doc);
            }

        }

        public GedcomDocument(TextReader reader)
            : this(reader, true)
        {
        }

        public GedcomDocument(Stream reader)
            : this(new StreamReader(reader), true)
        {
        }

        public GedcomDocument(Stream reader, bool lazyLoad)
            : this(new StreamReader(reader), lazyLoad)
        {
        }

        private IEnumerable<Individual> _individuals;

        public IEnumerable<Individual> Individuals
        {
            get
            {
                return (_individuals = _individuals ?? _file.Where(x => x.Tag == "INDI")
                    .Select(x => new Individual(this, x))
                    .ToList());
            }
        }

        private IEnumerable<Family> _families;

        public IEnumerable<Family> Families
        {
            get
            {
                return (_families = _families ?? _file.Where(x => x.Tag == "FAM")
                    .Select(x => new Family(this, x))
                    .ToList());
            }
        }
    }


}
