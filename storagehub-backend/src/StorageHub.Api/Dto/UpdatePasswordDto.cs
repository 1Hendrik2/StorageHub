namespace StorageHub.Api.Dto
{
    public class UpdatePasswordDto
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string NewPassword { get; set; }
    }
}
