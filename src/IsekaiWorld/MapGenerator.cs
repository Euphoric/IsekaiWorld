using System.Collections.Generic;
using Godot;

public interface IMapGenerator
{
    (HexagonalMapEntity, List<BuildingEntity>) GenerateNewMap();
}

public class MapGenerator : IMapGenerator
{
    public (HexagonalMapEntity, List<BuildingEntity>) GenerateNewMap()
    {
        var map = new HexagonalMapEntity(32);
        
        var surfaceNoise = new Simplex.Noise(){Seed = 123};
        var rockWallNoise = new Simplex.Noise(){Seed = 654};
        
        List<BuildingEntity> buildings = new List<BuildingEntity>();
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

            var surface = isGrass ? SurfaceDefinitions.Grass : SurfaceDefinitions.Dirt;
            cell.Surface = surface;

            if (isRockWall)
            {
                buildings.Add(new BuildingEntity(cell.Position, BuildingDefinitions.RockWall));
            }
        }
        return (map, buildings);
    }
}

public class EmptyMapGenerator : IMapGenerator
{
    public (HexagonalMapEntity, List<BuildingEntity>) GenerateNewMap()
    {
        var map = new HexagonalMapEntity(8);

        foreach (var cell in map.Cells)
        {
            cell.Surface = SurfaceDefinitions.Dirt;
        }
        
        List<BuildingEntity> buildings = new List<BuildingEntity>();
        return (map, buildings);
    }
}

public class WallTilingTestMapGenerator : IMapGenerator
{
    public (HexagonalMapEntity, List<BuildingEntity>) GenerateNewMap()
    {
        var map = new HexagonalMapEntity(16);

        foreach (var cell in map.Cells)
        {
            cell.Surface = SurfaceDefinitions.Grass;
        }
        
        List<BuildingEntity> buildings = new List<BuildingEntity>();
        
        buildings.Add(new BuildingEntity(new HexCubeCoord(3, 3, -6), BuildingDefinitions.StoneWall));
        buildings.Add(new BuildingEntity(new HexCubeCoord(3, 2, -5), BuildingDefinitions.StoneWall));
        buildings.Add(new BuildingEntity(new HexCubeCoord(3, 1, -4), BuildingDefinitions.StoneWall));
        
        buildings.Add(new BuildingEntity(new HexCubeCoord(-3, 3, 0), BuildingDefinitions.StoneWall));
        buildings.Add(new BuildingEntity(new HexCubeCoord(-2, 3, -1), BuildingDefinitions.StoneWall));
        buildings.Add(new BuildingEntity(new HexCubeCoord(-1, 3, -2), BuildingDefinitions.StoneWall));
        
        buildings.Add(new BuildingEntity(new HexCubeCoord(-3, 0, 3), BuildingDefinitions.StoneWall));
        buildings.Add(new BuildingEntity(new HexCubeCoord(-2, -1, 3), BuildingDefinitions.StoneWall));
        buildings.Add(new BuildingEntity(new HexCubeCoord(-1, -2, 3), BuildingDefinitions.StoneWall));
        
        buildings.Add(new BuildingEntity(new HexCubeCoord(1, -4, 3), BuildingDefinitions.StoneWall));
        buildings.Add(new BuildingEntity(new HexCubeCoord(2, -5, 3), BuildingDefinitions.StoneWall));
        buildings.Add(new BuildingEntity(new HexCubeCoord(1, -5, 4), BuildingDefinitions.StoneWall));
        
        buildings.Add(new BuildingEntity(new HexCubeCoord(4, -3, -1), BuildingDefinitions.StoneWall));
        buildings.Add(new BuildingEntity(new HexCubeCoord(5, -4, -1), BuildingDefinitions.StoneWall));
        buildings.Add(new BuildingEntity(new HexCubeCoord(5, -3, -2), BuildingDefinitions.StoneWall));
        
        buildings.Add(new BuildingEntity(new HexCubeCoord(-7, 3, 4), BuildingDefinitions.StoneWall));
        buildings.Add(new BuildingEntity(new HexCubeCoord(-6, 2, 4), BuildingDefinitions.StoneWall));
        buildings.Add(new BuildingEntity(new HexCubeCoord(-8, 3, 5), BuildingDefinitions.StoneWall));
        buildings.Add(new BuildingEntity(new HexCubeCoord(-7, 4, 3), BuildingDefinitions.StoneWall));
        
        buildings.Add(new BuildingEntity(new HexCubeCoord(-7, 7, 0), BuildingDefinitions.StoneWall));
        buildings.Add(new BuildingEntity(new HexCubeCoord(-7, 6, 1), BuildingDefinitions.StoneWall));
        buildings.Add(new BuildingEntity(new HexCubeCoord(-6, 7, -1), BuildingDefinitions.StoneWall));
        buildings.Add(new BuildingEntity(new HexCubeCoord(-8, 8, 0), BuildingDefinitions.StoneWall));
        
        return (map, buildings);
    }
}