using System;
using System.Collections.Generic;

namespace TermProject.Models;

public partial class Ilan
{
    public int IlanId { get; set; }

    public int AracId { get; set; }

    public int KullaniciId { get; set; }

    public decimal? Fiyat { get; set; }

    public DateOnly? IlanTarihi { get; set; }

    public bool? TakasaUygun { get; set; }

    public string? Kimden { get; set; }

    public virtual Arac Arac { get; set; } = null!;

    public virtual Kullanicilar Kullanici { get; set; } = null!;
}
