using Microsoft.AspNetCore.Mvc;

namespace SampleApplication.Controllers
{
    public class HomeController : ControllerBase
    {
        [HttpGet]
        public string Index()
        {
            return "Hello world";
        }
    }
}