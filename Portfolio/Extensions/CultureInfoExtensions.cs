using System.Diagnostics.Contracts;
using System.Globalization;
using Portfolio.Enums;

namespace Portfolio.Extensions;

public static class CultureInfoExtensions
{
    [Pure]
    public static bool HasParent(this CultureInfo info) => info.Parent.Name != string.Empty;

    [Pure]
    public static bool IsDirectParentOf(this CultureInfo a, CultureInfo b) => a.Equals(b.Parent);

    [Pure]
    public static bool IsDirectChildOf(this CultureInfo a, CultureInfo b) => a.Parent.Equals(b);

    public static CultureInfoComparisonOutcome GreedyComparison(this CultureInfo a, CultureInfo b)
    {
        if (a.Equals(b)) return CultureInfoComparisonOutcome.ExactMatch;
        if (a.IsDirectChildOf(b)) return CultureInfoComparisonOutcome.IsChildOf;
        if (a.IsDirectParentOf(b)) return CultureInfoComparisonOutcome.IsParentOf;
        if (a.HasParent() && b.HasParent() && a.Parent.Equals(b.Parent)) return CultureInfoComparisonOutcome.HasSharedParent;
        return CultureInfoComparisonOutcome.None;
    }

    public static int BestGreedyEquivalent(this IEnumerable<CultureInfo> cultures, CultureInfo target)
    {
        var outcomes = cultures.Select(culture => culture.GreedyComparison(target)).ToList();
        
        var exactIdx = outcomes.IndexOf(CultureInfoComparisonOutcome.ExactMatch);
        if (exactIdx != -1) return exactIdx;

        var parentOfIdx = outcomes.IndexOf(CultureInfoComparisonOutcome.IsParentOf);
        if (parentOfIdx != -1) return parentOfIdx;

        var childOfIdx = outcomes.IndexOf(CultureInfoComparisonOutcome.IsChildOf);
        if (childOfIdx != -1) return childOfIdx;

        var sharedIdx = outcomes.IndexOf(CultureInfoComparisonOutcome.HasSharedParent);
        return sharedIdx;
    }
}