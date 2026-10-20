using MeiFinanceiro.Domain.Entities;

namespace MeiFinanceiro.Domain.Repositories;

public interface IEstabelecimentoRepository
{
    Task<List<Estabelecimento>> ListarAsync();
    Task<Estabelecimento?> ObterPorIdAsync(int id);
    Task AdicionarAsync(Estabelecimento estabelecimento);
    Task AtualizarAsync(Estabelecimento estabelecimento);
    Task RemoverAsync(int id);
}
