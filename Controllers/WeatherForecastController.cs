using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.FeatureManagement;
using Microsoft.FeatureManagement.Mvc;

namespace feature_flags.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly IFeatureManager _featureManager;
        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(
            IFeatureManager featureManager,
            ILogger<WeatherForecastController> logger)
        {
            _featureManager = featureManager;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IEnumerable<WeatherForecast>> Get()
        {
            var rng = new Random();
            var isRainEnabled = await _featureManager.IsEnabledAsync("RainEnabled");
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                RainExpected = isRainEnabled ? $"{rng.Next(0,100)}%" : null,
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToArray();
        }
        
        [HttpGet("advanced")]
        [FeatureGate("AdvanceEnabled")]
        public async Task<IEnumerable<WeatherForecast>> GetAdvanced()
        {
            var useNewAlgorithm = await _featureManager.IsEnabledAsync("NewAlgorithmEnabled");
            
            return useNewAlgorithm
            ? await NewAlgorithm() 
            : await OldAlgorithm();
        }

        private async Task<IEnumerable<WeatherForecast>> OldAlgorithm()
        {
            var rng = new Random();
            var isRainEnabled = await _featureManager.IsEnabledAsync("RainEnabled");
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
                {
                    Date = DateTime.Now.AddDays(index),
                    TemperatureC = rng.Next(-20, 55),
                    RainExpected = isRainEnabled ? $"{rng.Next(0, 100)}% OLD" : null,
                    Summary = Summaries[rng.Next(Summaries.Length)]
                })
                .ToArray();
        }
        
        private async Task<IEnumerable<WeatherForecast>> NewAlgorithm()
        {
            var rng = new Random();
            var isRainEnabled = await _featureManager.IsEnabledAsync("RainEnabled");
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
                {
                    Date = DateTime.Now.AddDays(index),
                    TemperatureC = rng.Next(-20, 55),
                    RainExpected = isRainEnabled ? $"{rng.Next(0, 100)}%" : null,
                    Summary = Summaries[rng.Next(Summaries.Length)]
                })
                .ToArray();
        }
    }
}
