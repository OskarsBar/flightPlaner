using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Web;

namespace flight_planner.core.Models
{
    public class Airport
    {
        public int Id { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public string AirportCode { get; set; }

        public override bool Equals(object obj)
        {
            var airport = obj as Airport;
            if (airport == null)
            {
                return false;
            }

            return airport.City == City && airport.AirportCode == AirportCode && airport.Country == Country;

        }
    }
}