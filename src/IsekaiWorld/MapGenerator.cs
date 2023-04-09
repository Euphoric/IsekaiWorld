using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

public interface IMapGenerator
{
    (HexagonalMapEntity, List<IEntity>) GenerateNewMap();
}

public class MapGenerator : IMapGenerator
{
    public (HexagonalMapEntity, List<IEntity>) GenerateNewMap()
    {
        var map = new HexagonalMapEntity(32);

        var surfaceNoise = new Simplex.Noise() { Seed = 123 };
        var rockWallNoise = new Simplex.Noise() { Seed = 654 };

        List<IEntity> entities = new List<IEntity>();
        foreach (var cell in map.Cells)
        {
            var center = cell.Position.Center(1000);
            var isRockWall = rockWallNoise.CalcPixel2D(
                Mathf.CeilToInt(center.X),
                Mathf.CeilToInt(center.Y), 1 / 1000f * 0.04f) < -0.5;

            var isGrass =
                surfaceNoise.CalcPixel2D(Mathf.CeilToInt(center.X), Mathf.CeilToInt(center.Y), 1 / 1000f * 0.04f) < 0;

            var surface = isGrass ? SurfaceDefinitions.Grass : SurfaceDefinitions.Dirt;
            cell.Surface = surface;

            if (isRockWall)
            {
                entities.Add(new BuildingEntity(cell.Position, HexagonDirection.Left, BuildingDefinitions.RockWall));
                cell.Surface = SurfaceDefinitions.Empty;
            }
        }

        Random plantRandom = new Random(6547);
        var allowedPlantCells = map.Cells.Where(c => c.Surface == SurfaceDefinitions.Grass).ToList();
        var grassCellsCount = allowedPlantCells.Count;
        var treeCount = (int)(grassCellsCount * (1f / 6));
        for (int i = 0; i < treeCount; i++)
        {
            var cellIndex = plantRandom.Next(0, allowedPlantCells.Count - 1);
            var position = allowedPlantCells[cellIndex].Position;
            entities.Add(new BuildingEntity(position, HexagonDirection.Left, BuildingDefinitions.Plant.TreeOak));
            allowedPlantCells.RemoveAt(cellIndex);
        }

        var haygrassCount = (int)(grassCellsCount * (1f / 6));
        for (int i = 0; i < haygrassCount; i++)
        {
            var cellIndex = plantRandom.Next(0, allowedPlantCells.Count - 1);
            var position = allowedPlantCells[cellIndex].Position;
            entities.Add(new BuildingEntity(position, HexagonDirection.Left, BuildingDefinitions.Plant.Haygrass));
            allowedPlantCells.RemoveAt(cellIndex);
        }

        var riceCount = (int)(grassCellsCount * (1f / 6));
        for (int i = 0; i < riceCount; i++)
        {
            var cellIndex = plantRandom.Next(0, allowedPlantCells.Count - 1);
            var position = allowedPlantCells[cellIndex].Position;
            entities.Add(new BuildingEntity(position, HexagonDirection.Left, BuildingDefinitions.Plant.WildRice));
            allowedPlantCells.RemoveAt(cellIndex);
        }

        foreach (var cell in map.Cells)
        {
            var isNearOrigin = cell.Position.DistanceFrom(HexCubeCoord.Zero) <= 6;

            if (isNearOrigin)
            {
                entities.RemoveAll(x => x.OccupiedCells.Contains(cell.Position));
                cell.Surface = SurfaceDefinitions.TileFloor;
            }

            var isOriginEdge = cell.Position.DistanceFrom(HexCubeCoord.Zero) == 6;
            var isCenterLine = cell.Position.Q == cell.Position.R && cell.Position.Q > 0;
            if (isOriginEdge)
            {
                if (isCenterLine)
                {
                    // add door
                }
                else
                {
                    entities.Add(new BuildingEntity(cell.Position, HexagonDirection.Left,
                        BuildingDefinitions.WoodenWall));
                }
            }

            var isInsideA = cell.Position.DistanceFrom(HexCubeCoord.Zero) <= 5;

            var isInsideB = cell.Position.DistanceFrom(HexCubeCoord.Zero) <= 1;
            var isHexant = cell.Position.Q <= 0 && cell.Position.R <= 0;
            if (isInsideA && !isInsideB && isHexant)
            {
                entities.Add(
                    new BuildingEntity(cell.Position, HexagonDirection.Left, BuildingDefinitions.StockpileZone));
            }
        }

        entities.Add(new BuildingEntity(new HexCubeCoord(3, -5, 2), HexagonDirection.TopRight, BuildingDefinitions.WoodenChair));
        entities.Add(new BuildingEntity(new HexCubeCoord(4, -5, 1), HexagonDirection.BottomRight, BuildingDefinitions.WoodenBed));

        entities.Add(new BuildingEntity(new HexCubeCoord(-3, 5, -2), HexagonDirection.BottomLeft, BuildingDefinitions.WoodenChair));
        entities.Add(new BuildingEntity(new HexCubeCoord(-4, 5, -1), HexagonDirection.TopLeft, BuildingDefinitions.WoodenBed));

        return (map, entities);
    }
}

public class EmptyMapGenerator : IMapGenerator
{
    public (HexagonalMapEntity, List<IEntity>) GenerateNewMap()
    {
        var map = new HexagonalMapEntity(8);

        foreach (var cell in map.Cells)
        {
            cell.Surface = SurfaceDefinitions.Dirt;
        }

        List<IEntity> entities = new List<IEntity>();
        return (map, entities);
    }
}

