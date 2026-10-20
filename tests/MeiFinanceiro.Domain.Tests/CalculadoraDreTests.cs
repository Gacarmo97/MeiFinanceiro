using MeiFinanceiro.Domain.Calculadoras;
using MeiFinanceiro.Domain.Enums;
using Xunit;

namespace MeiFinanceiro.Domain.Tests;

public class CalculadoraDreTests
{
    // Mesmo cenário testado manualmente no navegador: faturamento 18000, CMV 6800,
    // custos fixos 3350, DAS 76,90 => lucro líquido R$7.773,10 (43,18%).
    [Fact]
    public void Calcular_CenarioValidadoNoNavegador_RetornaLucroLiquidoConferido()
    {
        var entrada = new DreEntrada(
            Atividade: AtividadeMei.Comercio,
            SalarioMinimo: 1518m,
            Faturamento: 18000m,
            Cmv: 6800m,
            TotalCustosFixos: 3350m);

        var resultado = CalculadoraDre.Calcular(entrada);

        Assert.True(resultado.Sucesso);
        Assert.Equal(76.90m, resultado.DasTotal, 2);
        Assert.Equal(11200.00m, resultado.LucroBruto, 2);
        Assert.Equal(7773.10m, resultado.LucroLiquido, 2);
        Assert.Equal(43.18m, Math.Round(resultado.MargemLiquidaPercentual, 2));
    }

    [Fact]
    public void Calcular_CustosMaioresQueFaturamento_RetornaLucroLiquidoNegativo()
    {
        var entrada = new DreEntrada(AtividadeMei.Comercio, 1518m, 1000m, 500m, 1000m);

        var resultado = CalculadoraDre.Calcular(entrada);

        Assert.True(resultado.Sucesso);
        Assert.True(resultado.LucroLiquido < 0);
    }

    [Fact]
    public void Calcular_SemFaturamento_RetornaErro()
    {
        var entrada = new DreEntrada(AtividadeMei.Comercio, 1518m, 0m, 0m, 0m);

        var resultado = CalculadoraDre.Calcular(entrada);

        Assert.False(resultado.Sucesso);
    }
}
