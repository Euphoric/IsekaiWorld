using Godot;

public interface IMapGenerator
{
    HexMap GenerateNewMap();
}

public class MapGenerator : IMapGenerator
{
    public HexMap GenerateNewMap()
    {
        var map = new HexMap(32);
        
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
    public HexMap GenerateNewMap()
    {
        var map = new HexMap(8);

        foreach (var cell in map.Cells)
        {
            cell.Surface = SurfaceDefinitions.Dirt;
        }
        
        return map;
    }
}