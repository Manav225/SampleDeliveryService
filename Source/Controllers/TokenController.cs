using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Cors;
using System;

namespace SampleDeliveryService.Controllers
{
    [Route("api/token")]
    [ApiController]
    [EnableCors("LocalAzure")]
    public class TokenController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetToken()
        {
            // Use the Azure Maps subscription key directly from app settings
            var subscriptionKey = Environment.GetEnvironmentVariable("AZURE_MAPS_SUBSCRIPTION_KEY");

            if (string.IsNullOrEmpty(subscriptionKey))
                return BadRequest("AZURE_MAPS_SUBSCRIPTION_KEY is not configured in App Settings.");

            return Ok(subscriptionKey);
        }
    }
}
