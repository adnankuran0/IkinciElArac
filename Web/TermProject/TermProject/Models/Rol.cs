using System;
using System.Collections.Generic;

namespace TermProject.Models;

public partial class Rol
{
    public int RolId { get; set; }

    public string RolAdi { get; set; } = null!;

    public virtual ICollection<Kullanicilar> Kullanicilars { get; set; } = new List<Kullanicilar>();
}
