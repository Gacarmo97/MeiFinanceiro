using MeiFinanceiro.Domain.Calculadoras;
using Xunit;

namespace MeiFinanceiro.Domain.Tests;

public class CalculadoraInvestimentoTests
{
    // Mesmo cenário testado manualmente no navegador: investimento total R$17.000,
    // lucro mensal R$1.200 => payback 14,2 meses (~1 ano e 2 meses), ROI 84,7% a.a.
    [Fact]
    public void Calcular_CenarioValidadoNoNavegador_RetornaPaybackERoiConferidos()
    {
        var entrada = new InvestimentoEntrada(
            Investimentos: new List<ItemValor>
            {
                new("Reforma / obra", 5000m),
                new("Equipamentos", 8000m),
                new("Móveis e utensílios", 2000m),
                new("Estoque inicial", 1500m),
                new("Abertura de CNPJ / alvarás", 500m),
            },
            LucroMensal: 1200m);

        var resultado = CalculadoraInvestimento.Calcular(entrada);

        Assert.True(resultado.Sucesso);
        Assert.Equal(17000.00m, resultado.InvestimentoTotal, 2);
        Assert.Equal(14.2m, Math.Round(resultado.PaybackMeses, 1));
        Assert.Equal(1, resultado.PaybackAnos);
        Assert.Equal(2, resultado.PaybackRestoMeses);
        Assert.Equal(84.7m, Math.Round(resultado.RoiAnualPercentual, 1));
        Assert.Null(resultado.MensagemAlerta);
    }

    [Fact]
    public void Calcular_SemLucroMensal_RetornaErro()
    {
        var entrada = new InvestimentoEntrada(new List<ItemValor> { new("Item", 1000m) }, 0m);

        var resultado = CalculadoraInvestimento.Calcular(entrada);

        Assert.False(resultado.Sucesso);
    }

    [Fact]
    public void Calcular_SemInvestimento_RetornaErro()
    {
        var entrada = new InvestimentoEntrada(Array.Empty<ItemValor>(), 1200m);

        var resultado = CalculadoraInvestimento.Calcular(entrada);

        Assert.False(resultado.Sucesso);
    }

    [Fact]
    public void Calcular_PaybackAcimaDe5Anos_GeraAlerta()
    {
        var entrada = new InvestimentoEntrada(new List<ItemValor> { new("Item", 100000m) }, 1000m);

        var resultado = CalculadoraInvestimento.Calcular(entrada);

        Assert.True(resultado.Sucesso);
        Assert.NotNull(resultado.MensagemAlerta);
    }
}
