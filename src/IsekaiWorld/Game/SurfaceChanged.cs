using System.Collections.Generic;

namespace IsekaiWorld.Game;

public record SurfaceChanged(IReadOnlyList<MapCell> MapCells) : IEntityMessage;