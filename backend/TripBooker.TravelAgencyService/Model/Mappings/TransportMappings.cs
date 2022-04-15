using AutoMapper;
using TripBooker.Common.Order;
using TripBooker.Common.Order.Transport;
using TripBooker.Common.Transport.Contract;

namespace TripBooker.TravelAgencyService.Model.Mappings;

internal class TransportMappings : Profile
{
    public TransportMappings()
    {
        CreateMap<TransportViewContract, TransportModel>();
        CreateMap<TransportReservationAccepted, OrderData>()
            .ForMember(dest => dest.TransportPrice, opt => opt.MapFrom(src => src.Price));
    }
}