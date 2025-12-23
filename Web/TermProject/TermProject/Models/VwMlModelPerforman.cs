using System;
using System.Collections.Generic;

namespace TermProject.Models;

public partial class VwMlModelPerforman
{
    public string ModelAdi { get; set; } = null!;

    public string? Versiyon { get; set; }

    public decimal? DogrulukOrani { get; set; }
}
