using Dapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Client.Extensions.Msal;
using Storage.ServiceLibrary.Entities;
using StorageHub.ServiceLibrary.Data;
using StorageHub.ServiceLibrary.Entities;
using StorageHub.ServiceLibrary.Repositories;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Storage.ServiceLibrary.Repositories
{
    public interface IStorageRepository
    {
        ICollection<StorageEntity> GetAllStorages();
        StorageEntity GetStorageById(Guid id);
        bool StorageExists(Guid id);
        bool InsertStorage(StorageEntity newStorage);
        bool UpdateStorage(StorageEntity storage);
        bool DeleteStorage(StorageEntity storage);
        ICollection<StorageEntity> SearchStorages(string query, string userId);
        ICollection<StorageEntity> GetChildStoragesByStorageId(Guid id);
        Task<List<StorageEntity>> GetUserSotrages(UserEntity user);
        bool CreateChildStorage(StorageEntity newChildStorage, Guid id);
        bool DoesStorageFit(StorageEntity parentStorage, IEnumerable<StorageEntity> childStorages, StorageEntity newChild);
    }
    public class StorageRepository : IStorageRepository
    {
        private readonly DataContext _context;
        private readonly ILogger _logger;
        public StorageRepository(DataContext context, IWebHostEnvironment hostEnvironment, ILogger<StorageRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public StorageEntity GetStorageById(Guid id)
        {
            return _context.Storages.Where(s => s.Id == id).FirstOrDefault();
        }

        public ICollection<StorageEntity> GetAllStorages()
        {
            return _context.Storages.OrderBy(s => s.Id).ToList();
        }

        public bool StorageExists(Guid id)
        {
            return _context.Storages.Any(s => s.Id == id);
        }

        public bool InsertStorage(StorageEntity newStorage)
        {
            _context.Add(newStorage);

            return Save();
        }

        public bool UpdateStorage(StorageEntity storage)
        {
            _context.Update(storage);

            return Save();
        }

        public bool DeleteStorage(StorageEntity storage)
        {
            var parentStorage = _context.Storages.Include(s => s.ChildStorages).FirstOrDefault(s => s.Id == storage.Id);

            if (parentStorage != null)
            {
                DeleteChildStorages(parentStorage.ChildStorages.ToList());

                _context.Storages.Remove(parentStorage);
            }
            else
            {
                _context.Remove(storage);
            }

            return Save();
        }
        private void DeleteChildStorages(List<StorageEntity> childStorages)
        {
            foreach (var childStorage in childStorages)
            {
                if(childStorage.ChildStorages != null) 
                { 
                DeleteChildStorages(childStorage.ChildStorages.ToList());
                }

                _context.Storages.Remove(childStorage);
            }
        }
        public bool Save()
        {
            var saved = _context.SaveChanges();
            return saved > 0 ? true : false;
        }

        public async Task<List<StorageEntity>> GetUserSotrages(UserEntity user)
        {
            return await _context.Storages.Where(s => s.UserId == user.Id && s.ParentStorageId == null).ToListAsync();
        }

        public bool CreateChildStorage(StorageEntity newChildStorage, Guid id)
        {
            var storage = _context.Storages.Where(s => s.Id == id).FirstOrDefault();

            if(storage.ParentTitles == null)
            {
                newChildStorage.ParentTitles = storage.Title;
            }
            else
            {
                newChildStorage.ParentTitles = GetParentTitle(storage.ParentTitles, newChildStorage.Title);
            }

            if (storage != null)
            {
                newChildStorage.ParentStorageId = id;
                if (storage.ChildStorages == null)
                {
                    storage.ChildStorages = new List<StorageEntity>();
                }
                _context.Update(storage);
                _context.Add(newChildStorage);
                return Save();
            }
            else
                return false;
        }

        public string GetParentTitle(string parentTitles, string title)
        {
            return parentTitles + " / " + title;
        }

        public ICollection<StorageEntity> GetChildStoragesByStorageId(Guid id)
        {
            return _context.Storages.Where(cs => cs.ParentStorageId == id).ToList();
        }

        public bool DoesStorageFit(StorageEntity parentStorage, IEnumerable<StorageEntity> childStorages, StorageEntity newChild)
        {
            double parentCubicArea = parentStorage.Length * parentStorage.Width * parentStorage.Height;

            double totalChildCubicArea = 0;
            if(childStorages == null)
            {
                totalChildCubicArea += newChild.Length * newChild.Width * newChild.Height;
            }
            else 
            {
                foreach (var childStorage in childStorages)
                {
                    totalChildCubicArea += childStorage.Length * childStorage.Width * childStorage.Height + (newChild.Length * newChild.Width * newChild.Height);
                }
            }
            

            return totalChildCubicArea <= parentCubicArea;
        }

        public ICollection<StorageEntity> SearchStorages(string query, string userId)
        {
            query = query.ToLower();

            var results = _context.Storages.Where(item =>
                item.UserId == userId &&
                item.Title.ToLower().Contains(query) ||
                item.SerialNumber.Contains(query)
            ).ToList();

            return results;
        }

    }
}
