namespace Sensor.Model
{
    public class Temperature
    {
        public int Id { get; set; }
        public int IdSensor { get; set; }
        public double Value { get; set; }
        public DateTime Date { get; set; }
    }
}
