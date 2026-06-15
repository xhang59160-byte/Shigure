namespace Shigure;

/// <summary>
/// 把动态单位 / 数量字段渲染成人类可读摘要 (如 "带[X]且血最低 (&lt;80)")。
/// 单位列表的"摘要"列与单位编辑器的实时预览共用同一套措辞, 避免两处描述漂移。
/// </summary>
internal static class UnitSummary
{
    public static string Describe(ModuleUnit unit)
    {
        var threshold = DescribeThreshold(unit.HealthThreshold, unit.HealthThresholdField);
        var aura = unit.AuraNames is { Count: > 0 } ? unit.AuraNames[0] : "?";
        var auras = unit.AuraNames is { Count: > 0 } ? string.Join("/", unit.AuraNames) : "?";
        var dir = unit.Reverse ? "逆序" : "正序";
        return unit.Kind switch
        {
            UnitSelectorKind.LowestHealth => $"血量最低 (<{threshold})",
            UnitSelectorKind.LowestHealthWithAnyAura => $"带任一[{auras}]且血最低 (<{threshold})",
            UnitSelectorKind.LowestHealthWithoutAura => $"不带[{aura}]且血最低 (<{threshold})",
            UnitSelectorKind.LowestHealthWithAura => $"带[{aura}]且血最低 (<{threshold})",
            UnitSelectorKind.LowestHealthWithAuraCount => $"[{aura}]={unit.AuraCount}且血最低 (<{threshold})",
            UnitSelectorKind.UnitWithRole => $"职责={unit.Role} {dir}首个",
            UnitSelectorKind.UnitWithRoleWithoutAura => $"职责={unit.Role}且不带[{aura}] {dir}",
            UnitSelectorKind.UnitWithAura => $"带[{aura}] 持续最久",
            UnitSelectorKind.UnitWithDispelType => $"驱散类型={unit.DispelType}",
            _ => unit.Kind.ToString()
        };
    }

    public static string Describe(ModuleCountField count)
    {
        var threshold = DescribeThreshold(count.HealthThreshold, count.HealthThresholdField);
        return count.Kind switch
        {
            CountKind.UnitsBelowHealth => $"血量<{threshold} 的人数",
            CountKind.UnitsWithoutAuraBelowHealth => $"不带[{count.AuraName}]且血<{threshold} 的人数",
            CountKind.UnitsWithAura => $"带[{count.AuraName}] 的人数",
            _ => count.Kind.ToString()
        };
    }

    private static string DescribeThreshold(int? fixedValue, string? field)
    {
        return string.IsNullOrWhiteSpace(field)
            ? (fixedValue ?? 100).ToString()
            : $"动态:{field.Trim()}";
    }
}
