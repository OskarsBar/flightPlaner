using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using flight_planner.core.Models;
using flight_planner.data.Migrations;

namespace flight_planner.data
{
    public class FlightPlanerDbContext : DbContext
    {
        public FlightPlanerDbContext() : base("flight-planner")
        {
            Database.SetInitializer<FlightPlanerDbContext>(null);
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<FlightPlanerDbContext, Configuration>());
        }
        public DbSet<Flight> Flights { get; set; }
        public DbSet<Airport> Airports { get; set; }
    }
}
