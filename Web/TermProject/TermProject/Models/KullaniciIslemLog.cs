using System;
using System.Collections.Generic;

namespace TermProject.Models;

public partial class KullaniciIslemLog
{
    public int IslemId { get; set; }

    public int KullaniciId { get; set; }

    public string IslemTipi { get; set; } = null!;

    public string? IslemDetayi { get; set; }

    public string? IpAdresi { get; set; }

    public DateTime? IslemTarihi { get; set; }

    public virtual Kullanicilar Kullanici { get; set; } = null!;
}
