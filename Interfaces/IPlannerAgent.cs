using BugFinderAgent.Models;

namespace BugFinderAgent.Interfaces;

public interface IPlannerAgent
{
    Task<AiPlanResponse> CreatePlan(string code, AiReviewResponse review, string? userRequest, string model = "deepseek-coder-v2:16b");
}
