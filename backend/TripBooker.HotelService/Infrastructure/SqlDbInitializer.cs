﻿using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using TripBooker.Common.Hotel;
using TripBooker.HotelService.Model;
using TripBooker.HotelService.Services;

namespace TripBooker.HotelService.Infrastructure;

internal static class SqlDbInitializer
{
    public static void Initialize(HotelDbContext hotelContext, IHotelService hotelService)
    {
        hotelContext.Database.EnsureCreated();

        // Hotel options
        if (!hotelContext.HotelOption.Any())
        {
            AddHotelOptions(hotelContext.HotelOption);
        }
        hotelContext.SaveChanges();

        // Hotel Occupation
        if (!hotelContext.HotelEvent.Any() && !hotelContext.HotelOccupationView.Any())
        {
            hotelService.AddNewHotelDay(DateTime.SpecifyKind(new DateTime(2022, 07, 01), DateTimeKind.Utc), default, 30)
                .GetAwaiter().GetResult();
        }
            
    }

    private static void AddHotelOptions(DbSet<HotelOption> dbSet)
    {
        var csvConfig = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HeaderValidated = null,
            MissingFieldFound = null
        };

        using var streamReader = File.OpenText("hotels.csv");
        using var csvReader = new CsvReader(streamReader, csvConfig);

        var hotelOptions = csvReader.GetRecords<HotelOption>()
            .DistinctBy(x => x.Code)
            .ToList();

        hotelOptions.ForEach(GenerateRoomsAndDetails);

        dbSet.AddRange(hotelOptions);
    }

    private static void GenerateRoomsAndDetails(HotelOption hotel)
    {
        var random = new Random();
        var rooms = new List<RoomOption>();

        var hotelSize = random.Next(1, 5);
        var rating = hotel.Rating == 0 ? 3 : (int)hotel.Rating;

        hotel.AllInclusive = hotel.Rating > 3 || random.NextDouble() < (hotel.Rating + 2) / 10;
        hotel.PriceModifier = 1.2 - random.NextDouble() + hotel.Rating / 10;

        // Studio
        for (var i = 0; i < random.Next((hotelSize - 1) * 2, 10 / rating * hotelSize); i++)
        {
            rooms.Add(new RoomOption
            {
                Hotel = hotel,
                RoomType = RoomType.Studio,
                PriceModifier = 0.5
            });
        }

        // Small
        for (var i = 0; i < random.Next(1, hotelSize * 10 - 5 ); i++)
        {
            rooms.Add(new RoomOption
            {
                Hotel = hotel,
                RoomType = RoomType.Small,
                PriceModifier = 1.0
            });
        }

        // Medium
        for (var i = 0; i < random.Next((hotelSize - 1) * 2, hotelSize * 8 - 4); i++)
        {
            rooms.Add(new RoomOption
            {
                Hotel = hotel,
                RoomType = RoomType.Medium,
                PriceModifier = 1.8
            });
        }

        // Large
        for (var i = 0; i < random.Next((hotelSize - 1) * 2, hotelSize * 5 - 3 ); i++)
        {
            rooms.Add(new RoomOption
            {
                Hotel = hotel,
                RoomType = RoomType.Large,
                PriceModifier = 2.5
            });
        }

        // Apartment
        for (var i = 0; i < random.Next((hotelSize - 1) * Math.Max(rating - 3, 0), hotelSize * Math.Max(rating - 2, 0) ); i++)
        {
            rooms.Add(new RoomOption
            {
                Hotel = hotel,
                RoomType = RoomType.Apartment,
                PriceModifier = 5.0
            });
        }

        hotel.Rooms = rooms;
    }
}
