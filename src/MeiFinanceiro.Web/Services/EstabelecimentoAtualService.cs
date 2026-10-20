namespace MeiFinanceiro.Web.Services;

/// <summary>
/// Estado com escopo de circuito (uma instância por aba/sessão do Blazor Server) que guarda
/// qual estabelecimento está selecionado, usado por todas as páginas de calculadora e pelos
/// dados reais (fechamento mensal / resumo).
/// </summary>
public class EstabelecimentoAtualService
{
    public int? EstabelecimentoId { get; private set; }
    public string? NomeEstabelecimento { get; private set; }

    public event Action? Mudou;
    public event Action? ListaMudou;

    public void Definir(int id, string nome)
    {
        EstabelecimentoId = id;
        NomeEstabelecimento = nome;
        Mudou?.Invoke();
    }

    public void Limpar()
    {
        EstabelecimentoId = null;
        NomeEstabelecimento = null;
        Mudou?.Invoke();
    }

    public void NotificarListaAlterada() => ListaMudou?.Invoke();
}
