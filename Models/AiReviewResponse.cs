namespace BugFinderAgent.Models;

public class AiReviewResponse
{
    public string Summary { get; set; } = string.Empty;
    public List<string> Issues { get; set; } = [];
    public List<string> Suggestions { get; set; } = [];
    public int SeverityScore { get; set; }
}
