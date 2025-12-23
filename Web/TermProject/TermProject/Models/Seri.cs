using System;
using System.Collections.Generic;

namespace TermProject.Models;

public partial class Seri
{
    public int SeriId { get; set; }

    public int MarkaId { get; set; }

    public string SeriAdi { get; set; } = null!;

    public virtual Marka Marka { get; set; } = null!;

    public virtual ICollection<Model> Models { get; set; } = new List<Model>();
}
