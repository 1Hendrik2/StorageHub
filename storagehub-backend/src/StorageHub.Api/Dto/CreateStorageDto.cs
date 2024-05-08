using System.ComponentModel.DataAnnotations;

namespace StorageHub.Api.Dto
{
    public class CreateStorageDto
    {
        [Required(ErrorMessage = "UserId is required")]
        public string UserId { get; set; }
        [Required(ErrorMessage = "Title is required")]
        public string Title { get; set; }
        public string? Image { get; set; }
        public IFormFile? ImageFile { get; set; }
        public string? SerialNumber { get; set; }
        public string? AccessControl { get; set; }
        public int Height { get; set; }
        public int Width { get; set; }
        public int Length { get; set; }
    }
}
