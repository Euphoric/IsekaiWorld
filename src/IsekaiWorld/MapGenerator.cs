using Godot;

public interface IMapGenerator
{
    HexagonalMapEntity GenerateNewMap();
}

public class MapGenerator : IMapGenerator
{
    public HexagonalMapEntity GenerateNewMap()
    {
        var map = new HexagonalMapEntity(32);
        
        var surfaceNoise = new Simplex.Noise(){Seed = 123};
        var rockWallNoise = new Simplex.Noise(){Seed = 654};
        
        foreach (var cell in map.Cells)
        {
            var center = cell.Position.Center(1000);
            var isRockWall = rockWallNoise.CalcPixel2D(
                Mathf.CeilToInt(center.x),
                Mathf.CeilToInt(center.y), 1 / 1000f * 0.04f) < -0.5;
            
            var isGrass = surfaceNoise.CalcPixel2D(
                              Mathf.CeilToInt(center.x),
                              Mathf.CeilToInt(center.y), 1 / 1000f * 0.04f) <
                          0;
                          
            var surface = isRockWall ? SurfaceDefinitions.RockWall : isGrass ? SurfaceDefinitions.Grass : SurfaceDefinitions.Dirt;
            cell.Surface = surface;
        }
        
        return map;
    }
}

public class EmptyMapGenerator : IMapGenerator
{
    public HexagonalMapEntity GenerateNewMap()
    {
        var map = new HexagonalMapEntity(8);

        foreach (var cell in map.Cells)
        {
            cell.Surface = SurfaceDefinitions.Dirt;
        }
        
        return map;
    }
}