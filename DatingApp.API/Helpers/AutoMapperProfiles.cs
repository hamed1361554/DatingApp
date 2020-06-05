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
                .ForMember(dest => dest.PhotoUrl, options => options.MapFrom(src => GetMainPhotUrl(src)))
                .ForMember(dest => dest.Age, options => options.MapFrom(src => src.BirthDate.CalculateAge()));
            CreateMap<User, UserForDetailedDto>()
                .ForMember(dest => dest.PhotoUrl, options => options.MapFrom(src => GetMainPhotUrl(src)))
                .ForMember(dest => dest.Age, options => options.MapFrom(src => src.BirthDate.CalculateAge()));
            CreateMap<UserForUpdateDto, User>();
            CreateMap<Photo, PhotoDto>();
        }

        private string GetMainPhotUrl(User src)
        {
            return src.Photos.FirstOrDefault(p => p.IsMain).Url;
        }
    }
}