using System;

namespace IsekaiWorld.Game;

public record CharacterCreated(string EntityId, String Label) : IEntityMessage;