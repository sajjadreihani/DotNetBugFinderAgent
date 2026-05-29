using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System.ComponentModel;
using System.Reflection;
using System.Text;

namespace BugFinderAgent.AITools;

public static class CodeDebuggingTools
{
    [Description("Validates C# code syntax and returns any compilation errors without executing it.")]
    public static string ValidateCSharpCode(
        [Description("The C# code to validate")] string code)
    {
        var syntaxTree = CSharpSyntaxTree.ParseText(code);
        var references = AppDomain.CurrentDomain.GetAssemblies()
            .Where(a => !a.IsDynamic && !string.IsNullOrEmpty(a.Location))
            .Select(a => MetadataReference.CreateFromFile(a.Location))
            .Cast<MetadataReference>()
            .ToList();

        var compilation = CSharpCompilation.Create(
            "ValidationAssembly",
            [syntaxTree],
            references,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        var diagnostics = compilation.GetDiagnostics()
            .Where(d => d.Severity == DiagnosticSeverity.Error)
            .ToList();

        if (!diagnostics.Any())
            return "Code is valid. No syntax errors found.";

        var sb = new StringBuilder();
        sb.AppendLine($"Found {diagnostics.Count} error(s):");
        foreach (var diag in diagnostics)
        {
            var line = diag.Location.GetLineSpan();
            sb.AppendLine($"  Line {line.StartLinePosition.Line + 1}: {diag.GetMessage()}");
        }
        return sb.ToString();
    }

    [Description("Executes C# code using Roslyn scripting and returns the output or any runtime errors.")]
    public static async Task<string> ExecuteCSharpCode(
        [Description("The complete C# code to execute")] string code)
    {
        try
        {
            var scriptCode = code.Contains("class ") || code.Contains("namespace ")
                ? code
                : WrapInScript(code);

            var syntaxTree = CSharpSyntaxTree.ParseText(scriptCode);
            var references = GetReferences();

            var compilation = CSharpCompilation.Create(
                $"DynamicAssembly_{Guid.NewGuid():N}",
                [syntaxTree],
                references,
                new CSharpCompilationOptions(OutputKind.ConsoleApplication));

            using var ms = new MemoryStream();
            var result = compilation.Emit(ms);

            if (!result.Success)
            {
                var errors = result.Diagnostics
                    .Where(d => d.Severity == DiagnosticSeverity.Error)
                    .Select(d => $"  {d.GetMessage()}");
                return $"Compilation failed:\n{string.Join("\n", errors)}";
            }

            ms.Seek(0, SeekOrigin.Begin);
            var assembly = Assembly.Load(ms.ToArray());

            var output = new StringBuilder();
            var originalOut = Console.Out;
            Console.SetOut(new StringWriter(output));

            try
            {
                var entryPoint = assembly.EntryPoint;
                entryPoint?.Invoke(null, entryPoint.GetParameters().Length == 0
                    ? null
                    : new object[] { Array.Empty<string>() });
            }
            finally
            {
                Console.SetOut(originalOut);
            }

            return $"Execution successful:\n{output}";
        }
        catch (Exception ex)
        {
            return $"Runtime error: {ex.Message}";
        }
    }

    private static string WrapInScript(string code) => $@"""
        using System;
        using System.Collections.Generic;
        using System.Linq;

        class Program
        {{
            static void Main()
            {{
                {code}
            }}
        }}
        """;

    private static List<MetadataReference> GetReferences() =>
        [.. AppDomain.CurrentDomain.GetAssemblies()
            .Where(a => !a.IsDynamic && !string.IsNullOrEmpty(a.Location))
            .Select(a => MetadataReference.CreateFromFile(a.Location))
            .Cast<MetadataReference>()];
}
