using System.Collections.Generic;

public class MessagingHub
{
    private readonly Queue<IEntityMessage> _receivedMessages = new();
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
        _receivedMessages.Enqueue(message);
    }
    
    public void DistributeMessages()
    {
        while (_receivedMessages.TryDequeue(out var message))
        {
            foreach (var recipient in _messageRecipients)
            {
                recipient.HandleMessage(message);
            }
        }
    }
}