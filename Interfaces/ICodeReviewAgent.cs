using BugFinderAgent.Models;

namespace BugFinderAgent.Interfaces;

public interface ICodeReviewAgent
{
    Task<AiReviewResponse> Review(string code, string model = "deepseek-coder-v2:16b");
}
