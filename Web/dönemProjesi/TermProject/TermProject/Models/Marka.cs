using System;
using System.Collections.Generic;

namespace TermProject.Models;

public partial class Marka
{
    public int MarkaId { get; set; }

    public string MarkaAdi { get; set; } = null!;

    public virtual ICollection<Seri> Seris { get; set; } = new List<Seri>();
}
