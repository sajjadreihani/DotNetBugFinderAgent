namespace BugFinderAgent.Models;

public class AiVerifyResponse
{
    public bool IsComplete { get; set; }
    public string Verdict { get; set; } = string.Empty;
    public List<string> RemainingIssues { get; set; } = [];
    public string? FinalCode { get; set; }
}
