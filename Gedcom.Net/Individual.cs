using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gedcom.Net
{
    public class Individual : BaseEntity
    {
        private readonly FileDom.FileNode _source;
        internal Individual(GedcomDocument document, FileDom.FileNode source)
            : base(document, source)
        {
            _source = source;

        }

        public Event Birth
        {
            get
            {
                return GetValueFromNodes("BIRT", n =>
                {
                    var node = n.FirstOrDefault();
                    if (node == null)
                    {
                        return null;
                    }
                    return new Event(Document, node);
                });
            }
        }

        public Event Death
        {
            get
            {
                return GetValueFromNodes("DEAT", n =>
                {
                    var node = n.FirstOrDefault();
                    if (node == null)
                    {
                        return null;
                    }
                    return new Event(Document, node);
                });
            }
        }

        public IEnumerable<Individual> Parents
        {
            get
            {
                //find family by following upwards from child link

                return GetValueFromNodes("FAMC", n =>
                {
                    var famRefs = n.Select(x => x.Value);

                    var fams = Document.Families.Where(x => famRefs.Contains(x.Reference));

                    return fams.SelectMany(x => x.Parents).ToList();
                });
            }
        }


        public IEnumerable<Individual> Children
        {
            get
            {
                return GetValueFromNodes("FAMS", n =>
                {
                    var famRefs = n.Select(x => x.Value);

                    var fams = Document.Families.Where(x => famRefs.Contains(x.Reference));

                    return fams.SelectMany(x => x.Children).ToList();
                });
            }
        }

        public IEnumerable<Residancy> Residances
        {
            get
            {
                return GetValueFromNodes("RESI", n =>
                {
                    return n.Select(x => new Residancy(Document, x)).ToList();
                });
            }
        }


        public IEnumerable<Event> Events
        {
            get
            {
                if (Birth != null)
                {
                    yield return Birth;
                }
                foreach (var e in Residances)
                {
                    yield return e;
                }
                if (Death != null)
                {
                    yield return Death;
                }
            }
        }

        public Name Name
        {
            get
            {
                return GetValueFromNodes("NAME", n =>
                {
                    var node = n.FirstOrDefault();
                    if (node == null)
                    {
                        return null;
                    }
                    return new Name(node);
                });
            }
        }

        public Genders? Gender
        {
            get
            {
                return GetValue<Genders?>("SEX", v =>
                {
                    //inner function should only be called once
                    v = v.TrimStart();
                    switch (v.First())
                    {
                        case 'M':
                            return Genders.Male;
                        case 'F':
                            return Genders.Female;
                        default:
                            return null;
                    }
                });
            }
        }

        public override string ToString()
        {
            if (Name != null)
                return Name.ToString();
            else
                return "-unknown-";
        }

    }

    public enum Genders
    {
        Male,
        Female
    }
}
