﻿using TripBooker.Common.TourOperator.Contract;

namespace TripBooker.TourOperator.Model.Extensions;

internal static class UpdateExtensions
{
    public static string Describe(this HotelUpdateContract contract, DateTime startDate, 
        HotelUpdateResponse hotelUpdateResponse)
    {
        var description = new System.Text.StringBuilder();
        description.AppendLine($"Updated hotel {hotelUpdateResponse.HotelDescription} with Id {contract.HotelId} " +
                               $"for the duration of {contract.HotelDays.Count} hotel days.");
        description.AppendLine($"Starting on {startDate:yyyy-MM-dd} day.");
        
        if (contract.PriceModifierFactor > 1.0)
            description.AppendLine($"Prices increased by {(int)((contract.PriceModifierFactor - 1.0) * 100)}%.");
        else
            description.AppendLine($"Prices decreased by {(int)((1.0 - contract.PriceModifierFactor) * 100)}%.");
        string temp;
        if (contract.RoomsStudioChange != 0)
        {
            temp = contract.RoomsStudioChange < 0 ? "taken" : "freed";
            description.AppendLine($"{Math.Abs(contract.RoomsStudioChange)} studio room(s) has been {temp}.");
        }
        if (contract.RoomsSmallChange != 0)
        {
            temp = contract.RoomsSmallChange < 0 ? "taken" : "freed";
            description.AppendLine($"{Math.Abs(contract.RoomsSmallChange)} small room(s) has been {temp}.");
        }
        if (contract.RoomsStudioChange != 0)
        {
            temp = contract.RoomsMediumChange < 0 ? "taken" : "freed";
            description.AppendLine($"{Math.Abs(contract.RoomsMediumChange)} medium room(s) has been {temp}.");
        }
        if (contract.RoomsStudioChange != 0)
        {
            temp = contract.RoomsLargeChange < 0 ? "taken" : "freed";
            description.AppendLine($"{Math.Abs(contract.RoomsLargeChange)} large room(s) has been {temp}.");
        }
        if (contract.RoomsApartmentChange != 0)
        {
            temp = contract.RoomsApartmentChange < 0 ? "taken" : "freed";
            description.AppendLine($"{Math.Abs(contract.RoomsApartmentChange)} apartment(s) has been {temp}.");
        }

        return description.ToString();
    }

    public static string Describe(this TransportUpdateContract contract, 
        TransportUpdateResponse response, 
        int oldPrice = -1)
    {
        var description = new System.Text.StringBuilder();
        description.AppendLine($"Updated transport {response.TransportDescription} with id {contract.Id}.");
        if (contract.PriceChangedFlag)
        {
            if (oldPrice != -1)
                description.AppendLine($"The ticket price changed from {oldPrice} to {contract.NewTicketPrice}.");
            else
                description.AppendLine($"The new ticket price is {contract.NewTicketPrice}.");
        }
        if (contract.AvailablePlacesChange != 0)
        {
            var temp = contract.AvailablePlacesChange < 0 ? "taken" : "freed";
            description.AppendLine($"{Math.Abs(contract.AvailablePlacesChange)} places has been {temp}.");
        }

        return description.ToString();
    }
}
