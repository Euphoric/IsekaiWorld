using System;

public record CharacterCreated(string EntityId, String Label) : IEntityMessage;