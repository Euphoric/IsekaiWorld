using Godot;

public interface IMapGenerator
{
    HexagonalMapEntity GenerateNewMap();
}

public class MapGenerator : IMapGenerator
{
    public HexagonalMapEntity GenerateNewMap()
    {
        var map = new HexagonalMapEntity(64);
        
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

public class WallTilingTestMapGenerator : IMapGenerator
{
    public HexagonalMapEntity GenerateNewMap()
    {
        var map = new HexagonalMapEntity(16);

        foreach (var cell in map.Cells)
        {
            cell.Surface = SurfaceDefinitions.Grass;
        }
        
        map.CellForPosition(new HexCubeCoord(3, 3, -6)).Surface = SurfaceDefinitions.StoneWall;
        map.CellForPosition(new HexCubeCoord(3, 2, -5)).Surface = SurfaceDefinitions.StoneWall;
        map.CellForPosition(new HexCubeCoord(3, 1, -4)).Surface = SurfaceDefinitions.StoneWall;
        
        map.CellForPosition(new HexCubeCoord(-3, 3, 0)).Surface = SurfaceDefinitions.StoneWall;
        map.CellForPosition(new HexCubeCoord(-2, 3, -1)).Surface = SurfaceDefinitions.StoneWall;
        map.CellForPosition(new HexCubeCoord(-1, 3, -2)).Surface = SurfaceDefinitions.StoneWall;
        
        map.CellForPosition(new HexCubeCoord(-3, 0, 3)).Surface = SurfaceDefinitions.StoneWall;
        map.CellForPosition(new HexCubeCoord(-2, -1, 3)).Surface = SurfaceDefinitions.StoneWall;
        map.CellForPosition(new HexCubeCoord(-1, -2, 3)).Surface = SurfaceDefinitions.StoneWall;
        
        map.CellForPosition(new HexCubeCoord(1, -4, 3)).Surface = SurfaceDefinitions.StoneWall;
        map.CellForPosition(new HexCubeCoord(2, -5, 3)).Surface = SurfaceDefinitions.StoneWall;
        map.CellForPosition(new HexCubeCoord(1, -5, 4)).Surface = SurfaceDefinitions.StoneWall;
        
        map.CellForPosition(new HexCubeCoord(4, -3, -1)).Surface = SurfaceDefinitions.StoneWall;
        map.CellForPosition(new HexCubeCoord(5, -4, -1)).Surface = SurfaceDefinitions.StoneWall;
        map.CellForPosition(new HexCubeCoord(5, -3, -2)).Surface = SurfaceDefinitions.StoneWall;
        
        map.CellForPosition(new HexCubeCoord(-7, 3, 4)).Surface = SurfaceDefinitions.StoneWall;
        map.CellForPosition(new HexCubeCoord(-6, 2, 4)).Surface = SurfaceDefinitions.StoneWall;
        map.CellForPosition(new HexCubeCoord(-8, 3, 5)).Surface = SurfaceDefinitions.StoneWall;
        map.CellForPosition(new HexCubeCoord(-7, 4, 3)).Surface = SurfaceDefinitions.StoneWall;
        
        map.CellForPosition(new HexCubeCoord(-7, 7, 0)).Surface = SurfaceDefinitions.StoneWall;
        map.CellForPosition(new HexCubeCoord(-7, 6, 1)).Surface = SurfaceDefinitions.StoneWall;
        map.CellForPosition(new HexCubeCoord(-6, 7, -1)).Surface = SurfaceDefinitions.StoneWall;
        map.CellForPosition(new HexCubeCoord(-8, 8, 0)).Surface = SurfaceDefinitions.StoneWall;
        
        return map;
    }
}