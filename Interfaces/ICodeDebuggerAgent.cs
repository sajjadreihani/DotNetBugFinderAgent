using BugFinderAgent.Models;

namespace BugFinderAgent.Interfaces;

public interface ICodeDebuggerAgent
{
    Task<List<AiDebugResponse>> Debug(string code, string model = "qwen2.5-coder:7b");
    Task<List<AiDebugResponse>> DebugWithPlan(string code, AiPlanResponse plan, string model = "qwen2.5-coder:7b");
}
