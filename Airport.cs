using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CheapestFlights
{
    public class Airport
    {
        public string Code { get; set; }
        public string FullName { get; set; }
        public List<string> Destinations { get; set; }

        public Airport(string code, string fullName)
        {
            Code = code;
            FullName = fullName;
            Destinations = new List<string>();
        }

        public void AddDestination(string destination)
        {
            if (!Destinations.Contains(destination))
            {
                Destinations.Add(destination);
            }
        }

        public void AddDestinations(IEnumerable<string> destinations)
        {
            foreach (string destination in destinations)
            {
                AddDestination(destination);
            }
        }
    }

}
