using System;
using System.Collections.Generic;

namespace EVCarbonMarketplace.Model.Entity;

public partial class CarbonListing
{
    public Guid Id { get; set; }

    public Guid? CarbonCreditId { get; set; }

    public Guid? AccountId { get; set; }

    public decimal? Price { get; set; }

    public decimal? Quantity { get; set; }

    public string? Type { get; set; }

    public string? Status { get; set; }

    public DateTime? StartTime { get; set; }

    public DateTime? EndTime { get; set; }

    public bool? IsActive { get; set; }

    public DateTime? CreateAt { get; set; }

    public DateTime? UpdateAt { get; set; }

    public DateTime? DeleteAt { get; set; }

    public virtual Account? Account { get; set; }

    public virtual ICollection<Bid> Bids { get; set; } = new List<Bid>();

    public virtual CarbonCredit? CarbonCredit { get; set; }

    public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}
