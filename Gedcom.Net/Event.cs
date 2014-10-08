using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gedcom.Net.FileDom;

namespace Gedcom.Net
{
    public class Event : BaseEntity
    {
        private readonly FileNode _node;

        public Event(GedcomDocument document, FileNode node) : base(document, node)
        {
            _node = node;
        }

        private Types? _type;
        public Types Type
        {
            get
            {
                if (!_type.HasValue)
                {

                    switch (_node.Tag)
                    {
                        case "BIRT":
                            _type = Types.Birth;
                            break;
                        case "DEAT":
                            _type = Types.Death;
                            break;
                        case "RESI":
                            _type = Types.Residancy;
                            break;
                        default:
                            _type = Types.Unknown;
                            break;
                    }
                }

                return _type.Value;
            }
        }

        public Date Date
        {
            get
            {
                return GetValueFromNodes("DATE", s =>
                {
                    if (s.Any())
                    {
                        return new Date(s.First());
                    }
                    return null;
                });
            }
        }

        public string Place
        {
            get
            {
                return GetValue("PLAC");
            }
        }

        public override string ToString()
        {
            return string.Format("{0} - {1}", Date, Place);
        }

        public enum Types
        {
            Unknown,
            Birth,
            Death,
            Residancy
        }
    }
}
