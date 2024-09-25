using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using AIChatApp.Model;
using AIChatApp.Services;

namespace AIChatApp.Components.Chat;

public partial class Chat
{
    [Inject]
    internal ChatHandler? ChatHandler { get; init; }
    List<Message> messages = new();
    ElementReference writeMessageElement;
    string? userMessageText;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            try
            {
                await using var module = await JS.InvokeAsync<IJSObjectReference>("import", "./Components/Chat/Chat.razor.js");
                await module.InvokeVoidAsync("submitOnEnter", writeMessageElement);
            }
            catch (JSDisconnectedException)
            {
                // Not an error
            }
        }
    }

    async void SendMessage()
    {
        if (ChatHandler is null) { return; }

        if (!string.IsNullOrWhiteSpace(userMessageText))
        {
            // Add the user's message to the UI
            // TODO: Don't rely on "magic strings" for the Role
            messages.Add(new Message() {
                IsAssistant = false,
                Content = userMessageText
                });
                
            userMessageText = null;

            ChatRequest request = new ChatRequest(messages);
            string response = "";

            IAsyncEnumerable<string> chunks = ChatHandler.Stream(request);
            await foreach (var chunk in chunks)
            {
                response += chunk;
                StateHasChanged();
            }

            // Add the assistant's reply to the UI
            messages.Add(new Message() {
                IsAssistant = true,
                Content = response
                });            
        }
    }
}