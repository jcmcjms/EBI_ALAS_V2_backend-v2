namespace EBI.ALAS.Api.Features.ApprovalMatrix;

public static class ApprovalFormConventions
{
    public static int ResolveApprovalTermDays(int termDays, int? policyTermMonths)
    {
        if (policyTermMonths.HasValue && policyTermMonths.Value > 0)
        {
            return policyTermMonths.Value * 30;
        }
        return termDays;
    }

    public static decimal ToAnnualRatePercent(decimal monthlyRate)
    {
        return Math.Round(monthlyRate * 12 * 100, 4);
    }
}