using System;
using System.Collections.Generic;

namespace EVCarbonMarketplace.Model.Entity;

public partial class Bid
{
    public Guid Id { get; set; }

    public Guid? CarbonListingId { get; set; }

    public Guid? AccountId { get; set; }

    public DateTime? BidTime { get; set; }

    public decimal? Price { get; set; }

    public string? Status { get; set; }

    public bool? IsActive { get; set; }

    public DateTime? CreateAt { get; set; }

    public DateTime? UpdateAt { get; set; }

    public DateTime? DeleteAt { get; set; }

    public virtual Account? Account { get; set; }

    public virtual CarbonListing? CarbonListing { get; set; }
}
