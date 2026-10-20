using MeiFinanceiro.Domain.Enums;

namespace MeiFinanceiro.Domain.Calculadoras;

public record OrcamentoEntrada(
    decimal Faturamento,
    decimal MargemLucroPercentual,
    IReadOnlyList<ItemPercentual> CustosVariaveis,
    AtividadeMei Atividade,
    decimal SalarioMinimo);

public record OrcamentoResultado(
    bool Sucesso,
    string? MensagemErro,
    string? MensagemAlerta,
    decimal TotalPercentVariavel,
    decimal DasTotal,
    decimal LucroEsperado,
    decimal GastoVariavelReais,
    decimal GastoMaximoTotal,
    decimal TetoCustosFixos)
{
    public static OrcamentoResultado Erro(string mensagem) =>
        new(false, mensagem, null, 0, 0, 0, 0, 0, 0);
}

public static class CalculadoraOrcamento
{
    public static OrcamentoResultado Calcular(OrcamentoEntrada entrada)
    {
        if (entrada.Faturamento <= 0)
            return OrcamentoResultado.Erro("Informe o faturamento esperado no mês para calcular o orçamento máximo.");

        var totalPercentVariavel = entrada.CustosVariaveis.Sum(c => c.Percentual);

        if (entrada.MargemLucroPercentual + totalPercentVariavel >= 100)
            return OrcamentoResultado.Erro(
                "A margem de lucro desejada somada aos custos variáveis (%) já ultrapassa 100% do faturamento. Reduza algum dos dois valores.");

        var das = CalculadoraDasMei.Calcular(entrada.SalarioMinimo, entrada.Atividade);
        var lucroEsperado = entrada.Faturamento * (entrada.MargemLucroPercentual / 100m);
        var gastoVariavelReais = entrada.Faturamento * (totalPercentVariavel / 100m);
        var gastoMaximoTotal = entrada.Faturamento - lucroEsperado;
        var tetoCustosFixos = gastoMaximoTotal - gastoVariavelReais - das.Total;

        string? alerta = tetoCustosFixos < 0
            ? $"Atenção: os custos variáveis ({gastoVariavelReais:C}) e o DAS-MEI ({das.Total:C}) já consomem mais do que o orçamento disponível ({gastoMaximoTotal:C}) para o lucro desejado. Não sobra nada para custos fixos — revise a margem, os custos variáveis ou o faturamento esperado."
            : null;

        return new OrcamentoResultado(
            true, null, alerta,
            totalPercentVariavel, das.Total,
            lucroEsperado, gastoVariavelReais,
            gastoMaximoTotal, tetoCustosFixos);
    }
}
