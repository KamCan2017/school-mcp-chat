using School.Web.Chat;
using School.Web.Components;
using School.Web.Mcp;
using School.Web.ViewModels;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<McpOptions>(builder.Configuration.GetSection(McpOptions.SectionName));
builder.Services.Configure<AiOptions>(builder.Configuration.GetSection(AiOptions.SectionName));
builder.Services.AddSingleton<SchoolMcpClient>();
builder.Services.AddSingleton<ISchoolMcpClient>(sp => sp.GetRequiredService<SchoolMcpClient>());
builder.Services.AddSingleton<IHostedService>(sp => sp.GetRequiredService<SchoolMcpClient>());
builder.Services.AddSingleton<SchoolQueryPlanner>();
builder.Services.AddHttpClient<McpResponseSummarizer>();
builder.Services.AddScoped<HomeViewModel>();
builder.Services.AddScoped<ToolsViewModel>();
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
