using System;
using System.Collections.Generic;

namespace TermProject.Models;

public partial class FiyatTahmin
{
    public int TahminId { get; set; }

    public int KullaniciId { get; set; }

    public int AracId { get; set; }

    public decimal? TahminEdilenFiyat { get; set; }

    public DateTime? TahminTarihi { get; set; }

    public virtual Arac Arac { get; set; } = null!;

    public virtual Kullanicilar Kullanici { get; set; } = null!;

    public virtual ICollection<TahminLog> TahminLogs { get; set; } = new List<TahminLog>();
}
