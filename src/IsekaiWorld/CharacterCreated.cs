using System;

namespace IsekaiWorld;

public record CharacterCreated(string EntityId, String Label) : IEntityMessage;