using System;
using Azure.Core;
using Azure.Identity;
using Microsoft.SemanticKernel;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

builder.Services.AddKernel();

// Azure OpenAI
builder.Services.AddAzureOpenAIChatCompletion(Environment.GetEnvironmentVariable("AZURE_OPENAI_DEPLOYMENT"),
    Environment.GetEnvironmentVariable("AZURE_OPENAI_ENDPOINT"),
    new DefaultAzureCredential());

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

var chatHandler = new ChatHandler();

app.MapPost("/chat", chatHandler.Chat);
app.MapPost("/chat/stream", chatHandler.Stream);

app.Run();