using BugFinderAgent.AIMiddlewares;
using BugFinderAgent.AITools;
using BugFinderAgent.Interfaces;
using BugFinderAgent.Models;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using OllamaSharp;

namespace BugFinderAgent.Services;

public class CodeDebuggerAgent : ICodeDebuggerAgent
{
    private readonly string instruction = @"You are a C# debugging assistant.

You MUST return valid JSON matching this schema exactly:

[
  {
    ""Title"": ""short title"",
    ""Explanation"": ""what happened"",
    ""CodeExecuted"": ""code executed"",
    ""Result"": ""execution result"",
    ""Success"": ""code corrected successfully?""
    ""FixedCode"": ""FULL corrected code""
  }
]

Rules:
- All Fields are REQUIRED
- Always include the FULL corrected code
- Never omit FixedCode and Success
- Never return markdown
- ALWAYS use tools to debug the fixed code
- NEVER OMIT Success
- Never return explanations outside JSON";

    public async Task<List<AiDebugResponse>> Debug(string code, string model = "qwen2.5-coder:7b")
    {
        var client = new OllamaApiClient("http://localhost:11434", model).AsAIAgent(instruction, tools: [
                AIFunctionFactory.Create(CodeDebuggingTools.ExecuteCSharpCode),
                AIFunctionFactory.Create(CodeDebuggingTools.ValidateCSharpCode)
            ]).AsBuilder()
            .Use(FunctionCallMiddleware.Call)
            .Build();

        var response = await client.RunAsync<List<AiDebugResponse>>(code);

        var result = response.Result;

        if (result == null || result.Count == 0)
            throw new Exception("Debugger returned no steps.");

        if (result.All(x => string.IsNullOrWhiteSpace(x.FixedCode)))
            throw new Exception("Debugger failed to generate fixed code.");

        return result;
    }

    public async Task<List<AiDebugResponse>> DebugWithPlan(string code, AiPlanResponse plan, string model = "qwen2.5-coder:7b")
    {
        var prompt = $"""
            ## Code to Fix
            ```csharp
            {code}
            ```

            ## Plan to Execute
            Goal: {plan.Goal}
            Steps:
            {string.Join("\n", plan.Steps.Select(s => $"  {s.Order}. {s.Description} → Expected: {s.ExpectedOutcome}"))}

            Execute this plan on the code above. Use the available tools to validate and test your changes.
            Return every action you took as structured debug steps.

            - You must follow the plan step by step and fixed the code as expected
            """;

        return await Debug(prompt, model);
    }
}
