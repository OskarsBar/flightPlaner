using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Mvc;
using flight_planner.core.Models;
using flight_planner.Models;
using flight_planner.services;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Text;
using WebGrease.Css.Extensions;


namespace flight_planner.Controllers
{
    public class ClientApiController : ApiController
    {
        private readonly FlightService _flightService;
        public FlightRequest FillInOldFlight(Flight flight)
        {
            var domainFlight = new FlightRequest
            {
                From = new AirportRequest
                {
                    Airport = flight.From.AirportCode,
                    City = flight.From.City,
                    Country = flight.From.Country
                },
                To = new AirportRequest
                {
                    Airport = flight.To.AirportCode,
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

        public ClientApiController()
        {
            _flightService = new FlightService();
        }
        // GET: api/ClientApi
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }



        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("api/FlightSearchRequest/{id}")]
        public async Task<HttpResponseMessage> Get(HttpRequestMessage request, int id)
        {
            var flight = await _flightService.GetFlightByIdentificator(id);

            if (flight == null)
            {
               return request.CreateResponse(HttpStatusCode.NotFound);
            }
            var oldFlight = FillInOldFlight(flight);
            return request.CreateResponse(HttpStatusCode.OK, oldFlight);
        }
        
        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("api/flights/{id}")]
        public async Task<HttpResponseMessage> FlightSearchById(HttpRequestMessage request, int id)
        {
            var flight = await _flightService.GetFlightByIdentificator(id);
           
            if (flight == null)
            {
                return request.CreateResponse(HttpStatusCode.NotFound);
            }
            var oldFlight = FillInOldFlight(flight);
            return request.CreateResponse(HttpStatusCode.OK, oldFlight);
        }

        // GET: api/ClientApi/5
        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("api/airports")]
        public async Task<AirportRequest[]> SearchAirportsByPhrase(string search)
        {
            var flights = await _flightService.GetFlights();
            
            var result = new HashSet<AirportRequest>();

            flights.ForEach(f =>
            {
                var flight = FillInOldFlight(f);
                result.Add(flight.From);
                result.Add(flight.To);
            });

            return result.Where(a => a.Airport.ToLower().Contains(search.ToLower().Trim()) ||
                                     a.City.ToLower().Contains(search.ToLower().Trim()) ||
                                     a.Country.ToLower().Contains(search.ToLower().Trim()))
                                    .ToArray();
            
        }

        // POST: api/ClientApi
        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("api/flights/search")]
        public async Task<HttpResponseMessage> FlightSearch(HttpRequestMessage request, FlightSearchRequest search)
        {
            if (IsValid(search) && NotSameAirport(search))
            {
                var result = await _flightService.GetFlights();
                
                var matchedItems = result.Where(f => f.From.AirportCode.ToLower().Contains(search.From.ToLower()) ||
                                                     f.To.AirportCode.ToLower().Contains(search.To.ToLower()) ||
                                                     f.DepartureTime.ToLower().Contains(search.To.ToLower())).ToList();
                
                var response = new FlightSearchResult
                {
                    TotalItems = result.Count,
                    Items = matchedItems,
                    Page = matchedItems.Any() ? 1 : 0
                };
                return request.CreateResponse(HttpStatusCode.OK, response);
            }
            return request.CreateResponse(HttpStatusCode.BadRequest);

            
            
        }
        private bool IsValid(FlightSearchRequest search)
        {
            return search != null && !string.IsNullOrEmpty(search.From) && !string.IsNullOrEmpty(search.To) &&
                   !string.IsNullOrEmpty(search.DepartureDate);
        }
        private bool NotSameAirport(FlightSearchRequest search)
        {
            return !string.Equals(search.From, search.To, StringComparison.InvariantCultureIgnoreCase);
        }

        // PUT: api/ClientApi/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/ClientApi/5
        public void Delete(int id)
        {
        }
    }
}
