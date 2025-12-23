using System;
using System.Collections.Generic;

namespace TermProject.Models;

public partial class MlModel
{
    public int MlModelId { get; set; }

    public string ModelAdi { get; set; } = null!;

    public string? Versiyon { get; set; }

    public decimal? DogrulukOrani { get; set; }

    public DateTime? EklenmeTarihi { get; set; }

    public virtual ICollection<TahminLog> TahminLogs { get; set; } = new List<TahminLog>();
}
