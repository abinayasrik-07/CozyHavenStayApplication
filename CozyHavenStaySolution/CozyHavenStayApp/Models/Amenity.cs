using System;
using System.Collections.Generic;

namespace CozyHavenStayApp.Models;

public partial class Amenity
{
    public int AmenityId { get; set; }

    public string AmenityName { get; set; } = null!;

    public virtual ICollection<Hotel> Hotels { get; set; } = new List<Hotel>();
}
