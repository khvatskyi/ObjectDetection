using AutoMapper;
using BlazorUI.Models;
using ML5;

namespace BlazorUI.Mapping
{
    public class ObjectResultProfile : Profile
    {
        public ObjectResultProfile()
        {
            CreateMap<ObjectResult, BoxInfo>()
                .ReverseMap();
        }
    }
}
