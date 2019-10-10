using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Routing;
using flight_planner.Atributes;
using flight_planner.core.Models;
using flight_planner.Models;
using flight_planner.services;

namespace flight_planner.Controllers
{
    [BasicAuthentication]
    public class AdminApiController : ApiController
    {
        private static readonly object obj = new object();
        private readonly FlightService _flightService;

        //private static readonly Object obj = new Object();

        public AdminApiController()
        {
            _flightService= new FlightService();
        }

        // GET: api/AdminApi
        public IEnumerable<string> Get()
        {
            return new string[] {"value1", "value2"};
        }

        // GET: api/AdminApi/5
        [HttpGet]
        [Route("admin-api/flights/{id}")]
        public async Task<HttpResponseMessage> Get(HttpRequestMessage request, int id)
        {
            var flight = await _flightService.GetFlightByIdentificator(id);
            if (flight == null)
            {
                return request.CreateResponse(HttpStatusCode.NotFound);
            }

            return request.CreateResponse(HttpStatusCode.OK, flight);
        }

        // POST: api/AdminApi
        public void Post([FromBody] string value)
        {
        }

        private static bool ValidateDates(string departure, string arrival)
        {
            if (!string.IsNullOrEmpty(departure) && !string.IsNullOrEmpty(arrival))
            {
                var departureDate = DateTime.Parse(departure);
                var arrivalDate = DateTime.Parse(arrival);
                return DateTime.Compare(arrivalDate, departureDate) > 0;
            }

            return false;
        }

        private bool IsGoodAirports(AirportRequest to, AirportRequest from)
        {
            return to.City.ToUpper() != from.City.ToUpper() &&
                   to.Airport.ToUpper() != from.Airport.ToUpper();
            //to.Country.ToUpper() != from.Country.ToUpper();
        }

        private bool IsValidAirport(AirportRequest airport)
        {
            return !string.IsNullOrEmpty(airport.Airport) &&
                   !string.IsNullOrEmpty(airport.City) &&
                   !string.IsNullOrEmpty(airport.Country);
        }

        private bool IsValid(FlightRequest flight)
        {
            
                return !string.IsNullOrEmpty(flight.ArrivalTime) &&
                       !string.IsNullOrEmpty(flight.Carrier) &&
                       !string.IsNullOrEmpty(flight.DepartureTime) &&
                       flight.To != null &&
                       flight.From != null &&
                       IsValidAirport(flight.To) &&
                       IsValidAirport(flight.From) &&
                       ValidateDates(flight.DepartureTime, flight.ArrivalTime) &&
                       IsGoodAirports(flight.To, flight.From)
                    ;
            
        }

        public Flight FillInFlight(FlightRequest flight)
        {
            var domainFlight = new Flight
            {
                From = new Airport
                {
                    AirportCode = flight.From.Airport,
                    City = flight.From.City,
                    Country = flight.From.Country
                },
                To = new Airport
                {
                    AirportCode = flight.To.Airport,
                    City = flight.To.City,
                    Country = flight.To.Country
                },
                ArrivalTime = flight.ArrivalTime,
                Id = flight.Id,
                DepartureTime = flight.DepartureTime,
                Carrier = flight.Carrier
            };
            return domainFlight;
        }
       
        // PUT: api/AdminApi/5
        [HttpPut]
        [Route("admin-api/flights")]
        
        public async Task<HttpResponseMessage> AddFlight(HttpRequestMessage request, FlightRequest flight)
        {
            if (!IsValid(flight))
            {
                return request.CreateResponse(HttpStatusCode.BadRequest);
            }
            var domainFlight = FillInFlight(flight);
            var rand = new Random();
            lock (obj)
            {
                {
                    if (_flightService.Exists(domainFlight))
                    {
                        return request.CreateResponse(HttpStatusCode.Conflict, flight);
                    }
                    domainFlight =  _flightService.AddFlight(domainFlight);
                    flight.Id = domainFlight.Id;

                    return request.CreateResponse(HttpStatusCode.Created, flight);

                }
            }
        }
        [HttpGet]
        [Route("admin-api/get/flights")]
        public async Task<ICollection<Flight>> GetFlights(HttpRequestMessage request)
        {

            return await _flightService.GetFlights();
        }
        // DELETE: api/AdminApi/5
        [HttpDelete]
        [Route("admin-api/flights/{id}")]
        public async Task<HttpResponseMessage> Delete(HttpRequestMessage request, int id)
        {
            
               await _flightService.DeleteFlightByIdentificator(id);

                return request.CreateResponse(HttpStatusCode.OK);

            
        }

    }
}
