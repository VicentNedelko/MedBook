using AutoMapper;
using MedBook.Models.ViewModels;

namespace MedBook.Models.Mapper
{
    public class ResearchProfile : Profile
    {
        public ResearchProfile()
        {
            CreateMap<Research, ResearchVM>()
                .ForMember(dest => dest.Laboratory, opt => opt.MapFrom(src => src.Order));
        }
    }
}
