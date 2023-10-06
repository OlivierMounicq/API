using Sensor.DTO;
using Sensor.Model;

namespace Sensor.Services.Interfaces
{
    public interface ITemperatureService
    {
        double GetTemperatureFromSensor(string sensorName);

        string GetTemperatureIntervalFromSensor(string sensorName);

        IEnumerable<TemperatureDTO> GetLastTemperatureList(string sensorName, int qty);

        void RedefineTemperatureThreshold(string intervalName, double minTemperature, double maxTemperature);

        IEnumerable<TemperatureThresholds> GetIntervalDefinition();

        void AddSensor(string name);

        void AddTemperature(string name, DateTime date, double temperature);

        void AddTemperatureList(string sensorName, IEnumerable<TemperatureDTO> temperatures);
    }
}
