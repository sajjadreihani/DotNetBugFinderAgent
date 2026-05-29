using BugFinderAgent.Models;
using Microsoft.Agents.AI;

namespace BugFinderAgent.Interfaces;

public interface IOllamaAgent
{
    public Task<T> GetResponseAsync<T>(string model, string instruction, string prompt);
    public Task<AgentResponse> GetResponseAsync(string model, string instruction, string prompt);
}
