using System;
using System.Collections.Generic;

public class MessagingEndpoint
{
    private MessagingHub? _messagingHub;
    private readonly Queue<IEntityMessage> _receivedMessages = new();

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
        _receivedMessages.Enqueue(message);
    }

    public void HandleMessages(Action<IEntityMessage> messageHandler)
    {
        while (_receivedMessages.TryDequeue(out var message))
        {
            messageHandler(message);
        }
    }
}

public interface IEntityMessage { }