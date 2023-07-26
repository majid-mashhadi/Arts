using System;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Public_API.Models.Arts;

namespace Public_API.Services
{
	public class ArtService
	{
        private readonly ApplicationDbContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ArtService(ApplicationDbContext dbContext,
            IHttpContextAccessor httpContextAccessor)		
		{
			this._dbContext = dbContext;
            this._httpContextAccessor = httpContextAccessor;

        }

        public async Task<IActionResult> GetArtList()
		{
            string userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value!;

            var list = await _dbContext.Arts
			.Where(a => a.UserId == userId )
            .Select(a => new Art
            {
                ArtId = a.ArtId,
                ArtName = a.ArtName,
                ArtSource = a.ArtSource,
                CanvasHeight = a.CanvasHeight,
                CanvasWidth = a.CanvasWidth,
            }).ToListAsync();

            return new OkObjectResult(list);

		}

        public async Task<IActionResult> Load(string ArtId)
        {
            try
            {
                // If the Id is not 0, it means it's an existing record (Update operation)
                var existingArt = await _dbContext.Arts.FindAsync(ArtId);
                if (existingArt == null)
                {
                    return new NotFoundObjectResult("Art not found."); // Art with the specified Id doesn't exist
                }
                return new OkObjectResult(existingArt);
            }
            catch(Exception )
            {
                return new BadRequestObjectResult("Failed to load the Art.");
            }
        }

        public async Task<IActionResult> SaveArt(Art art)
        {
            try
            {
                string userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value!;
                art.UserId = userId;

                if (art.ArtId == "")
                {
                    art.ArtId = Guid.NewGuid().ToString();
                    await _dbContext.Arts.AddAsync(art);
                }
                else
                {
                    // If the Id is not 0, it means it's an existing record (Update operation)
                    var existingArt = await _dbContext.Arts.FindAsync(art.ArtId);
                    if (existingArt == null)
                    {
                        return new NotFoundObjectResult("Art not found."); // Art with the specified Id doesn't exist
                    }

                    // Update the existingArt entity with the new values from art
                    _dbContext.Entry(existingArt).CurrentValues.SetValues(art);
                }

                await _dbContext.SaveChangesAsync();
                return new OkResult(); // Successfully saved or updated
            }
            catch (Exception)
            {
                return new BadRequestObjectResult("Failed to save the Art."); // Failed to save
            }

        }
	}
}

