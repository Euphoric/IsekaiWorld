using System.Collections.Concurrent;
using System.Collections.Generic;

public class MessagingHub
{
    private readonly ConcurrentQueue<IEntityMessage> _receivedMessages = new();
    private readonly List<MessagingEndpoint> _messageRecipients = new();
    private readonly List<MessagingHub> _connectedHubs = new();
    
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

    public void ConnectMessageHub(MessagingHub hub)
    {
        _connectedHubs.Add(hub);
    }

    public void Broadcast(IEntityMessage message)
    {
        _receivedMessages.Enqueue(message);
        
        foreach (var hub in _connectedHubs)
        {
            hub.Broadcast(message);
        }
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