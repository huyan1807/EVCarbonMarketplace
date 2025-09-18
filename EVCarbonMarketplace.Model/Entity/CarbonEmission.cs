using System;
using System.Collections.Generic;

namespace EVCarbonMarketplace.Model.Entity;

public partial class CarbonEmission
{
    public Guid Id { get; set; }

    public Guid? ElectricVehicleId { get; set; }

    public decimal? DistanceTravelled { get; set; }

    public decimal? EnergyConsumed { get; set; }

    public decimal? Co2reduced { get; set; }

    public DateTime? PeriodStart { get; set; }

    public DateTime? PeriodEnd { get; set; }

    public bool? IsActive { get; set; }

    public DateTime? CreateAt { get; set; }

    public DateTime? UpdateAt { get; set; }

    public DateTime? DeleteAt { get; set; }

    public string? Status { get; set; }

    public virtual ICollection<CarbonCredit> CarbonCredits { get; set; } = new List<CarbonCredit>();

    public virtual ElectricVehicle? ElectricVehicle { get; set; }

    public virtual ICollection<VehicleTelemetry> VehicleTelemetries { get; set; } = new List<VehicleTelemetry>();
}
