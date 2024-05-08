using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client.Extensions.Msal;
using Storage.ServiceLibrary.Entities;
using Storage.ServiceLibrary.Repositories;
using StorageHub.Api.Dto;
using StorageHub.ServiceLibrary.Entities;
using StorageHub.ServiceLibrary.Extensions;
using StorageHub.ServiceLibrary.Repositories;

namespace StorageHub.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StorageController : ControllerBase
    {

        private readonly IStorageRepository _storageRepository;
        private readonly UserManager<UserEntity> _userManager;
        private readonly IMapper _mapper;
        public StorageController(IStorageRepository storageRepository, UserManager<UserEntity> userManager, IMapper mapper)
        {
            _storageRepository = storageRepository;
            _userManager = userManager;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<StorageEntity>))]
        [Authorize]
        public IActionResult GetStorages()
        {
            var storages = _mapper.Map<List<StorageDto>>(_storageRepository.GetAllStorages());

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(storages);
        }

        [HttpGet("{storageId}")]
        [Authorize]
        public IActionResult GetStorage([FromRoute] Guid storageId)
        {
            var storage = _mapper.Map<StorageDto>(_storageRepository.GetStorageById(storageId));

            if (storage == null)
                return NotFound("Storage was not found.");
            return Ok(storage);
        }

        [HttpGet("{storageId}/child-storages")]
        [Authorize]
        public IActionResult GetChildStorage([FromRoute] Guid storageId)
        {
            var storages = _mapper.Map<List<StorageDto>>(_storageRepository.GetChildStoragesByStorageId(storageId));

            if (storages == null)
                return NotFound("Storage was not found.");
            return Ok(storages);
        }

        [HttpGet("{userId}/user-storages")]
        [Authorize]
        public async Task<IActionResult> GetUserStorages(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            var userStorages = await _storageRepository.GetUserSotrages(user);
            return Ok(userStorages);
        }

        [HttpPost("search")]
        [Authorize]
        public async Task<IActionResult> SearchStorages([FromBody] SearchStoragesDto search)
        {
            if(search.Query == null)
            {
                return BadRequest("Search is empty");
            }

            var searchResult = _storageRepository.SearchStorages(search.Query, search.UserId);

            return Ok(searchResult);
            
        }

        [HttpPost("upload-storage")]
        [Authorize]
        public async Task<IActionResult> UploadStorage([FromForm]CreateStorageDto storageUpload)
        {
            if (storageUpload == null)
                return BadRequest(ModelState);

            var newStorage = _mapper.Map<StorageEntity>(storageUpload);

            newStorage.CubicSize = newStorage.Height * newStorage.Width * newStorage.Length;

            var isSuccess = _storageRepository.InsertStorage(newStorage);

            if (!isSuccess)
            {
                ModelState.AddModelError("", "Something went wrong while saving storage.");
                return StatusCode(500, ModelState);
            }

            var user = await _userManager.FindByIdAsync(storageUpload.UserId);

            if (user.numberOfStorages == null)
            {
                user.numberOfStorages = 1;
            }
            else
            {
                user.numberOfStorages = user.numberOfStorages + 1;
            }
            if (user.largestStorage == null || user.largestStorage < newStorage.CubicSize)
            {
                user.largestStorage = newStorage.CubicSize;
            }

            if (newStorage.AccessControl == "FullAccess")
            {
                if (user.fullAccessStroages == null)
                {
                    user.fullAccessStroages = 1;
                }
                else
                {
                    user.fullAccessStroages = user.fullAccessStroages + 1;
                }
            }
            else if (newStorage.AccessControl == "LimitedAccess")
            {
                if (user.LimitedAccessStorages == null)
                {
                    user.LimitedAccessStorages = 1;
                }
                else
                {
                    user.LimitedAccessStorages = user.LimitedAccessStorages + 1;
                }
            }
            else if (newStorage.AccessControl == "Locked")
            {
                if (user.lockedStorages == null)
                {
                    user.lockedStorages = 1;
                }
                else
                {
                    user.lockedStorages = user.lockedStorages + 1;
                }
            }

            await _userManager.UpdateAsync(user);

            return Ok("Storage created");
        }

        [HttpPost("{storageId}/upload-child")]
        [Authorize]
        public async Task<IActionResult> CreateChildStorage([FromForm] CreateStorageDto storageUpload, Guid storageId)
        {
            if (storageUpload == null)
                return BadRequest(ModelState);

            var parent = _storageRepository.GetStorageById(storageId);

            var newChildStorage = _mapper.Map<StorageEntity>(storageUpload);

            var doesFit = _storageRepository.DoesStorageFit(parent, parent.ChildStorages, newChildStorage);

            if(!doesFit)
            {
                return BadRequest("Storage does not fit.");
            }

            var isSuccess = _storageRepository.CreateChildStorage(newChildStorage, storageId);

            if (!isSuccess)
            {
                ModelState.AddModelError("", "Something went wrong while saving storage.");
                return StatusCode(500, ModelState);
            }

            var user = await _userManager.FindByIdAsync(newChildStorage.UserId);

            if (user.numberOfStorages == null)
            {
                user.numberOfStorages = 1;
            }
            else
            {
                user.numberOfStorages = user.numberOfStorages + 1;
            }
            if (user.largestStorage == null || user.largestStorage < newChildStorage.CubicSize)
            {
                user.largestStorage = newChildStorage.CubicSize;
            }

            if (newChildStorage.AccessControl == "FullAccess")
            {
                if (user.fullAccessStroages == null)
                {
                    user.fullAccessStroages = 1;
                }
                else
                {
                    user.fullAccessStroages = user.fullAccessStroages + 1;
                }
            }
            else if (newChildStorage.AccessControl == "LimitedAccess")
            {
                if (user.LimitedAccessStorages == null)
                {
                    user.LimitedAccessStorages = 1;
                }
                else
                {
                    user.LimitedAccessStorages = user.LimitedAccessStorages + 1;
                }
            }
            else if (newChildStorage.AccessControl == "Locked")
            {
                if (user.lockedStorages == null)
                {
                    user.lockedStorages = 1;
                }
                else
                {
                    user.lockedStorages = user.lockedStorages + 1;
                }
            }

            await _userManager.UpdateAsync(user);

            return Ok("Storage created");
        }

        [HttpPut("{storageId}/update-storage")]
        [Authorize]
        public async Task<IActionResult> UpdateStorage([FromForm] CreateStorageDto updateStorage, Guid storageId)
        {
            if (updateStorage == null)
                return BadRequest(ModelState);

            var storage = _storageRepository.GetStorageById(storageId);

            if (storage == null)
                return NotFound();

            storage.Title = updateStorage.Title;
            storage.Image = updateStorage.Image;
            storage.ImageFile = updateStorage.ImageFile;
            storage.SerialNumber = updateStorage.SerialNumber;
            storage.AccessControl = updateStorage.AccessControl;
            storage.Height = updateStorage.Height;
            storage.Width = updateStorage.Width;
            storage.Length = updateStorage.Length;
            storage.CubicSize = updateStorage.Length * updateStorage.Width * updateStorage.Height;


            if (!_storageRepository.StorageExists(storageId))
                return NotFound();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var storageMap = _mapper.Map<StorageEntity>(storage);

            if (!_storageRepository.UpdateStorage(storageMap))
            {
                ModelState.AddModelError("", "Something went wrong when updating storage.");
                return StatusCode(500, ModelState);
            }

            var user = await _userManager.FindByIdAsync(storageMap.UserId);

            if(user.numberOfStorages == null)
            {
                user.numberOfStorages = 1;
            }
            else
            {
                user.numberOfStorages = user.numberOfStorages + 1;
            }
            if(user.largestStorage == null || user.largestStorage < storageMap.CubicSize)
            {

            }

            if (storageMap.AccessControl == "FullAccess")
            {
                if(user.fullAccessStroages == null)
                {
                    user.fullAccessStroages = 1;
                }
                else
                {
                    user.fullAccessStroages = user.fullAccessStroages + 1;
                }  
            }
            else if (storageMap.AccessControl == "LimitedAccess")
            {
                if (user.LimitedAccessStorages == null)
                {
                    user.LimitedAccessStorages = 1;
                }
                else
                {
                    user.LimitedAccessStorages = user.LimitedAccessStorages + 1;
                }
            }
            else if (storageMap.AccessControl == "Locked")
            {
                if(user.lockedStorages == null)
                {
                    user.lockedStorages = 1;
                }
                else
                {
                    user.lockedStorages = user.lockedStorages + 1;
                }    
            }

            return Ok("Storage updated");
        }

        [HttpDelete("{storageId}/delete")]
        [Authorize]
        public IActionResult DeleteUser(Guid storageId)
        {
            if (!_storageRepository.StorageExists(storageId))
            {
                return NotFound();
            }

            var storageToDelete = _storageRepository.GetStorageById(storageId);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!_storageRepository.DeleteStorage(storageToDelete))
            {
                ModelState.AddModelError("", "Something went wrong while deleting storage.");
                return StatusCode(500, ModelState);
            }

            return Ok("Storage deleted");
        }
    }
}
