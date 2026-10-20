using MeiFinanceiro.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MeiFinanceiro.Infrastructure.Persistence.Configurations;

public class EstabelecimentoConfiguration : IEntityTypeConfiguration<Estabelecimento>
{
    public void Configure(EntityTypeBuilder<Estabelecimento> builder)
    {
        builder.ToTable("Estabelecimentos");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Nome).IsRequired().HasMaxLength(200);
        builder.Property(e => e.Atividade).HasConversion<string>().HasMaxLength(20);

        builder.HasMany(e => e.Fechamentos)
            .WithOne(f => f.Estabelecimento)
            .HasForeignKey(f => f.EstabelecimentoId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
