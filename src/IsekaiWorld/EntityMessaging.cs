using System;
using System.Collections.Generic;

public class EntityMessaging
{
    private MessagingHub? _messagingHub;
    private readonly List<IEntityMessage> _receivedMessages = new();

    public void Broadcast(IEntityMessage message)
    {
        _messagingHub?.Broadcast(message);
    }

    public void RegisterHub(MessagingHub messagingHub)
    {
        if (_messagingHub != null)
        {
            throw new InvalidOperationException("Cannot register new messaging hub");
        }

        _messagingHub = messagingHub;
    }

    public void UnregisterHub(MessagingHub messagingHub)
    {
        if (_messagingHub != messagingHub)
        {
            throw new InvalidOperationException("Cannot unregister different messaging hub");
        }

        _messagingHub = null;
    }

    public void Receive(IEntityMessage message)
    {
        _receivedMessages.Add(message);
    }

    public void HandleMessages(Action<IEntityMessage> messageHandler)
    {
        foreach (var message in _receivedMessages)
        {
            messageHandler(message);
        }
        _receivedMessages.Clear();
    }
}

public interface IEntityMessage { }