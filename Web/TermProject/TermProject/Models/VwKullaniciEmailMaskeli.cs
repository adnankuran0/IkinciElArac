using System;
using System.Collections.Generic;

namespace TermProject.Models;

public partial class VwKullaniciEmailMaskeli
{
    public int KullaniciId { get; set; }

    public string? AdSoyad { get; set; }

    public string? Email { get; set; }
}
