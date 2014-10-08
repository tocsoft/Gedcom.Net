using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gedcom.Scratch
{
    class Program
    {
        static void Main(string[] args)
        {
            var file = Gedcom.Net.FileDom.FileDocument.Load("Family Tree.ged");
            var doc = new Gedcom.Net.GedcomDocument(file);
            var person = doc.Individuals.First();

            var geo = new Geocode();

            //var locations = person.Residances.Select(x => geo.Lookup(x.Place));

            var place = person.Residances.FirstOrDefault();


            var places = doc.Individuals.SelectMany(i =>i.Residances.Select(e =>e.Place)).Distinct().ToList();

        }
    }
}
