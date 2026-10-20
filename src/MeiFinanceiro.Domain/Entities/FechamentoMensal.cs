namespace MeiFinanceiro.Domain.Entities;

/// <summary>
/// Fechamento real de um mês para um estabelecimento. LucroLiquido e MargemLiquida ficam
/// gravados (não recalculados na leitura) para o histórico não mudar se a fórmula evoluir.
/// </summary>
public class FechamentoMensal
{
    public int Id { get; set; }
    public int EstabelecimentoId { get; set; }
    public Estabelecimento? Estabelecimento { get; set; }

    public int Ano { get; set; }
    public int Mes { get; set; }

    public decimal Faturamento { get; set; }
    public decimal Cmv { get; set; }
    public decimal CustosFixos { get; set; }
    public decimal DasCalculado { get; set; }

    public decimal LucroLiquido { get; set; }
    public decimal MargemLiquida { get; set; }
}
