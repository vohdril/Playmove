using System;
using System.Collections.Generic;

namespace Playmove.DAO.Models;

public partial class Usuario
{
    public int Id { get; set; }

    public string? Nome { get; set; }

    public string? Sobrenome { get; set; }
}
