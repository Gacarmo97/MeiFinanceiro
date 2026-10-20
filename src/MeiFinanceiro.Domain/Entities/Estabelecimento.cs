using MeiFinanceiro.Domain.Enums;

namespace MeiFinanceiro.Domain.Entities;

public class Estabelecimento
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public AtividadeMei Atividade { get; set; }
    public decimal SalarioMinimoReferencia { get; set; } = 1518m;

    public List<FechamentoMensal> Fechamentos { get; set; } = new();
}
