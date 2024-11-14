using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TodoApi.Services;
using System.Net.Http.Json;
using Moq.Protected;
using Xunit;


namespace TodoApi.Tests.UnitTests
{
    public class WeatherServiceTests
    {
        private readonly WeatherService _weatherService;
        private readonly Mock<HttpMessageHandler> _httpMessageHandlerMock;

        public WeatherServiceTests()
        {
            // Mock HttpMessageHandler
            _httpMessageHandlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            var httpClient = new HttpClient(_httpMessageHandlerMock.Object);
            _weatherService = new WeatherService(httpClient);
        }

        [Fact]
        public async Task GetWeatherAsync_ReturnsWeatherData()
        {
            // Arrange
            var latitude = 37.7749;
            var longitude = -122.4194;
            var fakeWeatherResponse = new WeatherResponse
            {

                Current = new Current
                {
                    TempC = 15.0,
                    Condition = new ConditionData
                    {
                        Text = "Sunny"
                    }
                }
            };

            _httpMessageHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    Content = JsonContent.Create(fakeWeatherResponse)
                });

            // Act
            var weatherData = await _weatherService.GetWeatherAsync(latitude, longitude);

            // Assert
            Assert.NotNull(weatherData);
            Assert.Equal(15.0, weatherData.TemperatureC);
            Assert.Equal("Sunny", weatherData.Condition);
        }
    }
}
