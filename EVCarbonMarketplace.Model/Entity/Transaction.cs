using System;
using System.Collections.Generic;

namespace EVCarbonMarketplace.Model.Entity;

public partial class Transaction
{
    public Guid Id { get; set; }

    public Guid? WalletId { get; set; }

    public Guid? DepositId { get; set; }

    public string? Type { get; set; }

    public string? Status { get; set; }

    public decimal? Amount { get; set; }

    public bool? IsActive { get; set; }

    public DateTime? CreateAt { get; set; }

    public DateTime? UpdateAt { get; set; }

    public DateTime? DeleteAt { get; set; }

    public Guid? BuyerId { get; set; }

    public Guid? SellerId { get; set; }

    public Guid? CarbonListingId { get; set; }

    public string? Description { get; set; }

    public Guid? WithdrawId { get; set; }

    public decimal? FeeRate { get; set; }

    public virtual Account? Buyer { get; set; }

    public virtual CarbonListing? CarbonListing { get; set; }

    public virtual Deposit? Deposit { get; set; }

    public virtual ICollection<Dispute> Disputes { get; set; } = new List<Dispute>();

    public virtual Account? Seller { get; set; }

    public virtual Wallet? Wallet { get; set; }

    public virtual Withdraw? Withdraw { get; set; }
}
