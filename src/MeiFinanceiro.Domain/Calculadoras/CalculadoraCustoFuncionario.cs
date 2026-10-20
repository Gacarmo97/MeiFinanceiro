namespace MeiFinanceiro.Domain.Calculadoras;

public record CustoFuncionarioEntrada(
    decimal Salario,
    decimal ValeTransporte,
    decimal ValeRefeicao,
    decimal OutrosBeneficios);

public record CustoFuncionarioResultado(
    bool Sucesso,
    string? MensagemErro,
    decimal Fgts,
    decimal InssPatronal,
    decimal Provisao13,
    decimal ProvisaoFerias,
    decimal Encargos,
    decimal Beneficios,
    decimal CustoTotalMensal,
    decimal OverheadPercentual)
{
    public static CustoFuncionarioResultado Erro(string mensagem) =>
        new(false, mensagem, 0, 0, 0, 0, 0, 0, 0, 0);
}

/// <summary>
/// Regime especial do empregador MEI: INSS patronal reduzido a 3% (em vez dos ~20% do
/// CLT comum), válido para o único empregado que o MEI pode ter (LC 128/2008).
/// Não inclui custo eventual de rescisão (multa de 40% do FGTS).
/// </summary>
public static class CalculadoraCustoFuncionario
{
    public static CustoFuncionarioResultado Calcular(CustoFuncionarioEntrada entrada)
    {
        if (entrada.Salario <= 0)
            return CustoFuncionarioResultado.Erro("Informe o salário bruto mensal do funcionário.");

        var fgts = entrada.Salario * 0.08m;
        var inssPatronal = entrada.Salario * 0.03m;
        var provisao13 = entrada.Salario / 12m;
        var provisaoFeriasBase = entrada.Salario / 12m;
        var provisaoTercoFerias = provisaoFeriasBase / 3m;
        var provisaoFerias = provisaoFeriasBase + provisaoTercoFerias;

        var encargos = fgts + inssPatronal + provisao13 + provisaoFerias;
        var beneficios = entrada.ValeTransporte + entrada.ValeRefeicao + entrada.OutrosBeneficios;
        var custoTotalMensal = entrada.Salario + encargos + beneficios;
        var overheadPercentual = ((custoTotalMensal - entrada.Salario) / entrada.Salario) * 100m;

        return new CustoFuncionarioResultado(
            true, null,
            fgts, inssPatronal, provisao13, provisaoFerias,
            encargos, beneficios, custoTotalMensal, overheadPercentual);
    }
}
