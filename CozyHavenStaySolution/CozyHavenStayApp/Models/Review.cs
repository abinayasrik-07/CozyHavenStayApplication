using System;
using System.Collections.Generic;

namespace CozyHavenStayApp.Models;

public partial class Review
{
    public int ReviewId { get; set; }

    public int BookingId { get; set; }

    public int HotelId { get; set; }

    public int Rating { get; set; }

    public string? Comment { get; set; }

    public DateTime? DatePosted { get; set; }

    public virtual Booking Booking { get; set; } = null!;

    public virtual Hotel Hotel { get; set; } = null!;
}
