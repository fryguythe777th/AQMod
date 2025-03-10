﻿using AQMod.Content.MapMarkers.Components;
using AQMod.Tiles.TileEntities;
using Microsoft.Xna.Framework;
using Terraria;

namespace AQMod.Content.MapMarkers
{
    public abstract class MapMarkerData
    {
        public string Name { get; internal set; }
        public int ItemTypeBind { get; internal set; }
        public MapMarkerData(string name, int item, bool setup = true)
        {
            Name = name;
            ItemTypeBind = item;
            if (setup)
                Setup();
        }

        public abstract void DrawMap(ref string mouseText, Player player, AQPlayer aQPlayer, MapMarkerLayerToggles toggles);

        protected virtual void Setup()
        {
        }

        public virtual void OnAddToGlobe(Player player, AQPlayer aQPlayer, TEGlobe globe)
        {
            var rectangle = new Rectangle(globe.Position.X * 16, globe.Position.Y * 16, 32, 32);
            for (int k = 0; k < 12; k++)
            {
                int d = Dust.NewDust(new Vector2(rectangle.X, rectangle.Y), rectangle.Width, rectangle.Height, 261);
                Main.dust[d].noGravity = true;
                Main.dust[d].velocity.X *= 0.2f;
                Main.dust[d].velocity.Y = -Main.rand.NextFloat(1f, 3f);
            }
        }

        public virtual void NearbyEffects(TEGlobe globe)
        {
        }
    }
}