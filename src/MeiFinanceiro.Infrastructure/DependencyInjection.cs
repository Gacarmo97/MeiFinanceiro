using MeiFinanceiro.Application;
using MeiFinanceiro.Domain.Repositories;
using MeiFinanceiro.Infrastructure.Persistence;
using MeiFinanceiro.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace MeiFinanceiro.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<AppDbContext>(options => options.UseSqlite(connectionString));

        services.AddScoped<IEstabelecimentoRepository, EstabelecimentoRepository>();
        services.AddScoped<IFechamentoMensalRepository, FechamentoMensalRepository>();

        services.AddScoped<EstabelecimentoAppService>();
        services.AddScoped<FechamentoMensalAppService>();
        services.AddScoped<ResumoAppService>();

        return services;
    }
}
