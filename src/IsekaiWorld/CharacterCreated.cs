using System;

public class CharacterCreated : IEntityMessage
{
    public CharacterCreated(string entityId)
    {
        EntityId = entityId;
    }
    
    public String EntityId { get; }
}