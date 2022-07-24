using System.Collections.Generic;

public class MessagingHub
{
    private readonly List<EntityMessaging> _messageRecipients = new List<EntityMessaging>();

    public void Register(EntityMessaging messaging)
    {
        _messageRecipients.Add(messaging);
        messaging.RegisterHub(this);
    }

    public void Unregister(EntityMessaging messaging)
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