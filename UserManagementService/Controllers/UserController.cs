using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using M2Store.Common.ServiceDiscovery;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860
namespace UserManagement.Controllers
{
    [Authorize]
    [ApiController]
    [ApiVersion("1.0")] // Specify the version for this controller
    [Route("api/v{version:apiVersion}/[controller]")]
    public class UserController : ControllerBase
    {

        [AllowAnonymous]
        [HttpGet]
        [Route("GetServices")]
        public async Task<IActionResult> GetServices()
        {
            try
            {
                var httpClient = new HttpClient
                {
                    BaseAddress = new Uri("http://localhost:4000")
                };

                var services = await httpClient.GetFromJsonAsync<DownstreamServiceOptions>("/api/v1/services");
                return Ok(services!);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }
    }
}
