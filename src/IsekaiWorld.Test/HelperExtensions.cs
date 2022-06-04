using System;

namespace IsekaiWorld.Test
{
    public static class HelperExtensions
    {
        public static void UpdateUntil(this GameEntity game, Func<bool> check)
        {
            while (!check())
            {
                game.Update(0.1f);
            }
        }
    }
}