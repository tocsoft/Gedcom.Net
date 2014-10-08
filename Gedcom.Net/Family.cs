using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gedcom.Net
{
    public class Family : BaseEntity
    {
        internal Family(GedcomDocument document, FileDom.FileNode source)
            : base(document, source)
        {
            //_source = source;
        }

        public Individual Husband
        {
            get
            {
                return GetValue<Individual>("HUSB", s =>
                {
                    return Document.Individuals.FirstOrDefault(x => x.Reference == s);
                });
            }
        }

        public Individual Wife
        {
            get
            {
                return GetValue<Individual>("WIFE", s =>
                {
                    return Document.Individuals.FirstOrDefault(x => x.Reference == s);
                });
            }
        }

        internal IEnumerable<Individual> Parents
        {
            get
            {
                if (Husband != null)
                {
                    yield return Husband;
                }

                if (Wife != null)
                {
                    yield return Wife;
                }
            }
        }

        public IEnumerable<Individual> Children
        {
            get
            {
                return GetValueFromNodes("CHIL", n =>
                {
                    var refs = n.Select(x => x.Value);
                    return Document.Individuals.Where(x => refs.Contains(x.Reference)).ToList();
                });
            }
        }
    }
}
