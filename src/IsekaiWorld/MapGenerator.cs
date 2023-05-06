using System;
using System.Linq;
using Godot;

namespace IsekaiWorld;

public interface IMapGenerator
{
    public int MapSize { get; }
    void GenerateMap(GameEntity game);
}

public class MapGenerator : IMapGenerator
{
    public int MapSize => 32;

    public void GenerateMap(GameEntity game)
    {
        GenerateSurface(game);

        GenerateBasicHut(game);

        GeneratePlants(game);
    }

    private void GenerateSurface(GameEntity game)
    {
        var surfaceNoise = new Noise { Seed = 123 };
        var rockWallNoise = new Noise { Seed = 654 };

        foreach (var cell in game.GameMap.Cells)
        {
            var center = cell.Position.Center(1000);
            var elevation = rockWallNoise.CalcPixel2D(
                Mathf.CeilToInt(center.X),
                Mathf.CeilToInt(center.Y), 1 / 1000f * 0.03f);
            var isRockWall = elevation < -0.6;
            var isRockSurface = elevation < -0.4;

            var isGrass =
                surfaceNoise.CalcPixel2D(Mathf.CeilToInt(center.X), Mathf.CeilToInt(center.Y), 1 / 1000f * 0.04f) <
                0.3f;

            var surface = isRockSurface ? SurfaceDefinitions.RoughStone :
                isGrass ? SurfaceDefinitions.Grass : SurfaceDefinitions.Dirt;
            game.GameMap.SetCellSurface(cell.Position, surface);

            if (isRockWall)
            {
                game.AddEntity(new BuildingEntity(cell.Position, HexagonDirection.Left, BuildingDefinitions.RockWall));
            }
        }
    }

    private static void GeneratePlants(GameEntity game)
    {
        Random plantRandom = new Random(6547);
        var allowedPlantCells = game.GameMap.Cells.Where(c => c.Surface == SurfaceDefinitions.Grass).ToList();
        var grassCellsCount = allowedPlantCells.Count;
        var treeCount = (int)(grassCellsCount * (1f / 6));
        for (int i = 0; i < treeCount; i++)
        {
            var cellIndex = plantRandom.Next(0, allowedPlantCells.Count - 1);
            var position = allowedPlantCells[cellIndex].Position;
            game.AddEntity(new BuildingEntity(position, HexagonDirection.Left, BuildingDefinitions.Plant.TreeOak));
            allowedPlantCells.RemoveAt(cellIndex);
        }

        var haygrassCount = (int)(grassCellsCount * (1f / 2));
        for (int i = 0; i < haygrassCount; i++)
        {
            var cellIndex = plantRandom.Next(0, allowedPlantCells.Count - 1);
            var position = allowedPlantCells[cellIndex].Position;
            game.AddEntity(new BuildingEntity(position, HexagonDirection.Left, BuildingDefinitions.Plant.Grass));
            allowedPlantCells.RemoveAt(cellIndex);
        }

        var riceCount = (int)(grassCellsCount * (1f / 6));
        for (int i = 0; i < riceCount; i++)
        {
            var cellIndex = plantRandom.Next(0, allowedPlantCells.Count - 1);
            var position = allowedPlantCells[cellIndex].Position;
            game.AddEntity(new BuildingEntity(position, HexagonDirection.Left, BuildingDefinitions.Plant.WildRice));
            allowedPlantCells.RemoveAt(cellIndex);
        }
    }

    private static void GenerateBasicHut(GameEntity game)
    {
        foreach (var pos in HexCubeCoord.HexagonArea(HexCubeCoord.Zero, 6))
        {
            var entitiesOn = game.EntitiesOn(pos);
            foreach (var entity in entitiesOn)
            {
                entity.Remove();
            }
            
            game.GameMap.SetCellSurface(pos, SurfaceDefinitions.StoneTileFloor);
        }
        
        foreach (var pos in HexCubeCoord.HexagonRing(HexCubeCoord.Zero, 6))
        {
            var isCenterLine = pos.Q == pos.R && pos.Q > 0;
            if (isCenterLine)
            {
                // add door
            }
            else
            {
                game.SpawnBuilding(pos, HexagonDirection.Left,
                    BuildingDefinitions.WoodenWall);
            }
        }
        
        foreach (var pos in HexCubeCoord.HexagonArea(HexCubeCoord.Zero, 5))
        {
            var isInsideB = pos.DistanceFrom(HexCubeCoord.Zero) <= 1;
            var isHexant = pos.Q <= 0 && pos.R <= 0;
            if (!isInsideB && isHexant)
            {
                game.SpawnBuilding(pos, HexagonDirection.Left,
                    BuildingDefinitions.StockpileZone);
            }
        }

        game.SpawnBuilding(new HexCubeCoord(3, -5, 2), HexagonDirection.TopRight,
            BuildingDefinitions.WoodenChair);
        game.SpawnBuilding(new HexCubeCoord(4, -5, 1), HexagonDirection.BottomRight,
            BuildingDefinitions.WoodenBed);

        game.SpawnBuilding(new HexCubeCoord(-3, 5, -2), HexagonDirection.BottomLeft,
            BuildingDefinitions.WoodenChair);
        game.SpawnBuilding(new HexCubeCoord(-4, 5, -1), HexagonDirection.TopLeft,
            BuildingDefinitions.WoodenBed);

        game.SpawnBuilding(new HexCubeCoord(5, -2, -3), HexagonDirection.Left, BuildingDefinitions.CraftingDesk);

        var stockpiles = game.Buildings.Where(x => x.Definition == BuildingDefinitions.StockpileZone).ToList();

        game.SpawnItem(stockpiles[0].Position, ItemDefinitions.Wood, 15);
        game.SpawnItem(stockpiles[1].Position, ItemDefinitions.Grains, 5);
    }
}

