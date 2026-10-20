using MeiFinanceiro.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MeiFinanceiro.Infrastructure.Persistence.Configurations;

public class FechamentoMensalConfiguration : IEntityTypeConfiguration<FechamentoMensal>
{
    public void Configure(EntityTypeBuilder<FechamentoMensal> builder)
    {
        builder.ToTable("FechamentosMensais");
        builder.HasKey(f => f.Id);

        // Um único fechamento por estabelecimento + ano + mês.
        builder.HasIndex(f => new { f.EstabelecimentoId, f.Ano, f.Mes }).IsUnique();
    }
}
