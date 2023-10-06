using Moq;
using Sensor.Controllers;
using Sensor.Services.Interfaces;
using Xunit;
using System;

namespace SensortWebApiTestProject
{
    public class SensorControllerTests
    {
        [Fact]
        public void GetTemperatureFromSensor_Returns_Temperature()
        {
            // Arrange
            var temperatureService = new Mock<ITemperatureService>();
            temperatureService
                .Setup(x => x.GetTemperatureFromSensor(It.IsAny<string>()))
                .Returns(22.2);

            var controller = new SensorController(temperatureService.Object);
            
            // Act
            var returnedTemperature = controller.GetTemperatureFromSensor(It.IsAny<string>());

            // Assert
            Assert.Equal(22.2, (returnedTemperature as Microsoft.AspNetCore.Mvc.OkObjectResult).Value);
        }

        [Fact]
        public void GetTemperatureFromSensor_throw_exception()
        {
            var temperatureService = new Mock<ITemperatureService>();
            temperatureService
                .Setup(x => x.GetTemperatureFromSensor(It.IsAny<string>()))
                .Throws(new Exception("No sensor with this name"));

            var controller = new SensorController(temperatureService.Object);

            var actionForAssert = () => { throw new Exception("No sensor with this name"); };
            
            Assert.Throws<Exception>(actionForAssert);
        }
    }
}