using TripBooker.Common.Hotel;
using TripBooker.HotelService.Contract;
using TripBooker.HotelService.Model;
using TripBooker.HotelService.Model.Events;
using TripBooker.HotelService.Repositories;

namespace TripBooker.HotelService.Services;

internal interface IHotelReservationService
{
    Task AddNewReservation(NewReservationContract newReservationContract, CancellationToken cancellationToken);
}

internal class HotelReservationService : IHotelReservationService
{
    private readonly IReservationEventRepository _reservationRepository;
    private readonly IHotelEventRepository _hotelRepository;

    public HotelReservationService(ReservationEventRepository reservationRepository, HotelEventRepository hotelRepository)
    {
        _reservationRepository = reservationRepository;
        _hotelRepository = hotelRepository;
    }

    public async Task AddNewReservation(NewReservationContract reservation, CancellationToken cancellationToken)
    {
        var data = new NewReservationEventData(reservation.HotelDays,
                                               reservation.RoomsStudio,
                                               reservation.RoomsSmall,
                                               reservation.RoomsMedium,
                                               reservation.RoomsLarge,
                                               reservation.RoomsApartment);
        var reservationStreamId = await _reservationRepository.AddNewAsync(data, cancellationToken);

        var transactionSuccesfull = false;

        do
        {
            transactionSuccesfull = true;

            // Check for all days
            var checkSuccesfull = true;
            foreach(var hotelday in data.HotelDays)
            {
                var hotelEvents = await _hotelRepository.GetTransportEventsAsync(hotelday, cancellationToken);
                var occupation = HotelOccupationBuilder.Build(hotelEvents);

                // Check if enough rooms awailable
                if (occupation.RoomsStudio < data.RoomsStudio 
                    && occupation.RoomsSmall < data.RoomsSmall 
                    && occupation.RoomsMedium < data.RoomsMedium 
                    && occupation.RoomsLarge < data.RoomsLarge 
                    && occupation.RoomsApartment < data.RoomsApartment)
                {
                    checkSuccesfull = false;
                    break;
                }
            }

            if (!checkSuccesfull)
            {
                // There is not enough free rooms
                await _reservationRepository.AddRejectedAsync(reservationStreamId, 1, cancellationToken);
                break;
            }
            
            // Check succesfull can reserve
            // TODO

        } 
        while (!transactionSuccesfull);

    }
}
