using Microsoft.EntityFrameworkCore;

namespace Sensor.Model
{
    public class SensorTemperatureDbContext : DbContext
    {
        public SensorTemperatureDbContext()
        { }

        public SensorTemperatureDbContext(DbContextOptions<SensorTemperatureDbContext> options)
            :base(options)
        { }

        public DbSet<Sensors>? Sensors { get; set; }
        public DbSet<Temperature>? Temperature { get; set; }
        public DbSet<TemperatureThresholds>? TemperatureThresholds { get; set; }

        //protected override void OnConfiguring(DbContextOptionsBuilder dbContextOptionsBuilder)
        //    => dbContextOptionsBuilder.UseSqlite("Data Source=sensor.db");

    }
}
