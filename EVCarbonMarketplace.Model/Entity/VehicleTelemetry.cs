using System;
using System.Collections.Generic;

namespace EVCarbonMarketplace.Model.Entity;

public partial class VehicleTelemetry
{
    public Guid Id { get; set; }

    public Guid? ElectricVehicleId { get; set; }

    public DateTime? LoggedAt { get; set; }

    public int? Odometer { get; set; }

    public decimal? DistanceTravelled { get; set; }

    public decimal? EnergyConsumed { get; set; }

    public decimal? BatteryLevel { get; set; }

    public bool? IsActive { get; set; }

    public DateTime? CreateAt { get; set; }

    public DateTime? UpdateAt { get; set; }

    public DateTime? DeleteAt { get; set; }

    public virtual ICollection<CarbonEmission> CarbonEmissions { get; set; } = new List<CarbonEmission>();

    public virtual ElectricVehicle? ElectricVehicle { get; set; }
}
