using System;
using System.Collections.Generic;

namespace EVCarbonMarketplace.Model.Entity;

public partial class Dispute
{
    public Guid Id { get; set; }

    public Guid? TransactionId { get; set; }

    public Guid? SendAccountId { get; set; }

    public string? Status { get; set; }

    public string? Type { get; set; }

    public string? Description { get; set; }

    public bool? IsActive { get; set; }

    public DateTime? CreateAt { get; set; }

    public DateTime? UpdateAt { get; set; }

    public DateTime? DeleteAt { get; set; }

    public virtual Account? SendAccount { get; set; }

    public virtual Transaction? Transaction { get; set; }
}
