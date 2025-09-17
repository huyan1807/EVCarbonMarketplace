using System;
using System.Collections.Generic;

namespace EVCarbonMarketplace.Model.Entity;

public partial class ElectricVehicle
{
    public Guid Id { get; set; }

    public Guid? AccountId { get; set; }

    public string? VehicleModel { get; set; }

    public string? Vin { get; set; }

    public decimal? BatteryCapacity { get; set; }

    public string? LicensePlate { get; set; }

    public string? Brand { get; set; }

    public Guid? VehicleTypeId { get; set; }

    public int? Odometer { get; set; }

    public bool? IsActive { get; set; }

    public DateTime? CreateAt { get; set; }

    public DateTime? UpdateAt { get; set; }

    public DateTime? DeleteAt { get; set; }

    public string? ImageUrl { get; set; }

    public virtual Account? Account { get; set; }

    public virtual ICollection<CarbonEmission> CarbonEmissions { get; set; } = new List<CarbonEmission>();

    public virtual ICollection<VehicleTelemetry> VehicleTelemetries { get; set; } = new List<VehicleTelemetry>();

    public virtual VehicleType? VehicleType { get; set; }
}
