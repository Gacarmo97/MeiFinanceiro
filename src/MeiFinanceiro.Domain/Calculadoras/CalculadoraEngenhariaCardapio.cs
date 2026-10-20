namespace MeiFinanceiro.Domain.Calculadoras;

public enum CategoriaPrato
{
    Estrela,
    CavaloDeBatalha,
    Enigma,
    Abacaxi
}

public record PratoEntrada(string Nome, decimal Quantidade, decimal Preco, decimal Custo);

public record PratoClassificado(
    string Nome,
    decimal Quantidade,
    decimal Preco,
    decimal Custo,
    decimal Margem,
    decimal Participacao,
    CategoriaPrato Categoria);

public record EngenhariaCardapioEntrada(IReadOnlyList<PratoEntrada> Pratos);

public record EngenhariaCardapioResultado(
    bool Sucesso,
    string? MensagemErro,
    decimal CmgMedia,
    IReadOnlyList<PratoClassificado> Itens,
    IReadOnlyDictionary<CategoriaPrato, int> Contagem)
{
    public static EngenhariaCardapioResultado Erro(string mensagem) =>
        new(false, mensagem, 0, Array.Empty<PratoClassificado>(), new Dictionary<CategoriaPrato, int>());
}

/// <summary>
/// Matriz clássica de engenharia de cardápio (Kasavana &amp; Smith): compara a participação
/// de cada prato nas vendas com a margem de contribuição média do cardápio.
/// </summary>
public static class CalculadoraEngenhariaCardapio
{
    public static EngenhariaCardapioResultado Calcular(EngenhariaCardapioEntrada entrada)
    {
        var itens = entrada.Pratos.Where(p => p.Quantidade > 0 && p.Preco > 0).ToList();

        if (itens.Count < 2)
            return EngenhariaCardapioResultado.Erro("Informe quantidade e preço de pelo menos 2 pratos para comparar.");

        var totalQtd = itens.Sum(i => i.Quantidade);
        var totalContribuicao = itens.Sum(i => (i.Preco - i.Custo) * i.Quantidade);
        var cmgMedia = totalContribuicao / totalQtd;
        var popularidadeEsperada = 1m / itens.Count;
        var limiarPopularidade = popularidadeEsperada * 0.7m;

        var contagem = new Dictionary<CategoriaPrato, int>
        {
            [CategoriaPrato.Estrela] = 0,
            [CategoriaPrato.CavaloDeBatalha] = 0,
            [CategoriaPrato.Enigma] = 0,
            [CategoriaPrato.Abacaxi] = 0
        };

        var classificados = new List<PratoClassificado>();

        foreach (var item in itens)
        {
            var margem = item.Preco - item.Custo;
            var participacao = item.Quantidade / totalQtd;
            var popular = participacao >= limiarPopularidade;
            var lucrativo = margem >= cmgMedia;

            var categoria = (popular, lucrativo) switch
            {
                (true, true) => CategoriaPrato.Estrela,
                (true, false) => CategoriaPrato.CavaloDeBatalha,
                (false, true) => CategoriaPrato.Enigma,
                (false, false) => CategoriaPrato.Abacaxi
            };

            contagem[categoria]++;
            classificados.Add(new PratoClassificado(
                item.Nome, item.Quantidade, item.Preco, item.Custo, margem, participacao, categoria));
        }

        return new EngenhariaCardapioResultado(true, null, cmgMedia, classificados, contagem);
    }
}
