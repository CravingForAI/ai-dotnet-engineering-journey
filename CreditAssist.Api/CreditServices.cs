public sealed record AssessmentRequest(
    decimal MonthlyIncome,
    decimal ExistingMonthlyObligations,
    decimal ProposedMonthlyInstallment);

public sealed record RuleResult(
    decimal DebtBurdenPercent,
    string Indicator);

public sealed record NarrativeResult(
    string Mode,
    string Summary);

public interface ICreditRuleService
{
    RuleResult Evaluate(AssessmentRequest request);
}

public interface IAiNarrativeService
{
    Task<NarrativeResult> GenerateAsync(
        RuleResult result,
        CancellationToken cancellationToken);
}

public sealed class CreditRuleService : ICreditRuleService
{
    public RuleResult Evaluate(AssessmentRequest request)
    {
        decimal totalObligations =
            request.ExistingMonthlyObligations +
            request.ProposedMonthlyInstallment;

        decimal percentage = Math.Round(
            totalObligations / request.MonthlyIncome * 100m,
            2);

        string indicator = percentage > 50m
            ? "AboveExampleThreshold"
            : "WithinExampleThreshold";

        return new RuleResult(percentage, indicator);
    }
}

public sealed class SimulatedNarrativeService
    : IAiNarrativeService
{
    public Task<NarrativeResult> GenerateAsync(
        RuleResult result,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        return Task.FromResult(new NarrativeResult(
            "simulated",
            $"Debt burden is {result.DebtBurdenPercent:0.00}%. " +
            "Human review is required."));
    }
}