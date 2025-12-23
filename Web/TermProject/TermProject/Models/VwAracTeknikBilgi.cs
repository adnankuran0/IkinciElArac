using System;
using System.Collections.Generic;

namespace TermProject.Models;

public partial class VwAracTeknikBilgi
{
    public int AracId { get; set; }

    public int? Yil { get; set; }

    public int? MotorHacmi { get; set; }

    public int? MotorGucu { get; set; }

    public string? YakitTipi { get; set; }

    public string? VitesTipi { get; set; }
}
