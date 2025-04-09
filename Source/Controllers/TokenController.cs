using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Authorization;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.Azure.Services.AppAuthentication;

namespace SampleDeliveryService.Controllers
{
    [Route("api/token")]
    [ApiController]
    [Authorize("SessionToken")]
    public class TokenController : ControllerBase
    {
        [HttpGet]
        [EnableCors("LocalAzure")]
        public async Task<IActionResult> GetTokenAsync()
        {
            try
            {
                // Get Key Vault URI from environment
                var keyVaultUri = Environment.GetEnvironmentVariable("KEY_VAULT_URI");
                if (string.IsNullOrEmpty(keyVaultUri))
                    return BadRequest("KEY_VAULT_URI not configured.");

                // Create a secret client
                var secretClient = new SecretClient(new Uri(keyVaultUri), new DefaultAzureCredential());

                // Get the Azure Maps connection string from Key Vault
                var secretName = "AzureMapsPrimaryKey"; // 🔁 Make sure this secret exists
                KeyVaultSecret mapsSecret = await secretClient.GetSecretAsync(secretName);

                var tokenProvider = new AzureServiceTokenProvider(mapsSecret.Value);
                string accessToken = await tokenProvider.GetAccessTokenAsync("https://atlas.microsoft.com/", cancellationToken: HttpContext.RequestAborted);

                return Ok(accessToken);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Token generation failed: {ex.Message}");
            }
        }
    }
}
