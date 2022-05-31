using AutoMapper;
using TripBooker.Common.Order;
using TripBooker.Common.Statistics;

namespace TripBooker.TravelAgencyService.Model.Mappings;

internal class StatisticsMappings : Profile
{
    public StatisticsMappings()
    {
        CreateMap<OrderData, NewReservationEvent>();
    }
}