using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Public_API.Models.Arts
{
	public class Art
	{
		public string? ArtId { get; set; }

        [JsonIgnore]
        public string? UserId { get; set; }

        [Required]
        public string? ArtSource { get; set; }

		[Required]
        public string? ArtName { get; set; }

        [Required]
        public int? CanvasHeight { get; set; }

        [Required]
        public int? CanvasWidth { get; set; }

	}
}

