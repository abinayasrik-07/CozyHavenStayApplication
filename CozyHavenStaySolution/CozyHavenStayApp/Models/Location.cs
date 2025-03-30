using System;
using System.Collections.Generic;

namespace CozyHavenStayApp.Models;

public partial class Location
{
    public int LocationId { get; set; }

    public string City { get; set; } = null!;

    public string Country { get; set; } = null!;

    public virtual ICollection<Hotel> Hotels { get; set; } = new List<Hotel>();
}
