using AutoMapper;
using TripBooker.Common.Transport.Contract;

namespace TripBooker.TransportService.Model.Mappings;

internal class TransportModelMappings : Profile
{
    public TransportModelMappings()
    {
        CreateMap<TransportModel, TransportViewContract>();
    }
}