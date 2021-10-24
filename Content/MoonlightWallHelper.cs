﻿using AQMod.Walls;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace AQMod.Content
{
    /// <summary>
    /// Helps manage things that interact with the Moonlight Wall
    /// </summary>
    public class MoonlightWallHelper
    {
        private bool _dayTime;
        public static MoonlightWallHelper Instance;
        public bool Active { get; private set; }

        public static bool BehindMoonlightWall(Vector2 center)
        {
            return BehindMoonlightWall((int)center.X / 16, (int)center.Y / 16);
        }

        public static bool BehindMoonlightWall(int x, int y)
        {
            if (x < 0 || x > Main.maxTilesX || y < 0 || y > Main.maxTilesY)
            {
                return false;
            }
            return x < 0 || x > Main.maxTilesX || y < 0 || y > Main.maxTilesY
                ? false
                : Framing.GetTileSafely(x, y).wall == ModContent.WallType<MoonlightWallWall>();
        }

        public void Begin()
        {
            Active = true;
            _dayTime = Main.dayTime;
            Main.dayTime = false;
        }

        public void End()
        {
            Active = false;
            Main.dayTime = _dayTime;
        }
    }
}