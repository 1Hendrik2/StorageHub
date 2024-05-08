using Microsoft.AspNetCore.Identity;
using Storage.ServiceLibrary.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StorageHub.ServiceLibrary.Entities
{
    public class UserEntity : IdentityUser
    {
        public ICollection<StorageEntity> Storages { get; set; }
        public int? numberOfStorages { get; set; }
        public int? largestStorage { get; set; }
        public int? fullAccessStroages { get; set; }
        public int? lockedStorages { get; set; }
        public int? LimitedAccessStorages { get; set; }
    }
}
