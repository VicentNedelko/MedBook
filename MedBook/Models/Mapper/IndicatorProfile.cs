using AutoMapper;
using MedBook.Models.ViewModels;

namespace MedBook.Models.Mapper
{
    public class IndicatorProfile : Profile
    {
        public IndicatorProfile()
        {
            CreateMap<SampleIndicator, IndicatorVM>()
                    .ForMember(dest => dest.ReferentMin, opt => opt.MapFrom(src => src.ReferenceMin))
                    .ForMember(dest => dest.ReferentMax, opt => opt.MapFrom(src => src.ReferenceMax));
        }
    }
}
