using System;
using System.Collections.Generic;

namespace prjECommerceDemo.Models;

public partial class TProduct
{
    public int FId { get; set; }

    public string? FName { get; set; }

    public int? FQty { get; set; }

    public decimal? FCost { get; set; }

    public decimal? FPrice { get; set; }

    public string? FImagePath { get; set; }

    public int? FCategoryId { get; set; }

    public string? FDescription { get; set; }

    public bool? FIsAvailable { get; set; }

    public DateTime? FCreatedDate { get; set; }

    public DateTime? FUpdatedDate { get; set; }
}
