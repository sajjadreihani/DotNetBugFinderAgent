using BugFinderAgent.Components;
using BugFinderAgent.Interfaces;
using BugFinderAgent.Services;
using MudBlazor.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddScoped<IOllamaAgent, OllamaAgent>();
builder.Services.AddScoped<ICodeReviewAgent, CodeReviewAgent>();
builder.Services.AddScoped<ICodeDebuggerAgent, CodeDebuggerAgent>();
builder.Services.AddScoped<IPlannerAgent, PlannerAgent>();
builder.Services.AddScoped<CodeDebuggerAgent>();
builder.Services.AddScoped<CodeDebuggerAgentExtended>();
builder.Services.AddScoped<ICodeVerifierAgent, CodeVerifierAgent>();
builder.Services.AddScoped<IAgentOrchestrator, AgentOrchestrator>();
builder.Services.AddMudServices();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
