using MeiFinanceiro.Domain.Calculadoras;
using MeiFinanceiro.Domain.Enums;
using Xunit;

namespace MeiFinanceiro.Domain.Tests;

public class CalculadoraPontoEquilibrioTests
{
    // Mesmo cenário testado manualmente no navegador na versão HTML:
    // aluguel 800, funcionário 1800, contador 150, água/luz/internet 200,
    // preço 50, insumo 15, taxa de cartão 3% => ponto de equilíbrio R$ 4.517,76 (90,4 vendas)
    [Fact]
    public void Calcular_CenarioRestauranteValidado_RetornaValoresConferidosNoNavegador()
    {
        var entrada = new PontoEquilibrioEntrada(
            AtividadeMei.Comercio,
            SalarioMinimo: 1518m,
            CustosFixos: new List<ItemValor>
            {
                new("Aluguel", 800m),
                new("Funcionário(s) + encargos", 1800m),
                new("Contador", 150m),
                new("Água / luz / internet", 200m),
            },
            PrecoVenda: 50m,
            CustoInsumo: 15m,
            CustosVariaveis: new List<ItemPercentual> { new("Taxa de cartão / maquininha", 3m) },
            DiasVenda: 26);

        var resultado = CalculadoraPontoEquilibrio.Calcular(entrada);

        Assert.True(resultado.Sucesso);
        Assert.Equal(76.90m, resultado.DasTotal, 2);
        Assert.Equal(3026.90m, resultado.TotalCustosFixos, 2);
        Assert.Equal(33.50m, resultado.MargemContribuicaoUnit, 2);
        Assert.Equal(0.67m, resultado.MargemContribuicaoPercent, 2);
        Assert.Equal(4517.76m, Math.Round(resultado.PontoEquilibrioReais, 2));
        Assert.Equal(90.4m, Math.Round(resultado.PontoEquilibrioUnidades, 1));
        Assert.Null(resultado.MensagemAlerta);
    }

    [Fact]
    public void Calcular_SemPrecoVenda_RetornaErro()
    {
        var entrada = new PontoEquilibrioEntrada(
            AtividadeMei.Comercio, 1518m, Array.Empty<ItemValor>(), 0m, 0m, Array.Empty<ItemPercentual>(), 26);

        var resultado = CalculadoraPontoEquilibrio.Calcular(entrada);

        Assert.False(resultado.Sucesso);
        Assert.NotNull(resultado.MensagemErro);
    }

    [Fact]
    public void Calcular_CustoVariavelMaiorQuePreco_RetornaErroDeMargemNegativa()
    {
        var entrada = new PontoEquilibrioEntrada(
            AtividadeMei.Comercio, 1518m, Array.Empty<ItemValor>(),
            PrecoVenda: 10m, CustoInsumo: 15m, CustosVariaveis: Array.Empty<ItemPercentual>(), DiasVenda: 26);

        var resultado = CalculadoraPontoEquilibrio.Calcular(entrada);

        Assert.False(resultado.Sucesso);
    }

    [Fact]
    public void Calcular_FaturamentoAnualAcimaDoLimiteMei_GeraAlerta()
    {
        var entrada = new PontoEquilibrioEntrada(
            AtividadeMei.Comercio, 1518m,
            CustosFixos: new List<ItemValor> { new("Aluguel", 10000m) },
            PrecoVenda: 50m, CustoInsumo: 5m,
            CustosVariaveis: Array.Empty<ItemPercentual>(), DiasVenda: 26);

        var resultado = CalculadoraPontoEquilibrio.Calcular(entrada);

        Assert.True(resultado.Sucesso);
        Assert.NotNull(resultado.MensagemAlerta);
        Assert.True(resultado.FaturamentoAnualProjetado > CalculadoraPontoEquilibrio.LimiteAnualMei);
    }
}
