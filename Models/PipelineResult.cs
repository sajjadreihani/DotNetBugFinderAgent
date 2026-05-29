namespace BugFinderAgent.Models;

public record PipelineResult(bool Success, string? FinalCode, string? OriginalRequest, int TotalIterations)
{
    public string OriginalCode { get; set; } = string.Empty;
    public List<StepReport> Reports { get; set; } = [];
    public string FinalVerdict { get; set; } = string.Empty;
}

public class StepReport
{
    public int Step { get; set; }
    public AiReviewResponse? Review { get; set; }
    public AiPlanResponse? Plan { get; set; }
    public List<AiDebugResponse>? DebugSteps { get; set; }
    public AiVerifyResponse? Verification { get; set; }
}
