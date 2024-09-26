using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using AIChatApp.Model;

namespace AIChatApp.Services;

internal class ChatService(IChatCompletionService chatService)
{
    internal async Task<Message> Chat(ChatRequest request)
    {
        ChatHistory history = CreateHistoryFromRequest(request);

        ChatMessageContent response = await chatService.GetChatMessageContentAsync(history);

        return new Message()
        {
            IsAssistant = response.Role == AuthorRole.Assistant,
            Content = (response.Items[0] as TextContent).Text
        };
    }

    internal async IAsyncEnumerable<string> Stream(ChatRequest request)
    {
        ChatHistory history = CreateHistoryFromRequest(request);

        IAsyncEnumerable<StreamingChatMessageContent> response = chatService.GetStreamingChatMessageContentsAsync(history);

        await foreach (StreamingChatMessageContent content in response)
        {
            yield return (content.Content);
        }
    }

    private static ChatHistory CreateHistoryFromRequest(ChatRequest request)
    {
        ChatHistory history = new ChatHistory("You are a helpful assistant.");
        foreach (Message message in request.Messages)
        {
            if (message.IsAssistant)
            {
                history.AddAssistantMessage(message.Content);
            }
            else
            {
                history.AddUserMessage(message.Content);
            }
        }

        return history;
    }
}