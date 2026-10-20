using MeiFinanceiro.Domain.Enums;

namespace MeiFinanceiro.Domain.Calculadoras;

public record PontoEquilibrioEntrada(
    AtividadeMei Atividade,
    decimal SalarioMinimo,
    IReadOnlyList<ItemValor> CustosFixos,
    decimal PrecoVenda,
    decimal CustoInsumo,
    IReadOnlyList<ItemPercentual> CustosVariaveis,
    int DiasVenda);

public record PontoEquilibrioResultado(
    bool Sucesso,
    string? MensagemErro,
    string? MensagemAlerta,
    decimal DasTotal,
    decimal TotalCustosFixos,
    decimal MargemContribuicaoUnit,
    decimal MargemContribuicaoPercent,
    decimal PontoEquilibrioReais,
    decimal PontoEquilibrioUnidades,
    decimal PontoEquilibrioDiarioReais,
    decimal PontoEquilibrioDiarioUnidades,
    decimal FaturamentoAnualProjetado)
{
    public static PontoEquilibrioResultado Erro(string mensagem) =>
        new(false, mensagem, null, 0, 0, 0, 0, 0, 0, 0, 0, 0);
}

public static class CalculadoraPontoEquilibrio
{
    public const decimal LimiteAnualMei = 81000m;

    public static PontoEquilibrioResultado Calcular(PontoEquilibrioEntrada entrada)
    {
        if (entrada.PrecoVenda <= 0)
            return PontoEquilibrioResultado.Erro("Informe o preço médio de venda para calcular o ponto de equilíbrio.");

        var das = CalculadoraDasMei.Calcular(entrada.SalarioMinimo, entrada.Atividade);
        var totalFixosUsuario = entrada.CustosFixos.Sum(c => c.Valor);
        var totalCustosFixos = totalFixosUsuario + das.Total;

        var totalPercentVariavel = entrada.CustosVariaveis.Sum(c => c.Percentual);
        var custoVariavelUnitario = entrada.CustoInsumo + entrada.PrecoVenda * (totalPercentVariavel / 100m);
        var margemContribuicaoUnit = entrada.PrecoVenda - custoVariavelUnitario;
        var margemContribuicaoPercent = margemContribuicaoUnit / entrada.PrecoVenda;

        if (margemContribuicaoPercent <= 0)
            return PontoEquilibrioResultado.Erro(
                "O preço de venda não cobre os custos variáveis (insumo + percentuais). Revise os valores — a margem de contribuição está zerada ou negativa.");

        var dias = entrada.DiasVenda <= 0 ? 26 : entrada.DiasVenda;
        var pontoEquilibrioReais = totalCustosFixos / margemContribuicaoPercent;
        var pontoEquilibrioUnidades = pontoEquilibrioReais / entrada.PrecoVenda;
        var pontoEquilibrioDiarioReais = pontoEquilibrioReais / dias;
        var pontoEquilibrioDiarioUnidades = pontoEquilibrioUnidades / dias;
        var faturamentoAnualProjetado = pontoEquilibrioReais * 12;

        string? alerta = faturamentoAnualProjetado > LimiteAnualMei
            ? $"Atenção: o ponto de equilíbrio mensal, projetado para 12 meses ({faturamentoAnualProjetado:C}), ultrapassa o limite anual de faturamento do MEI ({LimiteAnualMei:C}). Reveja os custos fixos ou considere migrar para ME."
            : null;

        return new PontoEquilibrioResultado(
            true, null, alerta,
            das.Total, totalCustosFixos,
            margemContribuicaoUnit, margemContribuicaoPercent,
            pontoEquilibrioReais, pontoEquilibrioUnidades,
            pontoEquilibrioDiarioReais, pontoEquilibrioDiarioUnidades,
            faturamentoAnualProjetado);
    }
}
