using MeiFinanceiro.Domain.Calculadoras;
using MeiFinanceiro.Domain.Entities;
using MeiFinanceiro.Domain.Repositories;

namespace MeiFinanceiro.Application;

public class FechamentoMensalAppService
{
    private readonly IFechamentoMensalRepository _fechamentoRepositorio;
    private readonly IEstabelecimentoRepository _estabelecimentoRepositorio;

    public FechamentoMensalAppService(
        IFechamentoMensalRepository fechamentoRepositorio,
        IEstabelecimentoRepository estabelecimentoRepositorio)
    {
        _fechamentoRepositorio = fechamentoRepositorio;
        _estabelecimentoRepositorio = estabelecimentoRepositorio;
    }

    public Task<List<FechamentoMensal>> ListarAsync(int estabelecimentoId) =>
        _fechamentoRepositorio.ListarPorEstabelecimentoAsync(estabelecimentoId);

    public Task<FechamentoMensal?> ObterPorMesAsync(int estabelecimentoId, int ano, int mes) =>
        _fechamentoRepositorio.ObterPorMesAsync(estabelecimentoId, ano, mes);

    /// <summary>
    /// Lança (ou atualiza, se já existir) o fechamento de um mês. Usa a mesma
    /// CalculadoraDre das simulações para calcular DAS, lucro líquido e margem,
    /// garantindo que o valor gravado é sempre consistente com a fórmula vigente no momento do lançamento.
    /// </summary>
    public async Task<FechamentoMensal> LancarAsync(
        int estabelecimentoId, int ano, int mes, decimal faturamento, decimal cmv, decimal custosFixos)
    {
        var estabelecimento = await _estabelecimentoRepositorio.ObterPorIdAsync(estabelecimentoId)
            ?? throw new InvalidOperationException("Estabelecimento não encontrado.");

        var dre = CalculadoraDre.Calcular(new DreEntrada(
            estabelecimento.Atividade, estabelecimento.SalarioMinimoReferencia, faturamento, cmv, custosFixos));

        if (!dre.Sucesso)
            throw new InvalidOperationException(dre.MensagemErro);

        var existente = await _fechamentoRepositorio.ObterPorMesAsync(estabelecimentoId, ano, mes);
        var fechamento = existente ?? new FechamentoMensal
        {
            EstabelecimentoId = estabelecimentoId,
            Ano = ano,
            Mes = mes,
        };

        fechamento.Faturamento = faturamento;
        fechamento.Cmv = cmv;
        fechamento.CustosFixos = custosFixos;
        fechamento.DasCalculado = dre.DasTotal;
        fechamento.LucroLiquido = dre.LucroLiquido;
        fechamento.MargemLiquida = dre.MargemLiquidaPercentual;

        if (existente is null)
            await _fechamentoRepositorio.AdicionarAsync(fechamento);
        else
            await _fechamentoRepositorio.AtualizarAsync(fechamento);

        return fechamento;
    }

    public Task RemoverAsync(int id) => _fechamentoRepositorio.RemoverAsync(id);
}
