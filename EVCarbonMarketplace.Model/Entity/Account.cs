using System;
using System.Collections.Generic;

namespace EVCarbonMarketplace.Model.Entity;

public partial class Account
{
    public Guid Id { get; set; }

    public string? Role { get; set; }

    public string? FullName { get; set; }

    public string? Email { get; set; }

    public string? Phone { get; set; }

    public DateOnly? DateOfBirth { get; set; }

    public string? Gender { get; set; }

    public string? AvatarUrl { get; set; }

    public bool? IsActive { get; set; }

    public DateTime? CreateAt { get; set; }

    public DateTime? UpdateAt { get; set; }

    public DateTime? DeleteAt { get; set; }

    public string? Username { get; set; }

    public string? Password { get; set; }
}
