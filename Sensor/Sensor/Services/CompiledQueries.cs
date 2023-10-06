using Microsoft.EntityFrameworkCore;
using Sensor.Model;

namespace Sensor.Services
{
    public static class CompiledQueries
    {
        public static Func<SensorTemperatureDbContext, string, IEnumerable<Sensors>> GetSensorByName = EF.CompileQuery((SensorTemperatureDbContext ctx, string sensorName) => ctx.Sensors.AsNoTracking().Where(t => t.SensorName == sensorName).AsQueryable());

        public static Func<SensorTemperatureDbContext, int> GetTemperatureThresholdLastVersionId = EF.CompileQuery((SensorTemperatureDbContext ctx) => ctx.TemperatureThresholds.AsNoTracking().Max(t => t.VersionId));

        public static Func<SensorTemperatureDbContext, int, IEnumerable<TemperatureThresholds>> GetTemperatureThresholdDefintion = EF.CompileQuery((SensorTemperatureDbContext ctx, int idVersion) => ctx.TemperatureThresholds.AsNoTracking().Where(t => t.VersionId == idVersion).AsQueryable());

        public static Func<SensorTemperatureDbContext, int, int, IEnumerable<Temperature>> GetLastTemperatureSeries = EF.CompileQuery((SensorTemperatureDbContext ctx, int idSensor, int qty) => ctx.Temperature.AsNoTracking().Where(t => t.IdSensor == idSensor).OrderByDescending(t => t.Date).Take(qty).AsQueryable());        
    }
}
