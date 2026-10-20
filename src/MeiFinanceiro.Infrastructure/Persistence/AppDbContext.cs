using MeiFinanceiro.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace MeiFinanceiro.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Estabelecimento> Estabelecimentos => Set<Estabelecimento>();
    public DbSet<FechamentoMensal> FechamentosMensais => Set<FechamentoMensal>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}
