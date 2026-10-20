namespace MeiFinanceiro.Domain.Calculadoras;

public record ItemInsumo(string Descricao, decimal CustoBruto, decimal PerdaPercentual)
{
    public decimal CustoAjustado()
    {
        var perda = Math.Min(Math.Max(PerdaPercentual, 0), 95) / 100m;
        return CustoBruto / (1 - perda);
    }
}

public record PrecificacaoEntrada(
    IReadOnlyList<ItemInsumo> Insumos,
    IReadOnlyList<ItemPercentual> CustosVariaveis,
    decimal MargemLucroPercentual);

public record PrecificacaoResultado(
    bool Sucesso,
    string? MensagemErro,
    decimal CustoBrutoTotal,
    decimal CustoRealTotal,
    decimal TotalPercentVariavel,
    decimal Divisor,
    decimal PrecoVenda,
    decimal PrecoArredondado,
    decimal ValorVariaveis,
    decimal ValorLucro)
{
    public static PrecificacaoResultado Erro(string mensagem) =>
        new(false, mensagem, 0, 0, 0, 0, 0, 0, 0, 0);
}

/// <summary>
/// Método do divisor de markup: o preço é ajustado para que, depois de pagar insumos
/// (já corrigidos pela % de perda/quebra) e taxas variáveis, a margem de lucro informada
/// corresponda de fato ao % do preço final — não do custo.
/// </summary>
public static class CalculadoraPrecificacao
{
    public static PrecificacaoResultado Calcular(PrecificacaoEntrada entrada)
    {
        var custoBrutoTotal = entrada.Insumos.Sum(i => i.CustoBruto);
        var custoRealTotal = entrada.Insumos.Sum(i => i.CustoAjustado());

        if (custoRealTotal <= 0)
            return PrecificacaoResultado.Erro("Informe o custo de ao menos um insumo para calcular o preço de venda.");

        var totalPercentVariavel = entrada.CustosVariaveis.Sum(c => c.Percentual);
        var divisor = 1 - (totalPercentVariavel + entrada.MargemLucroPercentual) / 100m;

        if (divisor <= 0)
            return PrecificacaoResultado.Erro(
                "A soma dos custos variáveis (%) com a margem de lucro desejada é igual ou maior que 100% do preço de venda. Reduza algum dos dois valores.");

        var precoVenda = custoRealTotal / divisor;
        var valorVariaveis = precoVenda * (totalPercentVariavel / 100m);
        var valorLucro = precoVenda * (entrada.MargemLucroPercentual / 100m);
        var precoArredondado = Math.Ceiling(precoVenda * 2) / 2;

        return new PrecificacaoResultado(
            true, null,
            custoBrutoTotal, custoRealTotal,
            totalPercentVariavel, divisor,
            precoVenda, precoArredondado,
            valorVariaveis, valorLucro);
    }
}
