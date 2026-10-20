using MeiFinanceiro.Domain.Entities;
using MeiFinanceiro.Domain.Repositories;
using MeiFinanceiro.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace MeiFinanceiro.Infrastructure.Repositories;

public class EstabelecimentoRepository : IEstabelecimentoRepository
{
    private readonly AppDbContext _db;

    public EstabelecimentoRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<List<Estabelecimento>> ListarAsync() =>
        await _db.Estabelecimentos.OrderBy(e => e.Nome).ToListAsync();

    public Task<Estabelecimento?> ObterPorIdAsync(int id) =>
        _db.Estabelecimentos.FirstOrDefaultAsync(e => e.Id == id);

    public async Task AdicionarAsync(Estabelecimento estabelecimento)
    {
        _db.Estabelecimentos.Add(estabelecimento);
        await _db.SaveChangesAsync();
    }

    public async Task AtualizarAsync(Estabelecimento estabelecimento)
    {
        _db.Estabelecimentos.Update(estabelecimento);
        await _db.SaveChangesAsync();
    }

    public async Task RemoverAsync(int id)
    {
        var entidade = await _db.Estabelecimentos.FindAsync(id);
        if (entidade is not null)
        {
            _db.Estabelecimentos.Remove(entidade);
            await _db.SaveChangesAsync();
        }
    }
}
