using System;
using System.Collections.Generic;

namespace CozyHavenStayApp.Models;

public partial class Discount
{
    public int DiscountId { get; set; }

    public int BookingId { get; set; }

    public string DiscountCode { get; set; } = null!;

    public decimal DiscountPercentage { get; set; }

    public DateTime? AppliedAt { get; set; }

    public DateTime ExpiryDate { get; set; }

    public virtual Booking Booking { get; set; } = null!;

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
}
