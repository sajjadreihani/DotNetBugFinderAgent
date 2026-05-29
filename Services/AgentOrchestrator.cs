using BugFinderAgent.Interfaces;
using BugFinderAgent.Models;

namespace BugFinderAgent.Services;

public class AgentOrchestrator(ICodeReviewAgent reviewer, IPlannerAgent planner, CodeDebuggerAgentExtended debugger, ICodeVerifierAgent verifier) : IAgentOrchestrator
{
    public async Task<PipelineResult> RunAsync(AgentInput input, int maxSteps = 5)
    {
        var result = new PipelineResult
        {
            OriginalCode = input.Code,
            OriginalRequest = input.Request
        };

        var currentCode = input.Code;

        Console.WriteLine(" Starting Autonomous Code Agent Pipeline");
        Console.WriteLine($" Request: {input.Request ?? "(none — fix all issues found)"}");
        Console.WriteLine(new string('─', 60));

        for (int i = 1; i <= maxSteps; i++)
        {
            Console.WriteLine($"\n Agent Step {i}/{maxSteps}");
            var report = new StepReport { Step = i };

            // ── Step 1: Review ──────────────────────────────────────
            Console.WriteLine("   Step 1: Reviewing code...");
            var review = await reviewer.Review(currentCode);
            report.Review = review;
            PrintReview(review);

            // If severity is very low and no issues, we might already be done
            if (review.SeverityScore <= 2 && review.Issues.Count == 0)
            {
                Console.WriteLine("   Code looks clean. Skipping further steps.");
                result.Success = true;
                result.FinalCode = currentCode;
                result.FinalVerdict = "Code was already in good shape. No significant issues found.";
                result.Reports.Add(report);
                break;
            }

            // ── Step 2: Plan ────────────────────────────────────────
            Console.WriteLine("    Step 2: Creating plan...");
            var plan = await planner.CreatePlan(currentCode, review, input.Request);
            report.Plan = plan;
            PrintPlan(plan);

            // ── Step 3: Debug / Apply Changes ───────────────────────
            Console.WriteLine("   Step 3: Applying changes...");
            var debugSteps = await debugger.DebugWithPlan(currentCode, plan);
            report.DebugSteps = debugSteps;
            PrintDebugSteps(debugSteps);

            // Extract the latest fixed code from debug steps
            var fixedCode = debugSteps
                .Where(d => d.Success && !string.IsNullOrWhiteSpace(d.FixedCode))
                .LastOrDefault()?.FixedCode ?? currentCode;

            // ── Step 4: Verify ──────────────────────────────────────
            Console.WriteLine("    Step 4: Verifying result...");
            var verification = await verifier.Verify(input.Code, fixedCode, plan, input.Request);
            report.Verification = verification;
            PrintVerification(verification);

            result.Reports.Add(report);

            if (verification.IsComplete)
            {
                Console.WriteLine($"\n Job complete after {i} step(s)!");
                result.Success = true;
                result.FinalCode = verification.FinalCode ?? fixedCode;
                result.FinalVerdict = verification.Verdict;
                result.TotalIterations = i;
                break;
            }

            currentCode = fixedCode;
            Console.WriteLine($"\n⚠  Remaining issues: {string.Join(", ", verification.RemainingIssues)}");
            Console.WriteLine("    Looping back for another step...");
        }

        if (!result.Success)
        {
            Console.WriteLine($"\n  Max steps ({maxSteps}) reached without full completion.");
            result.FinalCode = currentCode;
            result.FinalVerdict = "Max steps reached. Partial improvements may have been applied.";
            result.TotalIterations = maxSteps;
        }

        return result;
    }


    private static void PrintReview(AiReviewResponse review)
    {
        Console.WriteLine($"     Summary: {review.Summary}");
        Console.WriteLine($"     Severity: {review.SeverityScore}/10");
        if (review.Issues.Count != 0)
            Console.WriteLine($"     Issues: {string.Join(", ", review.Issues.Take(3))}{(review.Issues.Count > 3 ? "..." : "")}");
    }

    private static void PrintPlan(AiPlanResponse plan)
    {
        Console.WriteLine($"     Goal: {plan.Goal}");
        Console.WriteLine($"     Steps: {plan.Steps.Count} planned");
        foreach (var step in plan.Steps)
            Console.WriteLine($"       {step.Order}. {step.Description}");
    }

    private static void PrintDebugSteps(List<AiDebugResponse> steps)
    {
        foreach (var step in steps)
        {
            var icon = step.Success ? "✅" : "❌";
            Console.WriteLine($"     {icon} {step.Action}: {step.Explanation}");
        }
    }

    private static void PrintVerification(AiVerifyResponse verification)
    {
        var icon = verification.IsComplete ? "✅" : "⚠️";
        Console.WriteLine($"     {icon} {verification.Verdict}");
        if (verification.RemainingIssues.Count != 0)
            Console.WriteLine($"     Remaining: {string.Join(", ", verification.RemainingIssues)}");
    }
}

public class CodeDebuggerAgentExtended(ICodeDebuggerAgent inner)
{
    public Task<List<AiDebugResponse>> DebugWithPlan(string code, AiPlanResponse plan) =>
        inner.DebugWithPlan(code, plan);
}