using AutoMapper;
using TripBooker.Common.Transport.Contract;

namespace TripBooker.TravelAgencyService.Model.Mappings;

internal class TransportMappings : Profile
{
    public TransportMappings()
    {
        CreateMap<TransportViewContract, TransportModel>();
    }
}