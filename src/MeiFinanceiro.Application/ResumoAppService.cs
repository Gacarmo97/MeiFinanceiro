using MeiFinanceiro.Domain.Repositories;

namespace MeiFinanceiro.Application;

public record ResumoMensal(
    int Ano,
    int Mes,
    decimal Faturamento,
    decimal Cmv,
    decimal CmvPercentual,
    decimal CustosFixos,
    decimal DasCalculado,
    decimal LucroLiquido,
    decimal MargemLiquida);

public record ResumoEstabelecimento(
    int EstabelecimentoId,
    string NomeEstabelecimento,
    ResumoMensal? UltimoMes,
    IReadOnlyList<ResumoMensal> Historico,
    decimal? MediaLucroLiquidoUltimos3Meses,
    decimal? MediaLucroLiquidoUltimos6Meses,
    decimal? MediaMargemLiquidaUltimos3Meses);

/// <summary>
/// Monta o resumo histórico de um estabelecimento a partir dos fechamentos mensais reais
/// já lançados — a tela de "como estão as contas".
/// </summary>
public class ResumoAppService
{
    private readonly IFechamentoMensalRepository _fechamentoRepositorio;
    private readonly IEstabelecimentoRepository _estabelecimentoRepositorio;

    public ResumoAppService(
        IFechamentoMensalRepository fechamentoRepositorio,
        IEstabelecimentoRepository estabelecimentoRepositorio)
    {
        _fechamentoRepositorio = fechamentoRepositorio;
        _estabelecimentoRepositorio = estabelecimentoRepositorio;
    }

    public async Task<ResumoEstabelecimento?> ObterResumoAsync(int estabelecimentoId)
    {
        var estabelecimento = await _estabelecimentoRepositorio.ObterPorIdAsync(estabelecimentoId);
        if (estabelecimento is null)
            return null;

        var fechamentos = await _fechamentoRepositorio.ListarPorEstabelecimentoAsync(estabelecimentoId);

        var historico = fechamentos
            .OrderBy(f => f.Ano).ThenBy(f => f.Mes)
            .Select(f => new ResumoMensal(
                f.Ano, f.Mes, f.Faturamento, f.Cmv,
                f.Faturamento > 0 ? f.Cmv / f.Faturamento * 100m : 0,
                f.CustosFixos, f.DasCalculado, f.LucroLiquido, f.MargemLiquida))
            .ToList();

        var ultimo = historico.LastOrDefault();

        decimal? MediaLucro(int quantidadeMeses)
        {
            var ultimosN = historico.TakeLast(quantidadeMeses).ToList();
            return ultimosN.Count > 0 ? ultimosN.Average(h => h.LucroLiquido) : null;
        }

        decimal? MediaMargem(int quantidadeMeses)
        {
            var ultimosN = historico.TakeLast(quantidadeMeses).ToList();
            return ultimosN.Count > 0 ? ultimosN.Average(h => h.MargemLiquida) : null;
        }

        return new ResumoEstabelecimento(
            estabelecimento.Id,
            estabelecimento.Nome,
            ultimo,
            historico,
            MediaLucro(3),
            MediaLucro(6),
            MediaMargem(3));
    }
}
