using System;
using Xunit;

namespace IsekaiWorld.Test
{
    public class Tests
    {
        [Fact]
        public void Test1()
        {
            var game = new GameEntity();
            var character = game.AddCharacter();
            
            Assert.True(character.IsIdle);
        }
    }
}