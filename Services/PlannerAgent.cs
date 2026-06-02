using BugFinderAgent.Interfaces;
using BugFinderAgent.Models;

namespace BugFinderAgent.Services;

public class PlannerAgent(IOllamaAgent agent) : IPlannerAgent
{
    private readonly string instruction =
        @"You are a software engineering planner. Given a code review and an optional user request, 
        produce a clear, ordered action plan. Each step should have a description and an expected outcome. 
        Be specific about what code changes need to happen. Your plan will be executed by a debugger agent.
The Result Should Follow below JSON:
{
 'Goal': 'string',
 'Steps' : [{
        'Order': 'int',
        'Description': 'string',
        'ExpectedOutcome': 'string'
}]
}
";

    public async Task<AiPlanResponse> CreatePlan(string code, AiReviewResponse review, string? userRequest, string model = "deepseek-coder-v2:16b")
    {
        var prompt = $"""
            ## Original Code
            ```csharp
            {code}
            ```

            ## Code Review
            Summary: {review.Summary}
            Issues:
            {string.Join("\n", review.Issues.Select((i, idx) => $"  {idx + 1}. {i}"))}
            Suggestions:
            {string.Join("\n", review.Suggestions.Select((s, idx) => $"  {idx + 1}. {s}"))}
            Severity Score: {review.SeverityScore}/10

            ## User Request (optional)
            {(string.IsNullOrWhiteSpace(userRequest) ? "No specific request. Fix all issues found in the review." : userRequest)}

            Create a step-by-step plan to address the issues and fulfill the user request.
            """;

        return await agent.GetResponseAsync<AiPlanResponse>(model, instruction, prompt);
    }
}
