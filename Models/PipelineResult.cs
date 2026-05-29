namespace BugFinderAgent.Models;

public class PipelineResult
{
    public bool Success { get; set; }
    public string OriginalCode { get; set; } = string.Empty;
    public string? FinalCode { get; set; }
    public string? OriginalRequest { get; set; }
    public List<StepReport> Reports { get; set; } = [];
    public int TotalIterations { get; set; }
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
