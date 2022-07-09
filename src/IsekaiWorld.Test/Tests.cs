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
            
            bool timedOut = game.UpdateUntil(()=>
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
        public void Items_hauling_test()
        {
            var game = new GameEntity();
            game.Initialize(new EmptyMapGenerator());
            
            var character = game.AddCharacter("Test guy");
            character.Position = HexCubeCoord.Zero;

            var stockpile = new BuildingEntity(new HexCubeCoord(1, 1, -2), HexagonDirection.Left, BuildingDefinitions.StockpileZone);
            game.AddEntity(stockpile);
            game.SpawnItem(new HexCubeCoord(-1, -1, 2), ItemDefinitions.Wood);

            bool timedOut = game.UpdateUntil(()=>
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
    }
}