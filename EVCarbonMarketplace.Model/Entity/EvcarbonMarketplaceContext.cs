using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace EVCarbonMarketplace.Model.Entity;

public partial class EvcarbonMarketplaceContext : DbContext
{
    public EvcarbonMarketplaceContext()
    {
    }

    public EvcarbonMarketplaceContext(DbContextOptions<EvcarbonMarketplaceContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Account> Accounts { get; set; }

    public virtual DbSet<Bid> Bids { get; set; }

    public virtual DbSet<CarbonCredit> CarbonCredits { get; set; }

    public virtual DbSet<CarbonEmission> CarbonEmissions { get; set; }

    public virtual DbSet<CarbonListing> CarbonListings { get; set; }

    public virtual DbSet<Certificate> Certificates { get; set; }

    public virtual DbSet<Deposit> Deposits { get; set; }

    public virtual DbSet<Dispute> Disputes { get; set; }

    public virtual DbSet<ElectricVehicle> ElectricVehicles { get; set; }

    public virtual DbSet<Transaction> Transactions { get; set; }

    public virtual DbSet<VehicleTelemetry> VehicleTelemetries { get; set; }

    public virtual DbSet<VehicleType> VehicleTypes { get; set; }

    public virtual DbSet<Wallet> Wallets { get; set; }

    public static string GetConnectionString(string connectionStringName)
    {
        var config = new ConfigurationBuilder()
            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
            .AddJsonFile("appsettings.json")
            .Build();

        string connectionString = config.GetConnectionString(connectionStringName);
        return connectionString;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer(GetConnectionString("DefautDB")).UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Account>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Account__3214EC078D45E594");

            entity.ToTable("Account");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.CreateAt).HasColumnType("datetime");
            entity.Property(e => e.DeleteAt).HasColumnType("datetime");
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.FullName).HasMaxLength(50);
            entity.Property(e => e.Gender).HasMaxLength(15);
            entity.Property(e => e.Password).HasMaxLength(50);
            entity.Property(e => e.Phone).HasMaxLength(15);
            entity.Property(e => e.Role).HasMaxLength(15);
            entity.Property(e => e.UpdateAt).HasColumnType("datetime");
            entity.Property(e => e.Username).HasMaxLength(50);
        });

        modelBuilder.Entity<Bid>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Bid__3214EC070EFA4DD6");

            entity.ToTable("Bid");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.BidTime).HasColumnType("datetime");
            entity.Property(e => e.CreateAt).HasColumnType("datetime");
            entity.Property(e => e.DeleteAt).HasColumnType("datetime");
            entity.Property(e => e.Price).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.Status).HasMaxLength(15);
            entity.Property(e => e.UpdateAt).HasColumnType("datetime");

            entity.HasOne(d => d.Account).WithMany(p => p.Bids)
                .HasForeignKey(d => d.AccountId)
                .HasConstraintName("FK_Bid_AccountId");

            entity.HasOne(d => d.CarbonListing).WithMany(p => p.Bids)
                .HasForeignKey(d => d.CarbonListingId)
                .HasConstraintName("FK_Bid_CarbonListingId");
        });

        modelBuilder.Entity<CarbonCredit>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__CarbonCr__3214EC07BF01CA54");

            entity.ToTable("CarbonCredit");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.CreateAt).HasColumnType("datetime");
            entity.Property(e => e.Credits).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.DeleteAt).HasColumnType("datetime");
            entity.Property(e => e.Status).HasMaxLength(20);
            entity.Property(e => e.UpdateAt).HasColumnType("datetime");

            entity.HasOne(d => d.Account).WithMany(p => p.CarbonCredits)
                .HasForeignKey(d => d.AccountId)
                .HasConstraintName("FK_CarbonCredit_AccountId");

            entity.HasOne(d => d.CarbonEmission).WithMany(p => p.CarbonCredits)
                .HasForeignKey(d => d.CarbonEmissionId)
                .HasConstraintName("FK_CarbonCredit_CarbonEmissionId");
        });

        modelBuilder.Entity<CarbonEmission>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__CarbonEm__3214EC07EFCBCEF8");

            entity.ToTable("CarbonEmission");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Co2reduced)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("CO2Reduced");
            entity.Property(e => e.CreateAt).HasColumnType("datetime");
            entity.Property(e => e.DeleteAt).HasColumnType("datetime");
            entity.Property(e => e.DistanceTravelled).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.EnergyConsumed).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.PeriodEnd).HasColumnType("datetime");
            entity.Property(e => e.PeriodStart).HasColumnType("datetime");
            entity.Property(e => e.Status).HasMaxLength(50);
            entity.Property(e => e.UpdateAt).HasColumnType("datetime");

            entity.HasOne(d => d.ElectricVehicle).WithMany(p => p.CarbonEmissions)
                .HasForeignKey(d => d.ElectricVehicleId)
                .HasConstraintName("FK_CarbonEmission_ElectricVehicleId");
        });

        modelBuilder.Entity<CarbonListing>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__CarbonLi__3214EC07E51737D8");

            entity.ToTable("CarbonListing");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.CreateAt).HasColumnType("datetime");
            entity.Property(e => e.DeleteAt).HasColumnType("datetime");
            entity.Property(e => e.EndTime).HasColumnType("datetime");
            entity.Property(e => e.Price).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.Quantity).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.StartTime).HasColumnType("datetime");
            entity.Property(e => e.Status).HasMaxLength(50);
            entity.Property(e => e.Type).HasMaxLength(50);
            entity.Property(e => e.UpdateAt).HasColumnType("datetime");

            entity.HasOne(d => d.Account).WithMany(p => p.CarbonListings)
                .HasForeignKey(d => d.AccountId)
                .HasConstraintName("FK_CarbonListing_AccountId");

            entity.HasOne(d => d.CarbonCredit).WithMany(p => p.CarbonListings)
                .HasForeignKey(d => d.CarbonCreditId)
                .HasConstraintName("FK_CarbonListing_CarbonCreditId");
        });

        modelBuilder.Entity<Certificate>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Certific__3214EC07E38F5457");

            entity.ToTable("Certificate");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.CreateAt).HasColumnType("datetime");
            entity.Property(e => e.DeleteAt).HasColumnType("datetime");
            entity.Property(e => e.ImgUrl).HasColumnName("imgUrl");
            entity.Property(e => e.IssuedAt).HasColumnType("datetime");
            entity.Property(e => e.Status).HasMaxLength(50);
            entity.Property(e => e.UpdateAt).HasColumnType("datetime");

            entity.HasOne(d => d.Buyer).WithMany(p => p.CertificateBuyers)
                .HasForeignKey(d => d.BuyerId)
                .HasConstraintName("FK_Certificate_BuyerId");

            entity.HasOne(d => d.CarbonCredit).WithMany(p => p.Certificates)
                .HasForeignKey(d => d.CarbonCreditId)
                .HasConstraintName("FK_Certificate_CarbonCreditId");

            entity.HasOne(d => d.IssuedBy).WithMany(p => p.CertificateIssuedBies)
                .HasForeignKey(d => d.IssuedById)
                .HasConstraintName("FK_Certificate_IssuedBy");
        });

        modelBuilder.Entity<Deposit>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Deposit__3214EC070ACED230");

            entity.ToTable("Deposit");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Amount).HasColumnType("decimal(15, 2)");
            entity.Property(e => e.Code)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.CreateAt).HasColumnType("datetime");
            entity.Property(e => e.DeleteAt).HasColumnType("datetime");
            entity.Property(e => e.UpdateAt).HasColumnType("datetime");

            entity.HasOne(d => d.Account).WithMany(p => p.Deposits)
                .HasForeignKey(d => d.AccountId)
                .HasConstraintName("FK_Deposit_AccountId");
        });

        modelBuilder.Entity<Dispute>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Dispute__3214EC07A8E35B5E");

            entity.ToTable("Dispute");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.CreateAt).HasColumnType("datetime");
            entity.Property(e => e.DeleteAt).HasColumnType("datetime");
            entity.Property(e => e.Status).HasMaxLength(50);
            entity.Property(e => e.Type).HasMaxLength(50);
            entity.Property(e => e.UpdateAt).HasColumnType("datetime");

            entity.HasOne(d => d.SendAccount).WithMany(p => p.Disputes)
                .HasForeignKey(d => d.SendAccountId)
                .HasConstraintName("FK_Dispute_SendAccount");

            entity.HasOne(d => d.Transaction).WithMany(p => p.Disputes)
                .HasForeignKey(d => d.TransactionId)
                .HasConstraintName("FK_Dispute_TransactionId");
        });

        modelBuilder.Entity<ElectricVehicle>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Electric__3214EC07DDC5DE9E");

            entity.ToTable("ElectricVehicle");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.BatteryCapacity).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.Brand).HasMaxLength(150);
            entity.Property(e => e.CreateAt).HasColumnType("datetime");
            entity.Property(e => e.DeleteAt).HasColumnType("datetime");
            entity.Property(e => e.LicensePlate).HasMaxLength(150);
            entity.Property(e => e.UpdateAt).HasColumnType("datetime");
            entity.Property(e => e.VehicleModel).HasMaxLength(150);
            entity.Property(e => e.Vin)
                .HasMaxLength(150)
                .HasColumnName("VIN");

            entity.HasOne(d => d.Account).WithMany(p => p.ElectricVehicles)
                .HasForeignKey(d => d.AccountId)
                .HasConstraintName("FK_ElectricVehicle_AccountId");

            entity.HasOne(d => d.VehicleType).WithMany(p => p.ElectricVehicles)
                .HasForeignKey(d => d.VehicleTypeId)
                .HasConstraintName("FK_ElectricVehicle_VehicleTypeId");
        });

        modelBuilder.Entity<Transaction>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Transact__3214EC07388ECB14");

            entity.ToTable("Transaction");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Amount).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.CreateAt).HasColumnType("datetime");
            entity.Property(e => e.DeleteAt).HasColumnType("datetime");
            entity.Property(e => e.Status).HasMaxLength(50);
            entity.Property(e => e.Type).HasMaxLength(50);
            entity.Property(e => e.UpdateAt).HasColumnType("datetime");

            entity.HasOne(d => d.Buyer).WithMany(p => p.TransactionBuyers)
                .HasForeignKey(d => d.BuyerId)
                .HasConstraintName("FK_Transaction_BuyerId");

            entity.HasOne(d => d.CarbonListing).WithMany(p => p.Transactions)
                .HasForeignKey(d => d.CarbonListingId)
                .HasConstraintName("FK_Transaction_CarbonListingId");

            entity.HasOne(d => d.Deposit).WithMany(p => p.Transactions)
                .HasForeignKey(d => d.DepositId)
                .HasConstraintName("FK_Transaction_DepositId");

            entity.HasOne(d => d.Seller).WithMany(p => p.TransactionSellers)
                .HasForeignKey(d => d.SellerId)
                .HasConstraintName("FK_Transaction_SellerId");

            entity.HasOne(d => d.Wallet).WithMany(p => p.Transactions)
                .HasForeignKey(d => d.WalletId)
                .HasConstraintName("FK_Transaction_WalletId");
        });

        modelBuilder.Entity<VehicleTelemetry>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__VehicleT__3214EC07613CF8FA");

            entity.ToTable("VehicleTelemetry");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.BatteryLevel).HasColumnType("decimal(5, 2)");
            entity.Property(e => e.CreateAt).HasColumnType("datetime");
            entity.Property(e => e.DeleteAt).HasColumnType("datetime");
            entity.Property(e => e.DistanceTravelled).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.EnergyConsumed).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.LoggedAt).HasColumnType("datetime");
            entity.Property(e => e.UpdateAt).HasColumnType("datetime");

            entity.HasOne(d => d.CarbonEmission).WithMany(p => p.VehicleTelemetries)
                .HasForeignKey(d => d.CarbonEmissionId)
                .HasConstraintName("FK_VehicleTelemetry_CarbonEmission");

            entity.HasOne(d => d.ElectricVehicle).WithMany(p => p.VehicleTelemetries)
                .HasForeignKey(d => d.ElectricVehicleId)
                .HasConstraintName("FK_VehicleTelemetry_ElectricVehicleId");
        });

        modelBuilder.Entity<VehicleType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__VehicleT__3214EC07F594EF08");

            entity.ToTable("VehicleType");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.CreateAt).HasColumnType("datetime");
            entity.Property(e => e.DeleteAt).HasColumnType("datetime");
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.UpdateAt).HasColumnType("datetime");
        });

        modelBuilder.Entity<Wallet>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Wallet__3214EC07E4BEBBB5");

            entity.ToTable("Wallet");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.CarbonUnit).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.Cash).HasColumnType("decimal(15, 2)");
            entity.Property(e => e.CreateAt).HasColumnType("datetime");
            entity.Property(e => e.DeleteAt).HasColumnType("datetime");
            entity.Property(e => e.UpdateAt).HasColumnType("datetime");

            entity.HasOne(d => d.Account).WithMany(p => p.Wallets)
                .HasForeignKey(d => d.AccountId)
                .HasConstraintName("FK_Wallet_AccountId");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
