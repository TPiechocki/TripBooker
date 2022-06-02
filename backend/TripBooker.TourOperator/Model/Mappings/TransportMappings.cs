using AutoMapper;
using TripBooker.Common.Transport.Contract;

namespace TripBooker.TourOperator.Model.Mappings;

public class TransportMappings : Profile
{
    public TransportMappings()
    {
        CreateMap<TransportViewContract, TransportModel>();
        CreateMap<TransportModel, TransportViewContract>()
            .ForMember(dest => dest.Id, opt => opt.Ignore());
    }
}