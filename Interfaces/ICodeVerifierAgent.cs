using BugFinderAgent.Models;

namespace BugFinderAgent.Interfaces;

public interface ICodeVerifierAgent
{
    Task<AiVerifyResponse> Verify(string originalCode, string modifiedCode, AiPlanResponse plan, string? userRequest, string model = "deepseek-coder-v2:16b");
}
