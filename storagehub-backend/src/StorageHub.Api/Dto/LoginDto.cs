using Storage.ServiceLibrary.Entities;
using System.ComponentModel.DataAnnotations;

namespace StorageHub.Api.Dto
{
    public class LoginDto
    {
        [Required]
        public string UserName { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
