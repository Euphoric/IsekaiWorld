using System;
using System.Linq;
using FluentAssertions;
using Xunit;

namespace IsekaiWorld.Test
{
    public class Tests
    {
        [Fact]
        public void Construction_test()
        {
            var game = new GameEntity();
            game.Initialize(new EmptyMapGenerator());

            var character = game.AddCharacter("Test guy");
            character.Position = HexCubeCoord.Zero;

            foreach (var cell in game.GameMap.Cells)
            {
                if (cell.Position.R == cell.Position.Q && cell.Position.DistanceFrom(HexCubeCoord.Zero) <= 6)
                {
                    game.StartConstruction(cell.Position, HexagonDirection.Left, ConstructionDefinitions.StoneWall);
                }
            }

            var constructionPositions = game.Constructions.Select(x => x.Position).ToHashSet();

            bool timedOut = game.UpdateUntil(() =>
            {
                var issues = game.CheckForIssues().ToList();
                issues.Should().BeEmpty();

                return !game.Constructions.Any();
            }, 300);

            if (timedOut)
            {
                throw new Exception("Didn't reach final check before timeout.");
            }

            var buildingPositions = game.Buildings.Select(x => x.Position).ToHashSet();

            buildingPositions.Should().BeEquivalentTo(constructionPositions);
        }

        [Fact]
        public void Items_hauling_test_simple()
        {
            var game = new GameEntity();
            game.Initialize(new EmptyMapGenerator());

            var character = game.AddCharacter("Test guy");
            character.Position = HexCubeCoord.Zero;

            var stockpile = new BuildingEntity(new HexCubeCoord(1, 1, -2), HexagonDirection.Left,
                BuildingDefinitions.StockpileZone);
            game.AddEntity(stockpile);
            game.SpawnItem(new HexCubeCoord(-1, -1, 2), ItemDefinitions.Wood);

            bool timedOut = game.UpdateUntil(() =>
            {
                var issues = game.CheckForIssues().ToList();
                issues.Should().BeEmpty();

                var itemInStockpile = game.Items.FirstOrDefault(item => item.Position == stockpile.Position);

                return itemInStockpile != null;
            }, 300);

            if (timedOut)
            {
                throw new Exception("Didn't reach final check before timeout.");
            }
        }

        [Fact]
        public void Items_hauling_test_stacking()
        {
            var game = new GameEntity();
            game.Initialize(new EmptyMapGenerator());

            var character = game.AddCharacter("Test guy");
            character.Position = HexCubeCoord.Zero;

            game.AddEntity(new BuildingEntity(new HexCubeCoord(0, 0, 0), HexagonDirection.Left, BuildingDefinitions.StockpileZone));
            game.AddEntity(new BuildingEntity(new HexCubeCoord(1, 0, -1), HexagonDirection.Left, BuildingDefinitions.StockpileZone));
            
            foreach (var mapCell in game.GameMap.Cells)
            {
                var zeroDistance = mapCell.Position.DistanceFrom(HexCubeCoord.Zero);
                var typeDivide = mapCell.Position.R >= 0;
                if (3 <= zeroDistance && zeroDistance <= 5)
                {
                    game.SpawnItem(mapCell.Position, typeDivide ? ItemDefinitions.Wood : ItemDefinitions.Grains);
                }
            }

            var totalItemCountStart = game.Items.GroupBy(x=>x.Definition).Select(grp=> new {Definition = grp.Key, Count = grp.Sum(x => x.Count)}).ToList();
            
            bool timedOut = game.UpdateUntil(() =>
            {
                var issues = game.CheckForIssues().ToList();
                issues.Should().BeEmpty();

                var stockpilePositions =
                    game.Buildings
                        .Where(b => b.Definition == BuildingDefinitions.StockpileZone)
                        .Select(x => x.Position).ToHashSet();

                var itemsOutsideStockpiles = game.Items.Where(it => !stockpilePositions.Contains(it.Position)).ToList();

                return !itemsOutsideStockpiles.Any();
            }, 300);

            if (timedOut)
            {
                throw new Exception("Didn't reach final check before timeout.");
            }
            
            var multipleItemsOnSamePositions =
                game.Items.GroupBy(it => it.Position)
                    .Where(grp => grp.Count() > 1)
                    .ToList();
            multipleItemsOnSamePositions.Should().BeEmpty();
            
            var totalItemCountEnd = game.Items.GroupBy(x=>x.Definition).Select(grp=> new {Definition = grp.Key, Count = grp.Sum(x => x.Count)}).ToList();
            totalItemCountEnd.Should().BeEquivalentTo(totalItemCountStart);
        }
    }
}