using System;
using System.Collections.Generic;

namespace EVCarbonMarketplace.Model.Entity;

public partial class BankAccount
{
    public Guid Id { get; set; }

    public Guid AccountId { get; set; }

    public string? BankCode { get; set; }

    public string? BankName { get; set; }

    public string? BankAccountNumber { get; set; }

    public string? BankAccountHolder { get; set; }

    public bool? IsDefault { get; set; }

    public bool? IsActive { get; set; }

    public DateTime? CreateAt { get; set; }

    public DateTime? UpdateAt { get; set; }

    public string? LogoUrl { get; set; }

    public virtual Account Account { get; set; } = null!;

    public virtual ICollection<Withdraw> Withdraws { get; set; } = new List<Withdraw>();
}
