using BugFinderAgent.Models;

namespace BugFinderAgent.Interfaces;

public interface IAgentOrchestrator
{
    Task<PipelineResult> RunAsync(AgentInput input, int maxIterations = 5);
}
