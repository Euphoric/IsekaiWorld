using System.Collections.Generic;

namespace IsekaiWorld;

record SurfaceChanged(IReadOnlyList<MapCell> MapCells) : IEntityMessage;