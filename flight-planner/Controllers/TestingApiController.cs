using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using flight_planner.Models;
using System.Threading.Tasks;

using System.Web.Routing;
using flight_planner.Atributes;
using flight_planner.core.Models;
using flight_planner.Models;
using flight_planner.services;

namespace flight_planner.Controllers
{
    public class TestingApiController : ApiController
    {
        [HttpPost]
        [Route("testing-api/clear")]
        public async Task<bool> Clear()
        {
            var a =new FlightService();
            await a.ClearDb();


            return true;
        }

        public string Get()
        {
            return "Testing api";
        }
    }
}
