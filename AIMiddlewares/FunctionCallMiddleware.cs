using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;

namespace BugFinderAgent.AIMiddlewares;

public static class FunctionCallMiddleware
{
    public static async ValueTask<object?> Call(AIAgent agent, FunctionInvocationContext context, Func<FunctionInvocationContext, CancellationToken, ValueTask<object>> next, CancellationToken cancellationToken)
    {
        Console.WriteLine($"    -- Tool : {context.Function.Name}");
        if (context.Arguments.Count > 0)
        {
            foreach (var arg in context.Arguments)
            {
                Console.WriteLine($"        --- args : {arg.Key} : {arg.Value}");
            }
        }

        return await next(context, cancellationToken);
    }

}
