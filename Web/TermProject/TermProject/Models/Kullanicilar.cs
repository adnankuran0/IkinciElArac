using System;
using System.Collections.Generic;

namespace TermProject.Models;

public partial class Kullanicilar
{
    public int KullaniciId { get; set; }

    public string Ad { get; set; } = null!;

    public string Soyad { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string SifreHash { get; set; } = null!;

    public string? Telefon { get; set; }

    public int RolId { get; set; }

    public DateTime? KayitTarihi { get; set; }

    public virtual ICollection<FiyatTahmin> FiyatTahmins { get; set; } = new List<FiyatTahmin>();

    public virtual ICollection<Ilan> Ilans { get; set; } = new List<Ilan>();

    public virtual ICollection<KullaniciIslemLog> KullaniciIslemLogs { get; set; } = new List<KullaniciIslemLog>();

    public virtual Rol Rol { get; set; } = null!;
}
