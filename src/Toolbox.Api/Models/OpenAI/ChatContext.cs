namespace Toolbox.Api.Models.OpenAI;
public class ChatContext
{
    public string Prompt { get; set; }
    public List<ChatMessage>? ChatMessages { get; set; }

    public ChatContext(string prompt, List<ChatMessage>? chatMessages)
    {
        Prompt = prompt;
        ChatMessages = chatMessages;
    }
}

public class ChatMessage
{
    public string Role { get; set; }

    public string Content { get; set; }

    public ChatMessage(string role, string content)
    {
        Role = role;
        Content = content;
    }
}