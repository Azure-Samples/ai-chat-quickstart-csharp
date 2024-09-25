using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using AIChatApp.Model;

namespace AIChatApp.Services;

internal class ChatHandler(IChatCompletionService chatService)
{
    internal async Task<Message> Chat(List<Message> messages)
    {
        ChatHistory history = new ChatHistory("You are a helpful assistant.");
        foreach (Message message in messages)
        {
            history.AddMessage(new AuthorRole(message.Role), message.Content);
        }

        ChatMessageContent response = await chatService.GetChatMessageContentAsync(history);

        return new Message()
        {
            Role = response.Role.ToString(),
            Content = (response.Items[0] as TextContent).Text
        };
    }

    internal async IAsyncEnumerable<string> Stream(ChatRequest request)
    {
        ChatHistory history = new ChatHistory("You are a helpful assistant.");
        foreach (Message message in request.Messages)
        {
            history.AddMessage(new AuthorRole(message.Role), message.Content);
        }   

        if (request.Context != null && !String.IsNullOrEmpty(request.Context.ImageURI))
        {
            history.AddUserMessage([new ImageContent(dataUri: request.Context.ImageURI)]);
        }

        IAsyncEnumerable<StreamingChatMessageContent> response = chatService.GetStreamingChatMessageContentsAsync(history);

        await foreach (StreamingChatMessageContent content in response)
        {
            yield return (content.Content);
        }
    }
}