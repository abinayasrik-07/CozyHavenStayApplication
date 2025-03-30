using System;
using System.Collections.Generic;

namespace CozyHavenStayApp.Models;

public partial class ProofOfUser
{
    public int ProofTypeId { get; set; }

    public string TypeName { get; set; } = null!;

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
