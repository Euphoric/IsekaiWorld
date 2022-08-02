using System;
using System.Linq;
using FluentAssertions;

namespace IsekaiWorld.Test
{
    public static class HelperExtensions
    {
        public static void UpdateUntil(this GameEntity game, Func<GameTestStep, bool> check, int maxSteps = 1000)
        {
            var timedOut = UpdateUntilInner(game, check, maxSteps);
            if (timedOut)
            {
                throw new Exception("Didn't reach final check before timeout.");
            }
        }

        public static bool UpdateUntilInner(this GameEntity game, Func<GameTestStep, bool> check, int maxSteps = 1000)
        {
            int steps = 0;
            while (!check(new GameTestStep(game)))
            {
                if (steps >= maxSteps)
                {
                    return true;
                }
                steps++;
                game.Update();
                
                var issues = game.CheckForIssues().ToList();
                issues.Should().BeEmpty();
            }

            return false;
        }

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