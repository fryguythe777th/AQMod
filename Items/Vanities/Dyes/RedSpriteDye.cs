﻿using AQMod.Effects.Dyes;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Graphics.Shaders;

namespace AQMod.Items.Vanities.Dyes
{
    public class RedSpriteDye : DyeItem
    {
        public override string Pass => "RedSpritePass";
        public override int Rarity => AQItem.Rarities.GaleStreamsRare - 1;

        public override ArmorShaderData CreateShaderData()
        {
            return new ArmorShaderDataModifyLightColor(Effect, Pass, (v) =>
            {
                return v * new Vector3(0.549f, 0f, 0.082f);
            }).UseColor(new Color(140, 0, 21, 255));
        }
    }
}