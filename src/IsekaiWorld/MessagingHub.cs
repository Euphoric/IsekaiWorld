using System.Collections.Generic;

public class MessagingHub
{
    private readonly List<MessagingEndpoint> _messageRecipients = new();

    public void Register(MessagingEndpoint messaging)
    {
        _messageRecipients.Add(messaging);
        messaging.RegisterHub(this);
    }

    public void Unregister(MessagingEndpoint messaging)
    {
        _messageRecipients.Remove(messaging);
        messaging.UnregisterHub(this);
    }

    public void Broadcast(IEntityMessage message)
    {
        foreach (var messageRecipient in _messageRecipients)
        {
            messageRecipient.Receive(message);
        }
    }
}