using MeiFinanceiro.Domain.Calculadoras;
using MeiFinanceiro.Domain.Enums;
using Xunit;

namespace MeiFinanceiro.Domain.Tests;

public class CalculadoraOrcamentoTests
{
    // Mesmo cenário testado manualmente no navegador: faturamento R$12.000, lucro 15%,
    // variáveis 13% (cartão 3% + delivery 10%), DAS R$76,90 => teto de custos fixos R$8.563,10.
    [Fact]
    public void Calcular_CenarioValidadoNoNavegador_RetornaTetoDeCustosFixosConferido()
    {
        var entrada = new OrcamentoEntrada(
            Faturamento: 12000m,
            MargemLucroPercentual: 15m,
            CustosVariaveis: new List<ItemPercentual>
            {
                new("Taxa de cartão / maquininha", 3m),
                new("Comissão de app de delivery", 10m),
            },
            Atividade: AtividadeMei.Comercio,
            SalarioMinimo: 1518m);

        var resultado = CalculadoraOrcamento.Calcular(entrada);

        Assert.True(resultado.Sucesso);
        Assert.Equal(76.90m, resultado.DasTotal, 2);
        Assert.Equal(1800.00m, resultado.LucroEsperado, 2);
        Assert.Equal(1560.00m, resultado.GastoVariavelReais, 2);
        Assert.Equal(10200.00m, resultado.GastoMaximoTotal, 2);
        Assert.Equal(8563.10m, resultado.TetoCustosFixos, 2);
        Assert.Null(resultado.MensagemAlerta);
    }

    [Fact]
    public void Calcular_MargemMaisVariaveisUltrapassa100Porcento_RetornaErro()
    {
        var entrada = new OrcamentoEntrada(
            3000m, 20m,
            new List<ItemPercentual> { new("A", 50m), new("B", 30m) },
            AtividadeMei.Comercio, 1518m);

        var resultado = CalculadoraOrcamento.Calcular(entrada);

        Assert.False(resultado.Sucesso);
    }

    [Fact]
    public void Calcular_SemFaturamento_RetornaErro()
    {
        var entrada = new OrcamentoEntrada(0m, 10m, Array.Empty<ItemPercentual>(), AtividadeMei.Comercio, 1518m);

        var resultado = CalculadoraOrcamento.Calcular(entrada);

        Assert.False(resultado.Sucesso);
    }
}
