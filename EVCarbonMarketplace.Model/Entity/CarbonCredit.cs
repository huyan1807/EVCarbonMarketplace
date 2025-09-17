using System;
using System.Collections.Generic;

namespace EVCarbonMarketplace.Model.Entity;

public partial class CarbonCredit
{
    public Guid Id { get; set; }

    public Guid? CarbonEmissionId { get; set; }

    public Guid? AccountId { get; set; }

    public decimal? Credits { get; set; }

    public string? Status { get; set; }

    public bool? IsActive { get; set; }

    public DateTime? CreateAt { get; set; }

    public DateTime? UpdateAt { get; set; }

    public DateTime? DeleteAt { get; set; }

    public virtual Account? Account { get; set; }

    public virtual CarbonEmission? CarbonEmission { get; set; }

    public virtual ICollection<CarbonListing> CarbonListings { get; set; } = new List<CarbonListing>();

    public virtual ICollection<Certificate> Certificates { get; set; } = new List<Certificate>();
}
