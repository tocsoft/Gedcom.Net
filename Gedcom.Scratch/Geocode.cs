using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using RestSharp;

namespace Gedcom.Scratch
{
    public class Geocode
    {
        RestClient client = new RestClient("http://nominatim.openstreetmap.org/");

        Dictionary<string, IEnumerable<Place>> _cache = new Dictionary<string, IEnumerable<Place>>();

        public IEnumerable<Place> Lookup(string address)
        {
            if (!_cache.ContainsKey(address))
            {


                var request = new RestRequest("search.php", Method.GET);
                request.AddParameter("q", address);
                request.AddParameter("format", "json");

                var results = client.Execute(request);

                IEnumerable<Place> places = Enumerable.Empty<Place>();
                if (!string.IsNullOrWhiteSpace(results.Content))
                {
                    places = Newtonsoft.Json.JsonConvert.DeserializeObject<IEnumerable<Place>>(results.Content);
                }
                _cache.Add(address, places);
            }

            return _cache[address];
        }
    }

    public class Place
    {
        public Place() { }
        public string place_id { get; set; }
        public string licence { get; set; }
        public string osm_type { get; set; }
        public string osm_id { get; set; }
        public string[] boundingbox { get; set; }
        public string lat { get; set; }
        public string lon { get; set; }
        public string display_name { get; set; }
        public string _class { get; set; }
        public string type { get; set; }
        public float importance { get; set; }
    }

}
