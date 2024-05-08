using Storage.ServiceLibrary.Entities;
using StorageHub.ServiceLibrary.Data;
using StorageHub.ServiceLibrary.Entities;

namespace StorageHub.Api
{
    public class Seed
    {
        private readonly DataContext dataContext;
        public Seed(DataContext context)
        {
            this.dataContext = context;
        }
        public void SeedDataContext()
        {
            
        }
    }
}
