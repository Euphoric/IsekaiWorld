using System;

namespace IsekaiWorld.Test
{
    public static class HelperExtensions
    {
        public static Func<GameTestStep, bool> Reaches(this CharacterEntity character, HexCubeCoord position)
        {
            return gts => character.Position == position;
        }
    }

    public class GameTestStep
    {
        public GameEntity Game { get; }

        public GameTestStep(GameEntity game)
        {
            Game = game;
        }
    }
}