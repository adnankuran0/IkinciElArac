using System;
using System.Collections.Generic;

namespace TermProject.Models;

public partial class TahminLog
{
    public int LogId { get; set; }

    public int TahminId { get; set; }

    public int MlModelId { get; set; }

    public int? CalismaSuresiMs { get; set; }

    public virtual MlModel MlModel { get; set; } = null!;

    public virtual FiyatTahmin Tahmin { get; set; } = null!;
}
