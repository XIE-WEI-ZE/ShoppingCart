using System;
using System.Collections.Generic;

namespace prjECommerceDemo.Models;

public partial class TProductImage
{
    public int FId { get; set; }

    public int? FProductId { get; set; }

    public string? FImagePath { get; set; }

    public int? FSortOrder { get; set; }

    public DateTime? FUploadedDate { get; set; }
}
