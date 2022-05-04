using System;
using System.ComponentModel.DataAnnotations;

namespace TripBooker.Common.Order.Payment;

public class Payment
{
    public int Price { get; set; }
    public Guid OrderId { get; set; }
    public Guid PaymentId { get; set; }
}