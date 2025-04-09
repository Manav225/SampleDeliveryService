using Microsoft.AspNetCore.Mvc;

namespace SampleDeliveryService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HealthController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get() => Ok("Service is up and running 🚀");
    }
}
