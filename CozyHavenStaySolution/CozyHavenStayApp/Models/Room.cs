using System;
using System.Collections.Generic;

namespace CozyHavenStayApp.Models;

public partial class Room
{
    public int RoomId { get; set; }

    public int HotelId { get; set; }

    public string Size { get; set; } = null!;

    public int RoomTypeId { get; set; }

    public decimal BaseFare { get; set; }

    public int MaxOccupancy { get; set; }

    public bool? IsAc { get; set; }

    public bool? IsAvailable { get; set; }

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    public virtual Hotel Hotel { get; set; } = null!;

    public virtual RoomType RoomType { get; set; } = null!;
}
