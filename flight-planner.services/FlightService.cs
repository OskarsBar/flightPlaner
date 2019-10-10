using flight_planner.core.Models;
using  flight_planner.data;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace flight_planner.services
{
    public class FlightService
    {
        public async Task <ICollection<Flight>> GetFlights()
        {
            using (var context = new FlightPlanerDbContext())
            {
                return await context.Flights.Include(f => f.To).Include(f => f.From).ToListAsync();
            }
        }
        public async Task<Flight> GetFlightByIdentificator(int id)
        {
            using (var context = new FlightPlanerDbContext())
            {
                var flight = await context.Flights.Include(f => f.To).Include(f => f.From).Where(f => f.Id == id).FirstOrDefaultAsync();
                // var x = await context.Flights.Where(f => f.Id == id).FirstOrDefault<Flight>();
                return flight;
            }
        }
        public async Task DeleteFlightByIdentificator(int id)
        {
            using (var context = new FlightPlanerDbContext())
            {
                Flight flight = await context.Flights.Include(f => f.To).Include(f => f.From).Where(f => f.Id == id)
                    .FirstOrDefaultAsync();
                if (flight != null)
                {
                context.Flights.Attach(flight);
                context.Flights.Remove(flight);
                await context.SaveChangesAsync();

                }
            }
        }
        public  Flight AddFlight(Flight flight)
        {
            using (var context = new FlightPlanerDbContext())
            {
                context.Flights.Add(flight);
                context.SaveChanges();
                return flight;
            }
        }
        
        int a = 0;
        int b = 0;
        int c = 0;
        int d = 0;
        int f = 0;
        int[] arr = new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,0,0,0,0 };
        public async Task ClearDb()
        {
            for(int i = 0; i < arr.Length; i++)
            {
                arr[i] = 0;
            }
             a = 0;
             b = 0;
             c = 0;
             d = 0;
             f = 0;
            using (var context = new FlightPlanerDbContext())
            {
                context.Airports.RemoveRange(context.Airports);
                context.Flights.RemoveRange(context.Flights);
                await context.SaveChangesAsync();
            }
        }
        

        public bool Exists(Flight flight)
        {
            /*var rand = new Random();
            if (a < 100) a += 25;
            System.Threading.Thread.Sleep(rand.Next(a,a+10));
            if (b < 100) b += 25;
            System.Threading.Thread.Sleep(rand.Next(b, b + 10));
            if (c < 100) c += 25;
            System.Threading.Thread.Sleep(rand.Next(c, c + 10));
            if (d < 100) d += 25;
            System.Threading.Thread.Sleep(rand.Next(d, d + 10));
            if (f < 100) f += 25;
            System.Threading.Thread.Sleep(rand.Next(f, f + 10));*/
            
            /*for (int i = 0; i < arr.Length; i++)
            {
                System.Threading.Thread.Sleep(rand.Next( arr[i],arr[i]+10));
                if (arr[i] == 0) arr[i] += 25;
            }*/
            using (var context = new FlightPlanerDbContext())
            {
                var result = context.Flights.Any(f => f.ArrivalTime == flight.ArrivalTime&&
                                                       f.Carrier == flight.Carrier&&
                                                       f.DepartureTime == flight.DepartureTime&&
                                                       f.From.AirportCode == flight.From.AirportCode&&
                                                       f.From.City == flight.From.City&&
                                                       f.From.Country == flight.From.Country&&
                                                       f.To.AirportCode == flight.To.AirportCode &&
                                                       f.To.City == flight.To.City &&
                                                       f.To.Country == flight.To.Country
                                                       
                                                       );
                return result;
            }
        }
    }
}
