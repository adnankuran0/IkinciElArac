using System;
using System.Collections.Generic;

namespace TermProject.Models;

public partial class VwKullaniciTahminleri
{
    public string? AdSoyad { get; set; }

    public decimal? TahminEdilenFiyat { get; set; }

    public DateTime? TahminTarihi { get; set; }
}
