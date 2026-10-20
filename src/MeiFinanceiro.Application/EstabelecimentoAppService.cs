using MeiFinanceiro.Domain.Entities;
using MeiFinanceiro.Domain.Enums;
using MeiFinanceiro.Domain.Repositories;

namespace MeiFinanceiro.Application;

public class EstabelecimentoAppService
{
    private readonly IEstabelecimentoRepository _repositorio;

    public EstabelecimentoAppService(IEstabelecimentoRepository repositorio)
    {
        _repositorio = repositorio;
    }

    public Task<List<Estabelecimento>> ListarAsync() => _repositorio.ListarAsync();

    public Task<Estabelecimento?> ObterAsync(int id) => _repositorio.ObterPorIdAsync(id);

    public async Task<Estabelecimento> CriarAsync(string nome, AtividadeMei atividade, decimal salarioMinimoReferencia)
    {
        var estabelecimento = new Estabelecimento
        {
            Nome = nome,
            Atividade = atividade,
            SalarioMinimoReferencia = salarioMinimoReferencia,
        };
        await _repositorio.AdicionarAsync(estabelecimento);
        return estabelecimento;
    }

    public async Task AtualizarAsync(int id, string nome, AtividadeMei atividade, decimal salarioMinimoReferencia)
    {
        var estabelecimento = await _repositorio.ObterPorIdAsync(id)
            ?? throw new InvalidOperationException("Estabelecimento não encontrado.");

        estabelecimento.Nome = nome;
        estabelecimento.Atividade = atividade;
        estabelecimento.SalarioMinimoReferencia = salarioMinimoReferencia;

        await _repositorio.AtualizarAsync(estabelecimento);
    }

    public Task RemoverAsync(int id) => _repositorio.RemoverAsync(id);
}
