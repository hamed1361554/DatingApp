using System.Linq;
using AutoMapper;
using DatingApp.API.Dto;
using DatingApp.API.Models;

namespace DatingApp.API.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<User, UserForListDto>()
                .ForMember(dest => dest.PhotoUrl, options => options.MapFrom(src => GetMainPhotoUrl(src)))
                .ForMember(dest => dest.Age, options => options.MapFrom(src => src.BirthDate.CalculateAge()));
            CreateMap<User, UserForDetailedDto>()
                .ForMember(dest => dest.PhotoUrl, options => options.MapFrom(src => GetMainPhotoUrl(src)))
                .ForMember(dest => dest.Age, options => options.MapFrom(src => src.BirthDate.CalculateAge()));
            CreateMap<UserForUpdateDto, User>();
            CreateMap<Photo, PhotoDto>();
            CreateMap<PhotoForCreateDto, Photo>();
            CreateMap<Photo, PhotoForReturnDto>();
            CreateMap<UserForRegisterDto, User>();
        }

        private string GetMainPhotoUrl(User src)
        {
            return src.Photos.FirstOrDefault(p => p.IsMain)?.Url;
        }
    }
}