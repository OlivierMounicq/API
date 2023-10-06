namespace Sensor.Model
{
    public class TemperatureThresholds
    {
        public int Id { get; set; }
        public int VersionId { get; set; }
        public string? IntervalName { get; set; }
        public double MinTemperature { get; set; }
        public double MaxTemperature { get; set; }
    }
}
