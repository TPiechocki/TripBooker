using AutoMapper;
using TripBooker.TransportService.Model.Events;

namespace TripBooker.TransportService.Model.Mappings;

public class NewTransportEventMappings : Profile
{
    public NewTransportEventMappings()
    {
        CreateMap<NewTransportEvent, TransportView>();
    }
}