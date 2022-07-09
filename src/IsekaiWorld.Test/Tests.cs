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
    }
}