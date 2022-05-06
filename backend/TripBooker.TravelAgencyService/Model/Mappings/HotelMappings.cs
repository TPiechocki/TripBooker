using AutoMapper;
using TripBooker.Common.Hotel.Contract;

namespace TripBooker.TravelAgencyService.Model.Mappings;

internal class HotelMappings : Profile
{
    public HotelMappings()
    {
        CreateMap<HotelOccupationViewContract, HotelOccupationModel>();
        CreateMap<HotelOccupationModel, HotelOccupationViewContract>()
            .ForMember(dest => dest.Id, opt => opt.Ignore());
    }
}