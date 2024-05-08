using StorageHub.ServiceLibrary.Data;
using StorageHub.ServiceLibrary.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StorageHub.ServiceLibrary.Repositories
{
    public interface IUsersRepository
    {

    }
    public class UserRepository
    {
        private readonly DataContext _context;
        public UserRepository(DataContext context)
        {
            _context = context;
        }
    }
}
