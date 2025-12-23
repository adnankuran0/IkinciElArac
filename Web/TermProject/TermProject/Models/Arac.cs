using System;
using System.Collections.Generic;

namespace TermProject.Models;

public partial class Arac
{
    public int AracId { get; set; }

    public int ModelId { get; set; }

    public int? Yil { get; set; }

    public int? Kilometre { get; set; }

    public string? VitesTipi { get; set; }

    public string? YakitTipi { get; set; }

    public string? KasaTipi { get; set; }

    public string? Renk { get; set; }

    public int? MotorHacmi { get; set; }

    public int? MotorGucu { get; set; }

    public string? CekisTipi { get; set; }

    public string? AracDurumu { get; set; }

    public decimal? OrtYakitTuketimi { get; set; }

    public int? YakitDeposu { get; set; }

    public string? BoyaDegisen { get; set; }

    public virtual ICollection<FiyatTahmin> FiyatTahmins { get; set; } = new List<FiyatTahmin>();

    public virtual ICollection<Ilan> Ilans { get; set; } = new List<Ilan>();

    public virtual Model Model { get; set; } = null!;
}
