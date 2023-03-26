using System.Collections.Generic;

record SurfaceChanged(IReadOnlyList<MapCell> MapCells) : IEntityMessage;