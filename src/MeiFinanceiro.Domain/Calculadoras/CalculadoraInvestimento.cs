namespace MeiFinanceiro.Domain.Calculadoras;

public record InvestimentoEntrada(
    IReadOnlyList<ItemValor> Investimentos,
    decimal LucroMensal);

public record InvestimentoResultado(
    bool Sucesso,
    string? MensagemErro,
    string? MensagemAlerta,
    decimal InvestimentoTotal,
    decimal PaybackMeses,
    int PaybackAnos,
    int PaybackRestoMeses,
    decimal RoiMensalPercentual,
    decimal RoiAnualPercentual)
{
    public static InvestimentoResultado Erro(string mensagem) =>
        new(false, mensagem, null, 0, 0, 0, 0, 0, 0);
}

/// <summary>
/// Payback simples (investimento ÷ lucro mensal). Não considera juros, inflação ou
/// variação de vendas mês a mês — é uma estimativa rápida, não um fluxo de caixa detalhado.
/// </summary>
public static class CalculadoraInvestimento
{
    public static InvestimentoResultado Calcular(InvestimentoEntrada entrada)
    {
        var investimentoTotal = entrada.Investimentos.Sum(i => i.Valor);

        if (investimentoTotal <= 0)
            return InvestimentoResultado.Erro("Informe ao menos um item de investimento inicial.");

        if (entrada.LucroMensal <= 0)
            return InvestimentoResultado.Erro("Informe um lucro líquido mensal esperado maior que zero.");

        var paybackMeses = investimentoTotal / entrada.LucroMensal;
        var paybackAnos = (int)Math.Floor(paybackMeses / 12);
        var paybackRestoMeses = (int)Math.Round(paybackMeses % 12);
        var roiMensal = (entrada.LucroMensal / investimentoTotal) * 100m;
        var roiAnual = roiMensal * 12;

        string? alerta = paybackMeses > 60
            ? "O retorno do investimento está acima de 5 anos. Vale revisar o investimento inicial, o lucro mensal projetado, ou buscar formas de reduzir custos fixos."
            : null;

        return new InvestimentoResultado(
            true, null, alerta,
            investimentoTotal, paybackMeses,
            paybackAnos, paybackRestoMeses,
            roiMensal, roiAnual);
    }
}
