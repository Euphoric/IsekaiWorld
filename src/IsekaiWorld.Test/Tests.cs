using Xunit;

namespace IsekaiWorld.Test
{
    public class Tests
    {
        [Fact]
        public void Test1()
        {
            var game = new GameEntity();
            game.Initialize();
            
            var character = game.AddCharacter();
            
            Assert.True(character.IsIdle);
            
            game.StartConstruction(new HexCubeCoord(1, 1, -2));
            
            Assert.True(character.IsIdle);
            
            game.Update(0.1f);
            
            Assert.False(character.IsIdle);
            var construction = Assert.IsType<ConstructionActivity>(character.CurrentActivity);
            Assert.False(construction.IsFinished);

            game.UpdateUntil(() => construction.IsFinished);
            game.Update(0.1f);
        }
    }
}