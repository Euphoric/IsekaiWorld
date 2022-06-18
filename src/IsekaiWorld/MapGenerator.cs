using System;
using System.Collections.Generic;
using System.Linq;
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
                buildings.Add(new BuildingEntity(cell.Position, HexagonDirection.Left, BuildingDefinitions.RockWall));
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
        
        buildings.Add(new BuildingEntity(new HexCubeCoord(3, 3, -6), HexagonDirection.Left, BuildingDefinitions.StoneWall));
        buildings.Add(new BuildingEntity(new HexCubeCoord(3, 2, -5), HexagonDirection.Left, BuildingDefinitions.StoneWall));
        buildings.Add(new BuildingEntity(new HexCubeCoord(3, 1, -4), HexagonDirection.Left,  BuildingDefinitions.StoneWall));
        
        buildings.Add(new BuildingEntity(new HexCubeCoord(-3, 3, 0), HexagonDirection.Left, BuildingDefinitions.StoneWall));
        buildings.Add(new BuildingEntity(new HexCubeCoord(-2, 3, -1),HexagonDirection.Left,  BuildingDefinitions.StoneWall));
        buildings.Add(new BuildingEntity(new HexCubeCoord(-1, 3, -2),HexagonDirection.Left,  BuildingDefinitions.StoneWall));
        
        buildings.Add(new BuildingEntity(new HexCubeCoord(-3, 0, 3), HexagonDirection.Left, BuildingDefinitions.StoneWall));
        buildings.Add(new BuildingEntity(new HexCubeCoord(-2, -1, 3),HexagonDirection.Left,  BuildingDefinitions.StoneWall));
        buildings.Add(new BuildingEntity(new HexCubeCoord(-1, -2, 3),HexagonDirection.Left,  BuildingDefinitions.StoneWall));
        
        buildings.Add(new BuildingEntity(new HexCubeCoord(1, -4, 3),HexagonDirection.Left,  BuildingDefinitions.StoneWall));
        buildings.Add(new BuildingEntity(new HexCubeCoord(2, -5, 3),HexagonDirection.Left,  BuildingDefinitions.StoneWall));
        buildings.Add(new BuildingEntity(new HexCubeCoord(1, -5, 4),HexagonDirection.Left, BuildingDefinitions.StoneWall));
        
        buildings.Add(new BuildingEntity(new HexCubeCoord(4, -3, -1),HexagonDirection.Left,  BuildingDefinitions.StoneWall));
        buildings.Add(new BuildingEntity(new HexCubeCoord(5, -4, -1),HexagonDirection.Left,  BuildingDefinitions.StoneWall));
        buildings.Add(new BuildingEntity(new HexCubeCoord(5, -3, -2),HexagonDirection.Left,  BuildingDefinitions.StoneWall));
        
        buildings.Add(new BuildingEntity(new HexCubeCoord(-7, 3, 4),HexagonDirection.Left, BuildingDefinitions.StoneWall));
        buildings.Add(new BuildingEntity(new HexCubeCoord(-6, 2, 4),HexagonDirection.Left,  BuildingDefinitions.StoneWall));
        buildings.Add(new BuildingEntity(new HexCubeCoord(-8, 3, 5),HexagonDirection.Left,  BuildingDefinitions.StoneWall));
        buildings.Add(new BuildingEntity(new HexCubeCoord(-7, 4, 3),HexagonDirection.Left,  BuildingDefinitions.StoneWall));
        
        buildings.Add(new BuildingEntity(new HexCubeCoord(-7, 7, 0), HexagonDirection.Left,  BuildingDefinitions.StoneWall));
        buildings.Add(new BuildingEntity(new HexCubeCoord(-7, 6, 1), HexagonDirection.Left, BuildingDefinitions.StoneWall));
        buildings.Add(new BuildingEntity(new HexCubeCoord(-6, 7, -1),HexagonDirection.Left,  BuildingDefinitions.StoneWall));
        buildings.Add(new BuildingEntity(new HexCubeCoord(-8, 8, 0), HexagonDirection.Left,  BuildingDefinitions.StoneWall));
        
        return (map, buildings);
    }
}

public class ConstructionTestMapGenerator: IMapGenerator
{
    public (HexagonalMapEntity, List<BuildingEntity>) GenerateNewMap()
    {
        var map = new HexagonalMapEntity(16);

        foreach (var cell in map.Cells)
        {
            cell.Surface = SurfaceDefinitions.Grass;
        }
        
        List<BuildingEntity> buildings = new List<BuildingEntity>();

        foreach (HexagonDirection direction in Enum.GetValues(typeof(HexagonDirection)).Cast<HexagonDirection>())
        {
            var chairPos = HexCubeCoord.Zero + direction;
            buildings.Add(new BuildingEntity(chairPos, direction, BuildingDefinitions.WoodenChair));

            var bedPos = chairPos + direction;
            buildings.Add(new BuildingEntity(bedPos, direction, BuildingDefinitions.WoodenBed));
            
            var stovePos = bedPos + direction + direction;
            buildings.Add(new BuildingEntity(stovePos, direction, BuildingDefinitions.TableStoveFueled));
        }
        
        return (map, buildings);
    }
}