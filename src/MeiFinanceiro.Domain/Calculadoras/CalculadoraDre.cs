using MeiFinanceiro.Domain.Enums;

namespace MeiFinanceiro.Domain.Calculadoras;

public record DreEntrada(
    AtividadeMei Atividade,
    decimal SalarioMinimo,
    decimal Faturamento,
    decimal Cmv,
    decimal TotalCustosFixos);

public record DreResultado(
    bool Sucesso,
    string? MensagemErro,
    decimal DasTotal,
    decimal LucroBruto,
    decimal LucroLiquido,
    decimal MargemLiquidaPercentual)
{
    public static DreResultado Erro(string mensagem) =>
        new(false, mensagem, 0, 0, 0, 0);
}

/// <summary>
/// DRE simplificada: Receita - CMV = Lucro Bruto; Lucro Bruto - Custos Fixos - DAS-MEI = Lucro Líquido.
/// Não inclui depreciação, impostos sobre o lucro ou investimentos pontuais.
/// É também o cálculo usado ao gravar um FechamentoMensal real de um estabelecimento.
/// </summary>
public static class CalculadoraDre
{
    public static DreResultado Calcular(DreEntrada entrada)
    {
        if (entrada.Faturamento <= 0)
            return DreResultado.Erro("Informe o faturamento do mês para calcular a DRE.");

        var das = CalculadoraDasMei.Calcular(entrada.SalarioMinimo, entrada.Atividade);
        var lucroBruto = entrada.Faturamento - entrada.Cmv;
        var lucroLiquido = lucroBruto - entrada.TotalCustosFixos - das.Total;
        var margemLiquida = (lucroLiquido / entrada.Faturamento) * 100m;

        return new DreResultado(true, null, das.Total, lucroBruto, lucroLiquido, margemLiquida);
    }
}
