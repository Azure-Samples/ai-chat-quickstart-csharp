using System;

namespace AIChatApp.Model;

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
