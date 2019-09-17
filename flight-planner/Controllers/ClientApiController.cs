using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Mvc;
using flight_planner.Models;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Text;
using WebGrease.Css.Extensions;


namespace flight_planner.Controllers
{
    public class ClientApiController : ApiController
    {
        // GET: api/ClientApi
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }



        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("api/FlightSearchRequest/{id}")]
        public HttpResponseMessage Get(HttpRequestMessage request, int id)
        {
            var flight = FlightStorage.GetFlightById(id);
            if (flight == null)
            {
                request.CreateResponse(HttpStatusCode.NotFound);
            }
            return request.CreateResponse(HttpStatusCode.OK, flight);
        }
        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("api/flights/{id}")]
        public HttpResponseMessage FlightSearchById(HttpRequestMessage request, int id)
        {
            var flight = FlightStorage.GetFlightById(id);
            if (flight == null)
            {
                return request.CreateResponse(HttpStatusCode.NotFound, flight);
            }
            return request.CreateResponse(HttpStatusCode.OK, flight);
        }

        // GET: api/ClientApi/5
        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("api/airports")]
        public AirportRequest[] SearchAirportsByPhrase(string search)
        {
            var flights = FlightStorage.GetFlights();
            var result = new HashSet<AirportRequest>();

            flights.ForEach(f =>
            {
                result.Add(f.From);
                result.Add(f.To);
            });

            return result.Where(a => a.Airport.ToLower().Contains(search.ToLower().Trim()) ||
                                     a.City.ToLower().Contains(search.ToLower().Trim()) ||
                                     a.Country.ToLower().Contains(search.ToLower().Trim()))
                                    .ToArray();
            
        }

        // POST: api/ClientApi
        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("api/flights/search")]
        public HttpResponseMessage FlightSearch(HttpRequestMessage request, FlightSearchRequest search)
        {
            if (IsValid(search) && NotSameAirport(search))
            {
                var result = FlightStorage.GetFlights();
                var matchedItems = result.Where(f => f.From.Airport.ToLower().Contains(search.From.ToLower()) ||
                                                     f.To.Airport.ToLower().Contains(search.To.ToLower()) ||
                                                     f.DepartureTime.ToLower().Contains(search.To.ToLower())).ToList();
                var response = new FlightSearchResult
                {
                    TotalItems = result.Length,
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
