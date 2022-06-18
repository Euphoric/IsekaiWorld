using System.Linq;
using FluentAssertions;
using Xunit;

namespace IsekaiWorld.Test
{
    public class Tests
    {
        [Fact]
        public void Test1()
        {
            var game = new GameEntity();
            game.Initialize(new EmptyMapGenerator());
            
            var character = game.AddCharacter("Test guy");
            character.Position = HexCubeCoord.Zero;

            foreach (var cell in game.GameMap.Cells)
            {
                if (cell.Position.DistanceFrom(HexCubeCoord.Zero) < 5)
                {
                    game.StartConstruction(cell.Position, HexagonDirection.Left, ConstructionDefinitions.StoneWall);
                }
            }

            game.UpdateUntil(()=>
            {
                var issues = game.CheckForIssues().ToList();
                issues.Should().BeEmpty();
                
                return !game.Constructions.Any();
            }, 300);
        }
    }
}