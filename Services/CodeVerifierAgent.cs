using BugFinderAgent.AIMiddlewares;
using BugFinderAgent.AITools;
using BugFinderAgent.Interfaces;
using BugFinderAgent.Models;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using OllamaSharp;

namespace BugFinderAgent.Services;

public class CodeVerifierAgent(IOllamaAgent agent) : ICodeVerifierAgent
{
    private readonly string instruction =
        @"You are a code verification specialist. Your job is to assess whether a modified version of code 
        successfully fulfills a given plan and user request. Be strict but fair. 
        If there are remaining issues, list them clearly so the next iteration can address them.";

    public async Task<AiVerifyResponse> Verify(string originalCode, string modifiedCode, AiPlanResponse plan, string? userRequest, string model = "deepseek-coder-v2:16b")
    {
        var prompt = $"""
            ## Original Code
            ```csharp
            {originalCode}
            ```

            ## Modified Code
            ```csharp
            {modifiedCode}
            ```

            ## Plan That Was Executed
            Goal: {plan.Goal}
            Steps:
            {string.Join("\n", plan.Steps.Select(s => $"  {s.Order}. {s.Description} → Expected: {s.ExpectedOutcome}"))}

            ## User Request
            {(string.IsNullOrWhiteSpace(userRequest) ? "No specific request. Verify all review issues are resolved." : userRequest)}

            Assess:
            1. Is the plan fully executed?
            2. Does the modified code satisfy the user request?
            3. Are there any remaining issues?
            4. Is the job complete?

            Return the final code in FinalCode if complete, or null if further iteration is needed.
            """;

            var client = new OllamaApiClient("http://localhost:11434", model).AsAIAgent(instruction, tools: [
                AIFunctionFactory.Create(CodeDebuggingTools.ExecuteCSharpCode),
                AIFunctionFactory.Create(CodeDebuggingTools.ValidateCSharpCode)
            ]).AsBuilder()
            .Use(FunctionCallMiddleware.Call)
            .Build();

        var respones = await client.RunAsync<AiVerifyResponse>(prompt);

        return respones.Result;
    }
}
