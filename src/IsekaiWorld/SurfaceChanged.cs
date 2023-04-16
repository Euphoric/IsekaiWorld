using System.Collections.Generic;

namespace IsekaiWorld;

public record SurfaceChanged(IReadOnlyList<MapCell> MapCells) : IEntityMessage;