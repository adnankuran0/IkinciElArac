using System;
using System.Collections.Generic;

namespace TermProject.Models;

public partial class Model
{
    public int ModelId { get; set; }

    public int SeriId { get; set; }

    public string ModelAdi { get; set; } = null!;

    public virtual ICollection<Arac> Aracs { get; set; } = new List<Arac>();

    public virtual Seri Seri { get; set; } = null!;
}
