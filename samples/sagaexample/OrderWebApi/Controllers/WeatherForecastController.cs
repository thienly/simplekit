using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OrderWebApi.Domains;

namespace OrderWebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private OrderServices _orderServices;
        private IDatabaseAdapter<Order> _databaseAdapter;
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, OrderServices orderServices, IDatabaseAdapter<Order> databaseAdapter)
        {
            _logger = logger;
            _orderServices = orderServices;
            _databaseAdapter = databaseAdapter;
        }

        [HttpGet]
        public IActionResult Get()
        {
            var order = _orderServices.CreateOrder("First Order", Guid.Parse("b0d7a1d7-aed3-4ed3-ae97-81b03f49a5ba"),
                Guid.Parse("6058c4f4-d001-44c4-bdca-c571317465b2"));
            return Ok("I'm Okay");
        }

        [HttpGet]
        [Route("Uncompleted")]
        public ActionResult<List<Order>> GetUncompleted()
        {
            var orders = _databaseAdapter.GetUncompletedOrders().ToList();
            return Ok(orders);
        }
    }
}