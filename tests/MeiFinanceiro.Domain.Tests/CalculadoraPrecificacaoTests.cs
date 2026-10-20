using MeiFinanceiro.Domain.Calculadoras;
using Xunit;

namespace MeiFinanceiro.Domain.Tests;

public class CalculadoraPrecificacaoTests
{
    // Mesmo cenário testado manualmente no navegador: carne R$10 com 20% de perda (custo real
    // R$12,50) + tempero R$2 + carvão R$1 + embalagem R$1 = R$16,50 de custo real; cartão 3% +
    // delivery 20%; margem de lucro 25% => preço R$31,73 (arredondado R$32,00).
    [Fact]
    public void Calcular_CenarioComPerdaDeInsumoValidadoNoNavegador_RetornaValoresConferidos()
    {
        var entrada = new PrecificacaoEntrada(
            Insumos: new List<ItemInsumo>
            {
                new("Carne", 10m, 20m),
                new("Tempero / acompanhamentos", 2m, 0m),
                new("Carvão / lenha", 1m, 0m),
                new("Embalagem", 1m, 0m),
            },
            CustosVariaveis: new List<ItemPercentual>
            {
                new("Taxa de cartão / maquininha", 3m),
                new("Comissão de app de delivery", 20m),
            },
            MargemLucroPercentual: 25m);

        var resultado = CalculadoraPrecificacao.Calcular(entrada);

        Assert.True(resultado.Sucesso);
        Assert.Equal(14.00m, resultado.CustoBrutoTotal, 2);
        Assert.Equal(16.50m, resultado.CustoRealTotal, 2);
        Assert.Equal(31.73m, Math.Round(resultado.PrecoVenda, 2));
        Assert.Equal(32.00m, resultado.PrecoArredondado, 2);
        Assert.Equal(7.30m, Math.Round(resultado.ValorVariaveis, 2));
        Assert.Equal(7.93m, Math.Round(resultado.ValorLucro, 2));
    }

    [Fact]
    public void Calcular_SemInsumos_RetornaErro()
    {
        var entrada = new PrecificacaoEntrada(Array.Empty<ItemInsumo>(), Array.Empty<ItemPercentual>(), 30m);

        var resultado = CalculadoraPrecificacao.Calcular(entrada);

        Assert.False(resultado.Sucesso);
    }

    [Fact]
    public void Calcular_MargemMaisVariaveisUltrapassa100Porcento_RetornaErro()
    {
        var entrada = new PrecificacaoEntrada(
            new List<ItemInsumo> { new("Carne", 10m, 0m) },
            new List<ItemPercentual> { new("Delivery", 60m) },
            MargemLucroPercentual: 50m);

        var resultado = CalculadoraPrecificacao.Calcular(entrada);

        Assert.False(resultado.Sucesso);
    }
}
