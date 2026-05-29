namespace BugFinderAgent.Models;

public class AiDebugResponse
{
    public string Action { get; set; } = string.Empty;
    public string CodeExecuted { get; set; } = string.Empty;
    public string Output { get; set; } = string.Empty;
    public bool Success { get; set; }
    public required string FixedCode { get; set; }
    public string Explanation { get; set; } = string.Empty;
}
