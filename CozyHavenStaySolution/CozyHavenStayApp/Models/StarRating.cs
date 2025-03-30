using System;
using System.Collections.Generic;

namespace CozyHavenStayApp.Models;

public partial class StarRating
{
    public int StarRatingId { get; set; }

    public int Rating { get; set; }

    public virtual ICollection<Hotel> Hotels { get; set; } = new List<Hotel>();
}
