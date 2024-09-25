using Azure.Identity;
using Microsoft.SemanticKernel;
using AIChatApp.Components;
using AIChatApp.Model;
using AIChatApp.Services;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Configure AI related features
builder.Services.AddKernel();

builder.Services.AddAzureOpenAIChatCompletion(builder.Configuration["AZURE_OPENAI_DEPLOYMENT"]!,
    builder.Configuration["AZURE_OPENAI_ENDPOINT"]!,
    new DefaultAzureCredential());

builder.Services.AddSingleton<ChatHandler>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseAntiforgery();

app.MapRazorComponents<App>()
   .AddInteractiveServerRenderMode();

// Configure APIs for chat related features
//app.MapPost("/chat", (ChatRequest request, ChatHandler chatHandler) => (chatHandler.); // Uncomment for a non-streaming response
app.MapPost("/chat/stream", (ChatRequest request, ChatHandler chatHandler) => chatHandler.Stream(request));

app.Run();