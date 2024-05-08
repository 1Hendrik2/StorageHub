using AutoMapper;
using Storage.ServiceLibrary.Entities;
using StorageHub.Api.Dto;
using StorageHub.ServiceLibrary.Entities;

namespace StorageHub.Api.Helper
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            CreateMap<StorageEntity, StorageDto>();
            CreateMap<UserEntity, CreateUserDto>();
            CreateMap<CreateUserDto, UserEntity>();
            CreateMap<UpdateUserDto,  UserEntity>();
            CreateMap<CreateStorageDto, StorageEntity>();
            CreateMap<StorageEntity, CreateStorageDto>();
        }
    }
}
