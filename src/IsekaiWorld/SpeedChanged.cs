namespace IsekaiWorld;

public record SpeedChanged(bool Paused, int Speed) : IEntityMessage;