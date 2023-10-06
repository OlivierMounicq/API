using Microsoft.AspNetCore.Mvc;
using Sensor.DTO;
using Sensor.Services.Interfaces;

namespace Sensor.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [ApiVersion("1")]
    public class SensorController : Controller
    {
        private readonly ITemperatureService temperatureService;

        public SensorController(ITemperatureService temperatureService)
        {
            this.temperatureService = temperatureService;
        }

        
        [HttpGet("GetTemperatureFromSensor")]
        [ProducesResponseType(StatusCodes.Status200OK, Type= typeof(double))]
        public ActionResult GetTemperatureFromSensor(string sensorName)
        {
            var res = temperatureService.GetTemperatureFromSensor(sensorName);
            return Ok(res);
        }

        [HttpGet("GetSensorStatus")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
        public ActionResult GetTemperatureIntervalFromSensor(string sensorName)
        {
            var res = temperatureService.GetTemperatureIntervalFromSensor(sensorName);
            return Ok(res);
        }

        [HttpGet("GetLast15Temperature")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
        public ActionResult GetLast15Temperature(string sensorName)
        {
            var res = temperatureService.GetLastTemperatureList(sensorName, 15);
            return Ok(res);
        }

        [HttpPut(Name = "RedefineStatus")]
        public ActionResult RedefineTemperatureThreshold(string intervalName, double minTemperature, double maxTemperature)
        {
            temperatureService.RedefineTemperatureThreshold(intervalName, minTemperature, maxTemperature);
            return Ok();
        }

        [HttpPost("AddSensor")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult AddSensor([FromBody] string name)
        {
            temperatureService.AddSensor(name);
            return Ok();
        }

        [HttpPost("AddTemperature")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult AddTemperature(string name, DateTime dateTime, double temperature)
        {
            temperatureService.AddTemperature(name, dateTime, temperature);
            return Ok();
        }

        [HttpPost("AddTemperatureLsit")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult AddTemperatureList(string name, [FromBody] IEnumerable<TemperatureDTO> temperatures)
        {
            temperatureService.AddTemperatureList(name, temperatures);
            return Ok();
        }
    }
}
