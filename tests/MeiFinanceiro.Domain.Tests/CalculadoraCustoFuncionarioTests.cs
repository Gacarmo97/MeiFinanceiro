using MeiFinanceiro.Domain.Calculadoras;
using Xunit;

namespace MeiFinanceiro.Domain.Tests;

public class CalculadoraCustoFuncionarioTests
{
    // Mesmo cenário testado manualmente no navegador: salário R$1.518 (mínimo), sem
    // benefícios => custo total mensal R$1.980,15 (30,4% acima do salário).
    [Fact]
    public void Calcular_CenarioValidadoNoNavegador_RetornaCustoTotalConferido()
    {
        var entrada = new CustoFuncionarioEntrada(Salario: 1518m, ValeTransporte: 0m, ValeRefeicao: 0m, OutrosBeneficios: 0m);

        var resultado = CalculadoraCustoFuncionario.Calcular(entrada);

        Assert.True(resultado.Sucesso);
        Assert.Equal(1980.15m, Math.Round(resultado.CustoTotalMensal, 2));
        Assert.Equal(30.4m, Math.Round(resultado.OverheadPercentual, 1));
        Assert.Equal(462.15m, Math.Round(resultado.Encargos, 2));
    }

    [Fact]
    public void Calcular_SemSalario_RetornaErro()
    {
        var entrada = new CustoFuncionarioEntrada(0m, 0m, 0m, 0m);

        var resultado = CalculadoraCustoFuncionario.Calcular(entrada);

        Assert.False(resultado.Sucesso);
    }
}
