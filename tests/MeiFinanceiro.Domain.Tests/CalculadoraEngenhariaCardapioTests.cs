using MeiFinanceiro.Domain.Calculadoras;
using Xunit;

namespace MeiFinanceiro.Domain.Tests;

public class CalculadoraEngenhariaCardapioTests
{
    // Mesmo cenário testado manualmente no navegador: 4 pratos, cmg média R$9,22,
    // classificando Espetinho e Refrigerante como Cavalo de batalha, Linguiça como
    // Estrela e Picanha como Enigma.
    [Fact]
    public void Calcular_CenarioValidadoNoNavegador_ClassificaOsQuatroPratosConferidos()
    {
        var entrada = new EngenhariaCardapioEntrada(new List<PratoEntrada>
        {
            new("Espetinho de carne", 300m, 15m, 6m),
            new("Picanha na brasa", 80m, 45m, 22m),
            new("Linguiça acebolada", 150m, 18m, 7m),
            new("Refrigerante", 250m, 6m, 2m),
        });

        var resultado = CalculadoraEngenhariaCardapio.Calcular(entrada);

        Assert.True(resultado.Sucesso);
        Assert.Equal(9.22m, Math.Round(resultado.CmgMedia, 2));
        Assert.Equal(1, resultado.Contagem[CategoriaPrato.Estrela]);
        Assert.Equal(2, resultado.Contagem[CategoriaPrato.CavaloDeBatalha]);
        Assert.Equal(1, resultado.Contagem[CategoriaPrato.Enigma]);
        Assert.Equal(0, resultado.Contagem[CategoriaPrato.Abacaxi]);

        Assert.Equal(CategoriaPrato.CavaloDeBatalha, resultado.Itens.Single(i => i.Nome == "Espetinho de carne").Categoria);
        Assert.Equal(CategoriaPrato.Enigma, resultado.Itens.Single(i => i.Nome == "Picanha na brasa").Categoria);
        Assert.Equal(CategoriaPrato.Estrela, resultado.Itens.Single(i => i.Nome == "Linguiça acebolada").Categoria);
        Assert.Equal(CategoriaPrato.CavaloDeBatalha, resultado.Itens.Single(i => i.Nome == "Refrigerante").Categoria);
    }

    [Fact]
    public void Calcular_MenosDeDoisPratos_RetornaErro()
    {
        var entrada = new EngenhariaCardapioEntrada(new List<PratoEntrada> { new("Único", 10m, 20m, 5m) });

        var resultado = CalculadoraEngenhariaCardapio.Calcular(entrada);

        Assert.False(resultado.Sucesso);
    }
}
