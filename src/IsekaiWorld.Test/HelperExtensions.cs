using System;
using Xunit;

namespace IsekaiWorld.Test
{
    public static class HelperExtensions
    {
        public static bool UpdateUntil(this GameEntity game, Func<bool> check, float maxTimeout = 1000)
        {
            float time = 0;
            while (!check())
            {
                if (time >= maxTimeout)
                {
                    return true;
                }
                time += 0.1f;
                game.Update();
            }

            return false;
        }
    }
}