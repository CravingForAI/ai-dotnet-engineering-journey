namespace CreditAssist.Tests;
using Xunit;

public class CreditRuleTests
{
    private readonly CreditRuleService _service = new();

    [Fact]
    public void ValidInput_Returns40Percent()
    {
        // Arrange
        var request = new AssessmentRequest(
            20000m,
            7000m,
            1000m);

        // Act
        var result = _service.Evaluate(request);

        // Assert
        Assert.Equal(40m, result.DebtBurdenPercent);
    }

    [Fact]
    public void HighObligations_ReturnsAboveThreshold()
    {
        var request = new AssessmentRequest(
            20000m,
            9000m,
            2000m);

        var result = _service.Evaluate(request);

        Assert.Equal(55m, result.DebtBurdenPercent);
        Assert.Equal(
            "AboveExampleThreshold",
            result.Indicator);
    }

    [Fact]
    public void Exactly50Percent_IsWithinThreshold()
    {
        var request = new AssessmentRequest(
            20000m,
            8000m,
            2000m);

        var result = _service.Evaluate(request);

        Assert.Equal(50m, result.DebtBurdenPercent);
        Assert.Equal(
            "WithinExampleThreshold",
            result.Indicator);
    }
}
