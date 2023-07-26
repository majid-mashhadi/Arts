using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Public_API.Models.Arts;
using Public_API.Services;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Public_API.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiVersion("1.0")] // Specify the version for this controller
    [Route("api/v{version:apiVersion}/[controller]")]
    public class ArtController : ControllerBase
    {
        private readonly ArtService artService;

        public ArtController(ArtService artService)
        {
            this.artService = artService;
        }

        [HttpGet("List")]
        public async Task<IActionResult> Get()
        {
            return await artService.GetArtList();
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string ArtId)
        {
            return await artService.Load(ArtId);
        }

        // POST api/values
        [HttpPost("Save")]
        public async Task<IActionResult> Post([FromBody]Art art)
        {
            return await artService.SaveArt(art);

        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(string  ArtId)
        {

        }
    }
}

