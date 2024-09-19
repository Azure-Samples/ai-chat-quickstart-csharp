using Azure;
using Azure.Identity;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using OllamaSharp.Models;

public record ChatRequest(List<Message> Messages, MessageContext? Context)
{}

public record Message
{
    public string Role
    {
        get; set;
    }

    public string Content
    {
        get; set;
    }
}

public record MessageContext
{
    public string ContentType { get; set; }

    public string ImageURI
    {
        get; set;
    }
}

internal class ChatHandler
{
    internal async Task<Message> Chat(List<Message> messages, IChatCompletionService chatService)
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

    internal async IAsyncEnumerable<string> Stream(ChatRequest request, IChatCompletionService chatService)
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