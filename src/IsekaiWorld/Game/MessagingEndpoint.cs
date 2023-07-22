using System;

namespace IsekaiWorld.Game;

public class MessagingEndpoint
{
    private readonly Action<IEntityMessage> _messageHandler;
    private MessagingHub? _messagingHub;

    private static void NullHandler(IEntityMessage msg)
    {
    }

    public MessagingEndpoint()
        : this(NullHandler)
    {
    }

    public MessagingEndpoint(Action<IEntityMessage> messageHandler)
    {
        _messageHandler = messageHandler;
    }

    public void HandleMessage(IEntityMessage message)
    {
        _messageHandler(message);
    }

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
}

public interface IEntityMessage
{
}