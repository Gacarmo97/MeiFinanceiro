namespace MeiFinanceiro.Domain.Calculadoras;

public enum FaixaCmv
{
    Saudavel,
    Atencao,
    Alerta
}

public record CmvEntrada(
    decimal EstoqueInicial,
    decimal Compras,
    decimal EstoqueFinal,
    decimal Faturamento);

public record CmvResultado(
    bool Sucesso,
    string? MensagemErro,
    decimal CmvReais,
    decimal CmvPercentual,
    FaixaCmv Faixa,
    string MensagemFaixa,
    decimal MargemBrutaRestante)
{
    public static CmvResultado Erro(string mensagem) =>
        new(false, mensagem, 0, 0, FaixaCmv.Alerta, string.Empty, 0);
}

public static class CalculadoraCmv
{
    public static CmvResultado Calcular(CmvEntrada entrada)
    {
        if (entrada.Faturamento <= 0)
            return CmvResultado.Erro("Informe o faturamento do mês para calcular o CMV.");

        var cmvReais = entrada.EstoqueInicial + entrada.Compras - entrada.EstoqueFinal;

        if (cmvReais < 0)
            return CmvResultado.Erro("O estoque final informado é maior que o inicial + compras — confira os valores.");

        var cmvPercentual = (cmvReais / entrada.Faturamento) * 100m;

        var (faixa, mensagem) = cmvPercentual switch
        {
            <= 30 => (FaixaCmv.Saudavel, "Saudável — dentro da faixa de referência para restaurantes (até ~30%)."),
            <= 35 => (FaixaCmv.Atencao, "Atenção — um pouco acima da faixa ideal (30–35%). Vale revisar desperdício e preços."),
            _ => (FaixaCmv.Alerta, "Alerta — acima de 35% costuma indicar desperdício, perdas de insumo ou preço de venda baixo demais.")
        };

        return new CmvResultado(
            true, null,
            cmvReais, cmvPercentual,
            faixa, mensagem,
            entrada.Faturamento - cmvReais);
    }
}
