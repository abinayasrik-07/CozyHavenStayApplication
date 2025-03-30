using System;
using System.Collections.Generic;

namespace CozyHavenStayApp.Models;

public partial class Hotel
{
    public int HotelId { get; set; }

    public int UserId { get; set; }

    public string HotelName { get; set; } = null!;

    public int LocationId { get; set; }

    public string? Description { get; set; }

    public int StarRatingId { get; set; }

    public virtual Location Location { get; set; } = null!;

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();

    public virtual ICollection<Room> Rooms { get; set; } = new List<Room>();

    public virtual StarRating StarRating { get; set; } = null!;

    public virtual User User { get; set; } = null!;

    public virtual ICollection<Amenity> Amenities { get; set; } = new List<Amenity>();
}
