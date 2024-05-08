using Storage.ServiceLibrary.Entities;
using System.ComponentModel.DataAnnotations;

namespace StorageHub.Api.Dto
{
    public class CreateUserDto
    {
        [Required]
        public string? UserName { get; set; }
        [Required]
        [EmailAddress]
        public string? Email { get; set; }
        [Required]
        public string? Password { get; set; }
        
    }
}
