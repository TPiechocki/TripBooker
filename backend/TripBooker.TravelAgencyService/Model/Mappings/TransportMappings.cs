using AutoMapper;
using TripBooker.Common.Transport.Contract;
using TripBooker.Common.TravelAgency.Model;

namespace TripBooker.TravelAgencyService.Model.Mappings;

internal class TransportMappings : Profile
{
    public TransportMappings()
    {
        CreateMap<TransportViewContract, TransportModel>();
    }
}