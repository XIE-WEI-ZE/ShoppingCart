using System;
using System.Collections.Generic;

namespace prjECommerceDemo.Models;

public partial class TCustomer
{
    public int FId { get; set; }

    public string? FName { get; set; }

    public string? FPhone { get; set; }

    public string? FAddress { get; set; }

    public string? FEmail { get; set; }

    public string? FPassword { get; set; }

    public string? FSalt { get; set; }

    public string? FPhoto { get; set; }

    public string? FExternalId { get; set; }

    public string FAccount { get; set; } = null!;

    public string? FLoginProvider { get; set; }

    public DateTime? FCreatedDate { get; set; }

    public bool? FIsEnabled { get; set; }

    public string? FCity { get; set; }

    public string? FDistrict { get; set; }

    public string? FRoadAddress { get; set; }

    public string? FGender { get; set; }

    public DateOnly? FBirthday { get; set; }

    public bool? IsTempPassword { get; set; }
}
