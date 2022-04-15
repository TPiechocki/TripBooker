using TripBooker.Common.Transport.Contract;

namespace TripBooker.TransportService.Model.Mappings;

internal static class TransportViewContractMapper
{
    public static TransportViewContract MapFrom(TransportModel transport, TransportOption option)
    {
        return new TransportViewContract
        {
            Id = transport.Id,
            AvailablePlaces = transport.AvailablePlaces,
            DepartureAirportName = option.DepartureAirportName,
            DepartureAirportCode = option.DepartureAirportCode,
            DepartureDate = transport.DepartureDate,
            DestinationAirportCode = option.DestinationAirportCode,
            DestinationAirportName = option.DestinationAirportName,
            TicketPrice = transport.TicketPrice,
            Type = option.Type
        };
    }
}