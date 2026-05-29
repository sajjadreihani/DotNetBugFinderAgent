using BugFinderAgent.Interfaces;
using BugFinderAgent.Models;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using OllamaSharp;

namespace BugFinderAgent.Services;

public class OllamaAgent : IOllamaAgent
{
    private readonly IConfiguration _configuration;

    public OllamaAgent(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    private string GetOllamaEndpoint()
    {
        return _configuration["Ollama:Endpoint"] ?? "http://localhost:11434";
    }

    public async Task<T> GetResponseAsync<T>(string model, string instruction, string prompt)
    {
        var endpoint = GetOllamaEndpoint();
        var client = new OllamaApiClient(endpoint, model).AsAIAgent(instruction);

        var response = await client.RunAsync<T>(prompt);

        return response.Result;
    }

    public async Task<AgentResponse> GetResponseAsync(string model, string instruction, string prompt)
    {
        var endpoint = GetOllamaEndpoint();
        var client = new OllamaApiClient(endpoint, model).AsAIAgent(instruction);

        return await client.RunAsync(prompt);
    }
}
