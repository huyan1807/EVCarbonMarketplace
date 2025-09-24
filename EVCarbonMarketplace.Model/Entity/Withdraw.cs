using System;
using System.Collections.Generic;

namespace EVCarbonMarketplace.Model.Entity;

public partial class Withdraw
{
    public Guid Id { get; set; }

    public Guid AccountId { get; set; }

    public Guid BankAccountId { get; set; }

    public decimal? Amount { get; set; }

    public string? Status { get; set; }

    public string? Description { get; set; }

    public DateTime? CreateAt { get; set; }

    public DateTime? UpdateAt { get; set; }

    public string? ProofUrl { get; set; }

    public virtual Account Account { get; set; } = null!;

    public virtual BankAccount BankAccount { get; set; } = null!;

    public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}
