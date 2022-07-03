using System;
using System.Collections.Generic;

public class EntityMessaging
{
    private readonly List<IEntityMessage> _broadcastMessages = new List<IEntityMessage>();
    public IReadOnlyList<IEntityMessage> BroadcastMessages => _broadcastMessages;

    private readonly Dictionary<Type, Action<IEntityMessage>> _handlers = new Dictionary<Type, Action<IEntityMessage>>();

    public void Broadcast(IEntityMessage message)
    {
        _broadcastMessages.Add(message);
    }

    public void Register<TMessage>(Action<TMessage> messageHandler)
        where TMessage : IEntityMessage
    {
        _handlers[typeof(TMessage)] = obj => messageHandler((TMessage)obj);
    }

    public void ClearBroadcast()
    {
        _broadcastMessages.Clear();
    }

    public void Handle(IEntityMessage message)
    {
        if (_handlers.TryGetValue(message.GetType(), out var action))
        {
            action(message);
        }
    }
}

public interface IEntityMessage { }