public class WallTilingTestMapGenerator : IMapGenerator
{
    public (HexagonalMapEntity, List<IEntity>) GenerateNewMap()
    {
        var map = new HexagonalMapEntity(16);

        foreach (var cell in map.Cells)
        {
            cell.Surface = SurfaceDefinitions.Grass;
        }

        List<IEntity> entities = new List<IEntity>();

        entities.Add(new BuildingEntity(new HexCubeCoord(3, 3, -6), HexagonDirection.Left,
            BuildingDefinitions.StoneWall));
        entities.Add(new BuildingEntity(new HexCubeCoord(3, 2, -5), HexagonDirection.Left,
            BuildingDefinitions.StoneWall));
        entities.Add(new BuildingEntity(new HexCubeCoord(3, 1, -4), HexagonDirection.Left,
            BuildingDefinitions.StoneWall));

        entities.Add(new BuildingEntity(new HexCubeCoord(-3, 3, 0), HexagonDirection.Left,
            BuildingDefinitions.StoneWall));
        entities.Add(new BuildingEntity(new HexCubeCoord(-2, 3, -1), HexagonDirection.Left,
            BuildingDefinitions.StoneWall));
        entities.Add(new BuildingEntity(new HexCubeCoord(-1, 3, -2), HexagonDirection.Left,
            BuildingDefinitions.StoneWall));

        entities.Add(new BuildingEntity(new HexCubeCoord(-3, 0, 3), HexagonDirection.Left,
            BuildingDefinitions.StoneWall));
        entities.Add(new BuildingEntity(new HexCubeCoord(-2, -1, 3), HexagonDirection.Left,
            BuildingDefinitions.StoneWall));
        entities.Add(new BuildingEntity(new HexCubeCoord(-1, -2, 3), HexagonDirection.Left,
            BuildingDefinitions.StoneWall));

        entities.Add(new BuildingEntity(new HexCubeCoord(1, -4, 3), HexagonDirection.Left,
            BuildingDefinitions.StoneWall));
        entities.Add(new BuildingEntity(new HexCubeCoord(2, -5, 3), HexagonDirection.Left,
            BuildingDefinitions.StoneWall));
        entities.Add(new BuildingEntity(new HexCubeCoord(1, -5, 4), HexagonDirection.Left,
            BuildingDefinitions.StoneWall));

        entities.Add(new BuildingEntity(new HexCubeCoord(4, -3, -1), HexagonDirection.Left,
            BuildingDefinitions.StoneWall));
        entities.Add(new BuildingEntity(new HexCubeCoord(5, -4, -1), HexagonDirection.Left,
            BuildingDefinitions.StoneWall));
        entities.Add(new BuildingEntity(new HexCubeCoord(5, -3, -2), HexagonDirection.Left,
            BuildingDefinitions.StoneWall));

        entities.Add(new BuildingEntity(new HexCubeCoord(-7, 3, 4), HexagonDirection.Left,
            BuildingDefinitions.StoneWall));
        entities.Add(new BuildingEntity(new HexCubeCoord(-6, 2, 4), HexagonDirection.Left,
            BuildingDefinitions.StoneWall));
        entities.Add(new BuildingEntity(new HexCubeCoord(-8, 3, 5), HexagonDirection.Left,
            BuildingDefinitions.StoneWall));
        entities.Add(new BuildingEntity(new HexCubeCoord(-7, 4, 3), HexagonDirection.Left,
            BuildingDefinitions.StoneWall));

        entities.Add(new BuildingEntity(new HexCubeCoord(-7, 7, 0), HexagonDirection.Left,
            BuildingDefinitions.StoneWall));
        entities.Add(new BuildingEntity(new HexCubeCoord(-7, 6, 1), HexagonDirection.Left,
            BuildingDefinitions.StoneWall));
        entities.Add(new BuildingEntity(new HexCubeCoord(-6, 7, -1), HexagonDirection.Left,
            BuildingDefinitions.StoneWall));
        entities.Add(new BuildingEntity(new HexCubeCoord(-8, 8, 0), HexagonDirection.Left,
            BuildingDefinitions.StoneWall));

        return (map, entities);
    }
}

public class ConstructionTestMapGenerator : IMapGenerator
{
    public (HexagonalMapEntity, List<IEntity>) GenerateNewMap()
    {
        var map = new HexagonalMapEntity(16);

        foreach (var cell in map.Cells)
        {
            cell.Surface = SurfaceDefinitions.Grass;
        }

        List<IEntity> entities = new List<IEntity>();

        foreach (HexagonDirection direction in Enum.GetValues(typeof(HexagonDirection)).Cast<HexagonDirection>())
        {
            var chairPos = HexCubeCoord.Zero + direction;
            entities.Add(new BuildingEntity(chairPos, direction, BuildingDefinitions.WoodenChair));

            var bedPos = chairPos + direction;
            entities.Add(new BuildingEntity(bedPos, direction, BuildingDefinitions.WoodenBed));

            // var stovePos = bedPos + direction + direction;
            // entities.Add(new BuildingEntity(stovePos, direction, BuildingDefinitions.TableStoveFueled));

            var craftingDeskPosition = bedPos + direction + direction;
            entities.Add(new BuildingEntity(craftingDeskPosition, direction, BuildingDefinitions.CraftingDesk));
        }

        return (map, entities);
    }
}