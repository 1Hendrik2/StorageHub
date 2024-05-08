using StorageHub.ServiceLibrary.Entities;

namespace StorageHub.Api.Dto
{
    public class StorageDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string? ParentStorageId { get; set; }
        public required string Title { get; set; }
        public string ParentTitle { get; set; }
        public string? Image { get; set; }
        public string? SerialNumber { get; set; }
        public string? AccessControl { get; set; }
        public int Height { get; set; }
        public int Width { get; set; }
        public int Length { get; set; }
        public UserEntity User { get; set; }
    }
}
