using System;
using System.Collections.Generic;

namespace CozyHavenStayApp.Models;

public partial class Payment
{
    public int PaymentId { get; set; }

    public int BookingId { get; set; }

    public int? DiscountId { get; set; }

    public decimal Amount { get; set; }

    public decimal FinalAmount { get; set; }

    public int PaymentMethodId { get; set; }

    public string PaymentStatus { get; set; } = null!;

    public string TransactionId { get; set; } = null!;

    public DateTime? PaymentDate { get; set; }

    public virtual Booking Booking { get; set; } = null!;

    public virtual Discount? Discount { get; set; }

    public virtual PaymentMethod PaymentMethod { get; set; } = null!;

    public virtual ICollection<Refund> Refunds { get; set; } = new List<Refund>();
}
