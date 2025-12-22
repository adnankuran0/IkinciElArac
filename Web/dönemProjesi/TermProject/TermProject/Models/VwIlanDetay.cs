using System;
using System.Collections.Generic;

namespace TermProject.Models;

public partial class VwIlanDetay
{
    public int IlanId { get; set; }

    public string MarkaAdi { get; set; } = null!;

    public string SeriAdi { get; set; } = null!;

    public string ModelAdi { get; set; } = null!;

    public int? Yil { get; set; }

    public int? Kilometre { get; set; }

    public decimal? Fiyat { get; set; }
}
