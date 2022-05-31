using AutoMapper;
using TripBooker.Common.Statistics;

namespace TripBooker.StatisticsService.Model;

internal class Mappings : Profile
{
    public Mappings()
    {
        CreateMap<NewReservationEvent, ReservationModel>()
            .ForMember(dest => dest.TimeStamp,
                opt => opt.MapFrom(_ => DateTime.UtcNow));
    }
}