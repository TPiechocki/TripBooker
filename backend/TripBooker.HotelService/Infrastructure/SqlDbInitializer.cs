using TripBooker.Common.Hotel;
using TripBooker.HotelService.Model;

namespace TripBooker.HotelService.Infrastructure;

internal static class SqlDbInitializer
{
    public static void Initialize(HotelDbContext hotelContext)
    {
        hotelContext.Database.EnsureCreated();

        // Hotel options
        if (!hotelContext.HotelOption.Any())
        {
            var hotelOptions = new[]
            {
                new HotelOption
                {
                    Name = "Grand Hotel",
                    City = "Warszawa",
                    Address = "al. Jerozolimskie 5",
                    Description = "Hotel w warszawie",
                    Rating = 5,
                    PriceModifier = 1.5
                }
            };
            hotelContext.HotelOption.AddRange(hotelOptions);

            hotelContext.SaveChanges();

            // Room options
            if (!hotelContext.RoomOption.Any())
            {
                var roomOptions = new[]
                {
                    new RoomOption
                    {
                        Hotel = hotelOptions[0],
                        HotelId = hotelOptions[0].Id,
                        RoomType = RoomType.Small,
                        PriceModifier = 0.8
                    },
                    new RoomOption
                    {
                        Hotel = hotelOptions[0],
                        HotelId = hotelOptions[0].Id,
                        RoomType = RoomType.Medium,
                        PriceModifier = 1.0
                    },
                    new RoomOption
                    {
                        Hotel = hotelOptions[0],
                        HotelId = hotelOptions[0].Id,
                        RoomType = RoomType.Large,
                        PriceModifier = 1.2
                    }
                };
                hotelContext.RoomOption.AddRange(roomOptions);
            }

            hotelContext.SaveChanges();
        }

        
    }
}
