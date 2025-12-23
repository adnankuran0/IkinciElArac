using System;
using System.Collections.Generic;

namespace TermProject.Models;

public partial class VwSonTahminler
{
    public int TahminId { get; set; }

    public decimal? TahminEdilenFiyat { get; set; }

    public DateTime? TahminTarihi { get; set; }
}
