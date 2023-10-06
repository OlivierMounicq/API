using Sensor.DTO;
using Sensor.Model;
using Sensor.Services.Interfaces;

namespace Sensor.Services
{
    public class TemperatureService : ITemperatureService
    {
        private readonly SensorTemperatureDbContext sensorTemperatureDbContext;

        public TemperatureService(SensorTemperatureDbContext dbContext)
        {
            this.sensorTemperatureDbContext = dbContext;
        }

        public double GetTemperatureFromSensor(string sensorName)
        {
            var idSensor = GetSensorId(sensorName);

            var temp = CompiledQueries.GetLastTemperatureSeries(sensorTemperatureDbContext, idSensor, 1);

            if (!temp.Any())
                throw new Exception($"There are no data for the sensor {sensorName}");

            var sensorTemperature = temp.FirstOrDefault().Value;

            return sensorTemperature; 
        }

        public string GetTemperatureIntervalFromSensor(string sensorName)
        {
            var tempIntervals = GetIntervalDefinition();

            double sensorTemperature = GetTemperatureFromSensor(sensorName);

            var temp = tempIntervals
                        .Select(t => (Name: t.IntervalName, Distance: sensorTemperature - t.MinTemperature ))
                        .Where(t => t.Distance >= 0)
                        .OrderBy(t => t.Distance)
                        .FirstOrDefault();

            var tempIntervalName = temp.Name;

            return tempIntervalName;
        }

        public IEnumerable<TemperatureDTO> GetLastTemperatureList(string sensorName, int qty)
        {
            var id = CompiledQueries.GetSensorByName(sensorTemperatureDbContext, sensorName).FirstOrDefault()?.Id;
            var res = CompiledQueries.GetLastTemperatureSeries(sensorTemperatureDbContext, id.Value, qty);

            var list = res.Select(t => new TemperatureDTO() { Date = t.Date, Temperature = t.Value });
            return list;
        }

        public void RedefineTemperatureThreshold(string intervalName, double minTemperature, double maxTemperature)
        {
            if (minTemperature > maxTemperature)
                throw new Exception($"The min temp {minTemperature} is greater than {maxTemperature}");

            var def = GetIntervalDefinition();

            if (!def.Any(t => t.IntervalName == intervalName))
                throw new Exception($"The interval with this name {intervalName} does not exists");

            var defWithIdx = def.Select((t, idx) => new { Idx = idx, Threshold = t }).ToList();

            var statusToUpdate = defWithIdx.Where(t => t.Threshold.IntervalName == intervalName).FirstOrDefault();

            var nextStatus = defWithIdx.Where(t => t.Idx == statusToUpdate?.Idx + 1).FirstOrDefault();
            var previousStatus = defWithIdx.Where(t => t.Idx == statusToUpdate?.Idx - 1).FirstOrDefault();

            var qty1 = statusToUpdate.Idx - 1 >= 0 ? statusToUpdate.Idx - 1 : 0;
            var previousUnchangedStatus = defWithIdx.Take(qty1);

            var qty2 = statusToUpdate.Idx + 2;
            var nextUnchangedStatus = defWithIdx.Skip(qty2);

            var qtyToTake = qty1 == 0 && qty2 == 2 ? 2 : 3;

            var statusToChange = defWithIdx.Skip(qty1).Take(qtyToTake).ToList();

            var position = statusToChange.Select((t, idx) => new { Ix = idx, Data = t.Threshold }).Where(t => t.Data.IntervalName == intervalName).FirstOrDefault().Ix;

            if(statusToChange.Count() ==3)
            {
                statusToChange[0].Threshold.MaxTemperature = minTemperature;
                statusToChange[1].Threshold.MinTemperature = minTemperature;
                statusToChange[1].Threshold.MaxTemperature = maxTemperature;
                statusToChange[2].Threshold.MinTemperature = maxTemperature;
            }
            else if(statusToChange.Count() ==2)
            {
                if(position == 0)
                {
                    statusToChange[0].Threshold.MinTemperature = minTemperature;
                    statusToChange[0].Threshold.MaxTemperature = maxTemperature;
                    statusToChange[1].Threshold.MinTemperature = maxTemperature;
                }
                else if(position == 1)
                {
                    statusToChange[0].Threshold.MaxTemperature = minTemperature;
                    statusToChange[1].Threshold.MinTemperature = minTemperature;
                    statusToChange[1].Threshold.MaxTemperature = maxTemperature;
                }
            }

            var idInterval = CompiledQueries.GetTemperatureThresholdLastVersionId(sensorTemperatureDbContext) + 1;
            var statusToInsert = new List<TemperatureThresholds>();

            statusToInsert = statusToInsert.Concat(previousUnchangedStatus.Select(t => new TemperatureThresholds() { VersionId = idInterval, IntervalName = t.Threshold.IntervalName, MinTemperature = t.Threshold.MinTemperature, MaxTemperature = t.Threshold.MaxTemperature })).ToList();
            statusToInsert = statusToInsert.Concat(statusToChange.Select(t => new TemperatureThresholds() { VersionId = idInterval, IntervalName = t.Threshold.IntervalName, MinTemperature = t.Threshold.MinTemperature, MaxTemperature = t.Threshold.MaxTemperature })).ToList();
            statusToInsert = statusToInsert.Concat(nextUnchangedStatus.Select(t => new TemperatureThresholds() { VersionId = idInterval, IntervalName= t.Threshold.IntervalName, MinTemperature = t.Threshold.MinTemperature,MaxTemperature = t.Threshold.MaxTemperature })).ToList();

            this.sensorTemperatureDbContext.AddRange(statusToInsert);
            this.sensorTemperatureDbContext.SaveChanges();
        }

        public IEnumerable<TemperatureThresholds> GetIntervalDefinition()
        {
            var idInterval = CompiledQueries.GetTemperatureThresholdLastVersionId(sensorTemperatureDbContext);
            var intervalDefinition = CompiledQueries.GetTemperatureThresholdDefintion(sensorTemperatureDbContext, idInterval).ToList();
            return intervalDefinition;            
        }

        public void AddSensor(string name)
        {
            var sensor = new Sensors() { SensorName = name };

            this.sensorTemperatureDbContext.Add(sensor);
            this.sensorTemperatureDbContext.SaveChanges();
        }

        public void AddTemperature(string name, DateTime date, double temperature)
        {
            var sensorId = GetSensorId(name);
            var temperatureObj = new Temperature() { IdSensor = sensorId, Value = temperature, Date = date };
            this.sensorTemperatureDbContext.Add(temperatureObj);
            this.sensorTemperatureDbContext.SaveChanges();
        }

        public void AddTemperatureList(string sensorName, IEnumerable<TemperatureDTO> temperatures)
        {
            var sensorId = GetSensorId(sensorName);

            if (!temperatures.Any())
                throw new Exception("There are no temperature list");

            var temperatureToSave = temperatures.Select(t => new Temperature() { IdSensor = sensorId, Value = t.Temperature, Date = t.Date }).ToArray();

            this.sensorTemperatureDbContext.AddRange(temperatureToSave);
            this.sensorTemperatureDbContext.SaveChanges();
        }

        private int GetSensorId(string sensorName)
        {
            var data = CompiledQueries.GetSensorByName(sensorTemperatureDbContext, sensorName);

            if (!data.Any())
                throw new Exception($"There is no sensor with this name {sensorName}");

            if (data.Count() > 1)
                throw new Exception("Several sensors have the same name");

            var idSensor = data.First().Id;
            return idSensor;
        }
    }
}
