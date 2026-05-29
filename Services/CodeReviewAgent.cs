using BugFinderAgent.Interfaces;
using BugFinderAgent.Models;

namespace BugFinderAgent.Services;

public class CodeReviewAgent(IOllamaAgent agent) : ICodeReviewAgent
{
    private readonly string instruction =
        @"You are a code reviewer. Analyze the provided code and return a structured review with: 
        a summary, a list of issues found, a list of suggestions, and a severity score from 1 (minor) to 10 (critical). 
        Your feedback will be passed to a planner and debugger agent.";

    public async Task<AiReviewResponse> Review(string code, string model = "deepseek-coder-v2:16b")
    {
        return await agent.GetResponseAsync<AiReviewResponse>(model, instruction, code);
    }
}
