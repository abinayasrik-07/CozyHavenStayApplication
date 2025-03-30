using System;
using System.Collections.Generic;

namespace CozyHavenStayApp.Models;

public partial class Refund
{
    public int RefundId { get; set; }

    public int PaymentId { get; set; }

    public decimal RefundAmount { get; set; }

    public string RefundStatus { get; set; } = null!;

    public DateTime? RefundDate { get; set; }

    public virtual Payment Payment { get; set; } = null!;
}
