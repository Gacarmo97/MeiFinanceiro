using MeiFinanceiro.Domain.Enums;

namespace MeiFinanceiro.Domain.Calculadoras;

public record DasMeiResultado(decimal Inss, decimal TributoFixo, decimal Total);

/// <summary>
/// DAS-MEI = 5% do salário mínimo (INSS) + R$1,00 (ICMS) e/ou R$5,00 (ISS), conforme a
/// atividade. Valores fixos em todo o Brasil (LC 123/2006), independente do estado.
/// </summary>
public static class CalculadoraDasMei
{
    public static DasMeiResultado Calcular(decimal salarioMinimo, AtividadeMei atividade)
    {
        var inss = salarioMinimo * 0.05m;
        var tributoFixo = atividade switch
        {
            AtividadeMei.Comercio => 1m,
            AtividadeMei.Servico => 5m,
            AtividadeMei.Ambos => 6m,
            _ => throw new ArgumentOutOfRangeException(nameof(atividade))
        };
        return new DasMeiResultado(inss, tributoFixo, inss + tributoFixo);
    }
}
