using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gedcom.Net.FileDom;

namespace Gedcom.Net
{
    public abstract class BaseEntity
    {
        private readonly FileNode _source;
        private readonly GedcomDocument _document;

        protected GedcomDocument Document { get { return _document; } }

        public string Reference { get { return _source.Reference; } }

        internal BaseEntity(GedcomDocument document, FileDom.FileNode source)
        {
            _document = document;
            _source = source;
        }


        private TValue GetValueInner<TValue>(string tag, Func<string, TValue> converter)
        {
            tag = tag.ToUpper();
            var vals = _source.Where(x => x.Tag == tag).Select(x => x.Value);
            var str = string.Join("\n", vals).Trim();

            return converter(str);
        }

        private TValue GetValueInnerFromNodes<TValue>(string tag, Func<IEnumerable<FileNode>, TValue> converter)
        {
            tag = tag.ToUpper();

            return converter(_source.Where(x => x.Tag == tag));
        }


        Dictionary<string, object> cache = new Dictionary<string, object>();
        object cacheLock = new object();


        protected string GetValue(string tag)
        {
            return GetValue(tag, x => x);
        }

        protected TValue GetValue<TValue>(string tag, Func<string, TValue> converter)
        {
            tag = tag.ToUpper();
            if (cache.ContainsKey(tag))
            {
                return (TValue)cache[tag];
            }
            else
            {
                lock (cacheLock)
                {
                    if (cache.ContainsKey(tag))
                    {
                        return (TValue)cache[tag];
                    }
                    else
                    {
                        var v = GetValueInner(tag, converter);
                        cache.Add(tag, v);
                        return v;
                    }
                }
            }
        }

        protected string GetValueFromNodes(string tag)
        {
            return GetValue(tag, x => x);
        }

        protected TValue GetValueFromNodes<TValue>(string tag, Func<IEnumerable<FileNode>, TValue> converter)
        {
            tag = tag.ToUpper();
            if (cache.ContainsKey(tag))
            {
                return (TValue)cache[tag];
            }
            else
            {
                lock (cacheLock)
                {
                    if (cache.ContainsKey(tag))
                    {
                        return (TValue)cache[tag];
                    }
                    else
                    {
                        var v = GetValueInnerFromNodes(tag, converter);
                        cache.Add(tag, v);
                        return v;
                    }
                }
            }
        }
    }
}