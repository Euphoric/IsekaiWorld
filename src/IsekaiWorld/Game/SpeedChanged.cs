namespace IsekaiWorld.Game;

public record SpeedChanged(bool Paused, int Speed) : IEntityMessage;