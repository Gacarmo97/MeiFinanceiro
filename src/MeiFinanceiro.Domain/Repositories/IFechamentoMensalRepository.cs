using MeiFinanceiro.Domain.Entities;

namespace MeiFinanceiro.Domain.Repositories;

public interface IFechamentoMensalRepository
{
    Task<List<FechamentoMensal>> ListarPorEstabelecimentoAsync(int estabelecimentoId);
    Task<FechamentoMensal?> ObterPorIdAsync(int id);
    Task<FechamentoMensal?> ObterPorMesAsync(int estabelecimentoId, int ano, int mes);
    Task AdicionarAsync(FechamentoMensal fechamento);
    Task AtualizarAsync(FechamentoMensal fechamento);
    Task RemoverAsync(int id);
}
