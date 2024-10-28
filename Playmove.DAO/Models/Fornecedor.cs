using System;
using System.Collections.Generic;

namespace Playmove.DAO.Models;

public partial class Fornecedor
{
    public int Id { get; set; }

    public string? Nome { get; set; }

    public string? Email { get; set; }

    public string Cnpj { get; set; } = null!;

    public string? Telefone { get; set; }

    public string Endereco { get; set; } = null!;

    public string Cidade { get; set; } = null!;

    public string Estado { get; set; } = null!;

    public string Cep { get; set; } = null!;

    public DateOnly DataCadastro { get; set; }

    public bool Status { get; set; }
}
