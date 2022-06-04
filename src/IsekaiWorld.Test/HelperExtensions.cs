using System;
using Xunit;

namespace IsekaiWorld.Test
{
    public static class HelperExtensions
    {
        public static void UpdateUntil(this GameEntity game, Func<bool> check, float maxTimeout = 100)
        {
            float time = 0;
            while (!check())
            {
                if (time >= maxTimeout)
                {
                    throw new Exception("Didn't reach final check before timeout.");
                }
                time += 0.1f;
                game.Update(0.1f);
            }
        }
    }
}