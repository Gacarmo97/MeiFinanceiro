using MeiFinanceiro.Domain.Calculadoras;
using Xunit;

namespace MeiFinanceiro.Domain.Tests;

public class CalculadoraCmvTests
{
    // Mesmo cenário testado manualmente no navegador: estoque inicial 1200, compras 6500,
    // estoque final 900, faturamento 18000 => CMV R$6.800,00 (37,8%), faixa de Alerta.
    [Fact]
    public void Calcular_CenarioValidadoNoNavegador_RetornaCmvEFaixaConferidos()
    {
        var entrada = new CmvEntrada(EstoqueInicial: 1200m, Compras: 6500m, EstoqueFinal: 900m, Faturamento: 18000m);

        var resultado = CalculadoraCmv.Calcular(entrada);

        Assert.True(resultado.Sucesso);
        Assert.Equal(6800.00m, resultado.CmvReais, 2);
        Assert.Equal(37.8m, Math.Round(resultado.CmvPercentual, 1));
        Assert.Equal(FaixaCmv.Alerta, resultado.Faixa);
    }

    [Theory]
    [InlineData(2400, FaixaCmv.Saudavel)]
    [InlineData(3200, FaixaCmv.Atencao)]
    [InlineData(4000, FaixaCmv.Alerta)]
    public void Calcular_FaixasDeReferencia_ClassificamCorretamente(decimal cmvReais, FaixaCmv faixaEsperada)
    {
        var entrada = new CmvEntrada(EstoqueInicial: 0m, Compras: cmvReais, EstoqueFinal: 0m, Faturamento: 10000m);

        var resultado = CalculadoraCmv.Calcular(entrada);

        Assert.Equal(faixaEsperada, resultado.Faixa);
    }

    [Fact]
    public void Calcular_EstoqueFinalMaiorQueInicialMaisCompras_RetornaErro()
    {
        var entrada = new CmvEntrada(EstoqueInicial: 100m, Compras: 100m, EstoqueFinal: 500m, Faturamento: 10000m);

        var resultado = CalculadoraCmv.Calcular(entrada);

        Assert.False(resultado.Sucesso);
    }
}
