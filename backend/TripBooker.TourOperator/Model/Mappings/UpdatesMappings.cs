using AutoMapper;
using TripBooker.Common.TourOperator.Contract.Query;

namespace TripBooker.TourOperator.Model.Mappings;

internal class UpdatesMappings : Profile
{
    public UpdatesMappings()
    {
        CreateMap<UpdateModel, UpdateContract>();
    }
}
