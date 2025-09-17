using System;
using System.Collections.Generic;

namespace EVCarbonMarketplace.Model.Entity;

public partial class Certificate
{
    public Guid Id { get; set; }

    public Guid? CarbonCreditId { get; set; }

    public int? SerialNumber { get; set; }

    public string? ImgUrl { get; set; }

    public string? Status { get; set; }

    public DateTime? IssuedAt { get; set; }

    public bool? IsActive { get; set; }

    public DateTime? CreateAt { get; set; }

    public DateTime? UpdateAt { get; set; }

    public DateTime? DeleteAt { get; set; }

    public Guid? BuyerId { get; set; }

    public Guid? IssuedById { get; set; }

    public virtual Account? Buyer { get; set; }

    public virtual CarbonCredit? CarbonCredit { get; set; }

    public virtual Account? IssuedBy { get; set; }
}
