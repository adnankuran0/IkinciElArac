using System;
using System.Collections.Generic;

namespace TermProject.Models;

public partial class VwKullaniciTelefonMaskeli
{
    public int KullaniciId { get; set; }

    public string? AdSoyad { get; set; }

    public string? Telefon { get; set; }
}
