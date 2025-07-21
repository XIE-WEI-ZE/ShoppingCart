using System;
using System.Collections.Generic;

namespace prjECommerceDemo.Models;

public partial class TProductCategory
{
    public int FId { get; set; }

    public string? FName { get; set; }

    public int? FSortOrder { get; set; }

    public DateTime? FCreatedDate { get; set; }
}
