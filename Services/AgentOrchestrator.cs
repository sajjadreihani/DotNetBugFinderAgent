using BugFinderAgent.Interfaces;
using BugFinderAgent.Models;
using Microsoft.Extensions.Logging;
using System.Text;

namespace BugFinderAgent.Services;

public class AgentOrchestrator(ICodeReviewAgent reviewer, IPlannerAgent planner, ICodeDebuggerAgent debugger, ICodeVerifierAgent verifier, ILogger<AgentOrchestrator> logger) : IAgentOrchestrator
{
    public async Task<PipelineResult> RunAsync(AgentInput input, int maxSteps = 5)
    {
            var result = new PipelineResult(default, null, input.Request, default)
            {
                OriginalCode = input.Code
            };

        try
        {
            var currentCode = input.Code;

            logger.LogInformation(
                "Starting pipeline. Request: {Request}",
                input.Request ?? "(none — fix all issues found)");

            for (int i = 1; i <= maxSteps; i++)
            {
                logger.LogInformation("Agent step {Step}/{MaxSteps}", i, maxSteps);
                var report = new StepReport { Step = i };

                // Step 1: Review
                var review = await reviewer.Review(currentCode);
                report.Review = review;
                logger.LogInformation(
                    "Review complete. Severity: {Severity}/10. Issues: {Issues}",
                    review.SeverityScore,
                    string.Join(", ", review.Issues));

                if (review.SeverityScore <= 2 && review.Issues.Count == 0)
                {
                    logger.LogInformation("Code is clean. Exiting early at step {Step}.", i);
                    result.Reports.Add(report);
                    return result with
                    {
                        Success = true,
                        FinalCode = currentCode,
                        FinalVerdict = "Code was already in good shape. No significant issues found.",
                        TotalIterations = i   // was always 0 before
                    };
                }

                // Step 2: Plan
                var plan = await planner.CreatePlan(currentCode, review, input.Request);
                report.Plan = plan;
                logger.LogInformation(GeneratePlanReport(plan));

                // Step 3: Debug / Apply
                var debugSteps = await debugger.DebugWithPlan(currentCode, plan);
                report.DebugSteps = debugSteps;

                foreach (var step in debugSteps)
                    logger.LogInformation(
                        "Debug step [{Success}] {Action}: {Explanation}",
                        step.Success ? "OK" : "FAIL", step.Action, step.Explanation);

                var fixedCode = debugSteps
                    .Where(d => !string.IsNullOrWhiteSpace(d.FixedCode))
                    .LastOrDefault()?.FixedCode ?? currentCode;

                // Step 4: Verify
                var verification = await verifier.Verify(input.Code, fixedCode, plan, input.Request, "qwen2.5-coder:7b");
                report.Verification = verification;
                logger.LogInformation(
                    "Verification: {Verdict}. Complete: {IsComplete}",
                    verification.Verdict, verification.IsComplete);

                result.Reports.Add(report);

                if (verification.IsComplete)
                {
                    logger.LogInformation("Pipeline complete after {Step} step(s).", i);
                    return result with
                    {
                        Success = true,
                        FinalCode = verification.FinalCode ?? fixedCode,
                        FinalVerdict = verification.Verdict,
                        TotalIterations = i
                    };
                }

                currentCode = fixedCode;
                logger.LogWarning(
                    "Remaining issues after step {Step}: {Issues}",
                    i, string.Join(", ", verification.RemainingIssues));
            }

            logger.LogWarning("Max steps ({MaxSteps}) reached without full completion.", maxSteps);
            return result with
            {
                FinalCode = currentCode,
                FinalVerdict = "Max steps reached. Partial improvements may have been applied.",
                TotalIterations = maxSteps
            };
        }
        catch (Exception e)
        {
            logger.LogError(e, e.Message);
            return new PipelineResult(false, result.FinalCode, e.Message, 0);
        }

    }

    private static string GeneratePlanReport(AiPlanResponse response)
    {
        StringBuilder plan = new();

        plan.AppendLine($"Plan created. Goal: {response.Goal}. Steps: {response.Steps.Count}");

        foreach (var step in response.Steps.OrderBy(r => r.Order))
        {
            plan.AppendLine($"  {step.Order}. {step.Description}");
        }

        return plan.ToString();
    }
}