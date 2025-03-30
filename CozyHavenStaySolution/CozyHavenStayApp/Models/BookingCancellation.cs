using System;
using System.Collections.Generic;

namespace CozyHavenStayApp.Models;

public partial class BookingCancellation
{
    public int CancellationId { get; set; }

    public int BookingId { get; set; }

    public DateTime? CancellationDate { get; set; }

    public string? ReasonForCancellation { get; set; }

    public virtual Booking Booking { get; set; } = null!;
}
