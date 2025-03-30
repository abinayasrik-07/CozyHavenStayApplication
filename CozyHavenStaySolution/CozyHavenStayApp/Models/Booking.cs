using System;
using System.Collections.Generic;

namespace CozyHavenStayApp.Models;

public partial class Booking
{
    public int BookingId { get; set; }

    public int UserId { get; set; }

    public int RoomId { get; set; }

    public DateOnly CheckInDate { get; set; }

    public DateOnly CheckOutDate { get; set; }

    public decimal TotalPrice { get; set; }

    public string BookingStatus { get; set; } = null!;

    public DateTime? DateOfBooking { get; set; }

    public virtual ICollection<BookingCancellation> BookingCancellations { get; set; } = new List<BookingCancellation>();

    public virtual ICollection<Discount> Discounts { get; set; } = new List<Discount>();

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();

    public virtual Room Room { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
