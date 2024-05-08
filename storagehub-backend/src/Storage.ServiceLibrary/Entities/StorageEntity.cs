using Microsoft.AspNetCore.Http;
using StorageHub.ServiceLibrary.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Storage.ServiceLibrary.Entities
{
    public class StorageEntity
    {
        public Guid Id { get; set; }
        public string UserId { get; set; }
        public Guid? ParentStorageId { get; set; }
        public string? ParentTitles { get; set; }
        public required string Title { get; set; }
        public string? Image { get; set; }
        [NotMapped]
        public IFormFile ImageFile { get; set; }
        public string? SerialNumber { get; set; }
        public string? AccessControl { get; set; }
        public int Height { get; set; }
        public int Width { get; set; }
        public int Length { get; set; }
        public int CubicSize { get; set; }
        public virtual StorageEntity? ParentStorage { get; set; }
        public virtual ICollection<StorageEntity>? ChildStorages { get; set; }
        public UserEntity User { get; set; }
    }

}