public class EmptyMapGenerator : IMapGenerator
{
    public int MapSize => 16;

    public void GenerateMap(GameEntity game)
    {
        foreach (var cell in game.GameMap.Cells)
        {
            game.GameMap.SetCellSurface(cell.Position, SurfaceDefinitions.Dirt);
        }
    }
}

public class WallTilingTestMapGenerator : IMapGenerator
{
    public int MapSize => 16;

    public void GenerateMap(GameEntity game)
    {
        foreach (var cell in game.GameMap.Cells)
        {
            game.GameMap.SetCellSurface(cell.Position, SurfaceDefinitions.Grass);
        }

        game.AddEntity(new BuildingEntity(new HexCubeCoord(3, 3, -6), HexagonDirection.Left,
            BuildingDefinitions.StoneWall));
        game.AddEntity(new BuildingEntity(new HexCubeCoord(3, 2, -5), HexagonDirection.Left,
            BuildingDefinitions.StoneWall));
        game.AddEntity(new BuildingEntity(new HexCubeCoord(3, 1, -4), HexagonDirection.Left,
            BuildingDefinitions.StoneWall));

        game.AddEntity(new BuildingEntity(new HexCubeCoord(-3, 3, 0), HexagonDirection.Left,
            BuildingDefinitions.StoneWall));
        game.AddEntity(new BuildingEntity(new HexCubeCoord(-2, 3, -1), HexagonDirection.Left,
            BuildingDefinitions.StoneWall));
        game.AddEntity(new BuildingEntity(new HexCubeCoord(-1, 3, -2), HexagonDirection.Left,
            BuildingDefinitions.StoneWall));

        game.AddEntity(new BuildingEntity(new HexCubeCoord(-3, 0, 3), HexagonDirection.Left,
            BuildingDefinitions.StoneWall));
        game.AddEntity(new BuildingEntity(new HexCubeCoord(-2, -1, 3), HexagonDirection.Left,
            BuildingDefinitions.StoneWall));
        game.AddEntity(new BuildingEntity(new HexCubeCoord(-1, -2, 3), HexagonDirection.Left,
            BuildingDefinitions.StoneWall));

        game.AddEntity(new BuildingEntity(new HexCubeCoord(1, -4, 3), HexagonDirection.Left,
            BuildingDefinitions.StoneWall));
        game.AddEntity(new BuildingEntity(new HexCubeCoord(2, -5, 3), HexagonDirection.Left,
            BuildingDefinitions.StoneWall));
        game.AddEntity(new BuildingEntity(new HexCubeCoord(1, -5, 4), HexagonDirection.Left,
            BuildingDefinitions.StoneWall));

        game.AddEntity(new BuildingEntity(new HexCubeCoord(4, -3, -1), HexagonDirection.Left,
            BuildingDefinitions.StoneWall));
        game.AddEntity(new BuildingEntity(new HexCubeCoord(5, -4, -1), HexagonDirection.Left,
            BuildingDefinitions.StoneWall));
        game.AddEntity(new BuildingEntity(new HexCubeCoord(5, -3, -2), HexagonDirection.Left,
            BuildingDefinitions.StoneWall));

        game.AddEntity(new BuildingEntity(new HexCubeCoord(-7, 3, 4), HexagonDirection.Left,
            BuildingDefinitions.StoneWall));
        game.AddEntity(new BuildingEntity(new HexCubeCoord(-6, 2, 4), HexagonDirection.Left,
            BuildingDefinitions.StoneWall));
        game.AddEntity(new BuildingEntity(new HexCubeCoord(-8, 3, 5), HexagonDirection.Left,
            BuildingDefinitions.StoneWall));
        game.AddEntity(new BuildingEntity(new HexCubeCoord(-7, 4, 3), HexagonDirection.Left,
            BuildingDefinitions.StoneWall));

        game.AddEntity(new BuildingEntity(new HexCubeCoord(-7, 7, 0), HexagonDirection.Left,
            BuildingDefinitions.StoneWall));
        game.AddEntity(new BuildingEntity(new HexCubeCoord(-7, 6, 1), HexagonDirection.Left,
            BuildingDefinitions.StoneWall));
        game.AddEntity(new BuildingEntity(new HexCubeCoord(-6, 7, -1), HexagonDirection.Left,
            BuildingDefinitions.StoneWall));
        game.AddEntity(new BuildingEntity(new HexCubeCoord(-8, 8, 0), HexagonDirection.Left,
            BuildingDefinitions.StoneWall));
    }
}

public class ConstructionTestMapGenerator : IMapGenerator
{
    public int MapSize => 16;

    public void GenerateMap(GameEntity game)
    {
        foreach (var cell in game.GameMap.Cells)
        {
            game.GameMap.SetCellSurface(cell.Position, SurfaceDefinitions.Grass);
        }

        foreach (HexagonDirection direction in Enum.GetValues(typeof(HexagonDirection)).Cast<HexagonDirection>())
        {
            var chairPos = HexCubeCoord.Zero + direction;
            game.AddEntity(new BuildingEntity(chairPos, direction, BuildingDefinitions.WoodenChair));

            var bedPos = chairPos + direction;
            game.AddEntity(new BuildingEntity(bedPos, direction, BuildingDefinitions.WoodenBed));

            var stovePos = bedPos + direction + direction;
            game.AddEntity(new BuildingEntity(stovePos, direction, BuildingDefinitions.TableStoveFueled));

            var craftingDeskPosition = bedPos + direction + direction;
            game.AddEntity(new BuildingEntity(craftingDeskPosition, direction, BuildingDefinitions.CraftingDesk));
        }
    }
}