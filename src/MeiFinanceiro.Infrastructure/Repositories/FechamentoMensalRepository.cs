using MeiFinanceiro.Domain.Entities;
using MeiFinanceiro.Domain.Repositories;
using MeiFinanceiro.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace MeiFinanceiro.Infrastructure.Repositories;

public class FechamentoMensalRepository : IFechamentoMensalRepository
{
    private readonly AppDbContext _db;

    public FechamentoMensalRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<List<FechamentoMensal>> ListarPorEstabelecimentoAsync(int estabelecimentoId) =>
        await _db.FechamentosMensais
            .Where(f => f.EstabelecimentoId == estabelecimentoId)
            .OrderBy(f => f.Ano).ThenBy(f => f.Mes)
            .ToListAsync();

    public Task<FechamentoMensal?> ObterPorIdAsync(int id) =>
        _db.FechamentosMensais.FirstOrDefaultAsync(f => f.Id == id);

    public Task<FechamentoMensal?> ObterPorMesAsync(int estabelecimentoId, int ano, int mes) =>
        _db.FechamentosMensais.FirstOrDefaultAsync(
            f => f.EstabelecimentoId == estabelecimentoId && f.Ano == ano && f.Mes == mes);

    public async Task AdicionarAsync(FechamentoMensal fechamento)
    {
        _db.FechamentosMensais.Add(fechamento);
        await _db.SaveChangesAsync();
    }

    public async Task AtualizarAsync(FechamentoMensal fechamento)
    {
        _db.FechamentosMensais.Update(fechamento);
        await _db.SaveChangesAsync();
    }

    public async Task RemoverAsync(int id)
    {
        var entidade = await _db.FechamentosMensais.FindAsync(id);
        if (entidade is not null)
        {
            _db.FechamentosMensais.Remove(entidade);
            await _db.SaveChangesAsync();
        }
    }
}
