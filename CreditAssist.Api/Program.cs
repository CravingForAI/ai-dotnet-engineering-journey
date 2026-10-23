using Microsoft.AspNetCore.Http;

var builder = WebApplication.CreateBuilder(args);

// Register dependencies
builder.Services.AddScoped<
    ICreditRuleService,
    CreditRuleService>();

builder.Services.AddSingleton<
    IAiNarrativeService,
    SimulatedNarrativeService>();

var app = builder.Build();


app.MapGet("/health", () => 
    Results.Ok(new {status = "Healthy"}));

app.MapPost("/api/assessments/preview",
    async (
        AssessmentRequest request,
        ICreditRuleService ruleService,
        IAiNarrativeService narrativeService,
        CancellationToken ct) =>
{
    if (request.MonthlyIncome <= 0 ||
        request.MonthlyIncome > 1_000_000_000m ||
        request.ExistingMonthlyObligations < 0 ||
        request.ExistingMonthlyObligations > 1_000_000_000m ||
        request.ProposedMonthlyInstallment < 0 ||
        request.ProposedMonthlyInstallment > 1_000_000_000m)
    {
        return Results.BadRequest(
            new { error = "Invalid financial input." });
    }

    var calculation = ruleService.Evaluate(request);

    var narrative = await narrativeService
        .GenerateAsync(calculation, ct);

    return Results.Ok(new
    {
        calculation,
        narrative,
        workflowStatus = "ManualReviewRequired"
    });
});

app.Run();
