﻿using AQMod.Items.Armor.Arachnotron;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace AQMod.Common.Graphics.PlayerEquips
{
    public class ArachnotronRibcageOverlay : EquipBodyOverlay
    {
        public ArachnotronRibcageOverlay() : base(ModContent.GetTexture(AQUtils.GetPath<ArachnotronRibcage>("_BodyGlow")))
        {
        }

        public override void Draw(PlayerDrawInfo info)
        {
            if (info.shadow == 0)
            {
                if (Main.myPlayer == info.drawPlayer.whoAmI)
                {
                    if (AQPlayer.oldPosLength < AQPlayer.ARACHNOTRON_OLD_POS_LENGTH)
                        AQPlayer.oldPosLength = AQPlayer.ARACHNOTRON_OLD_POS_LENGTH;
                    AQPlayer.arachnotronBodyTrail = true;
                }
                GetBasicPlayerDrawInfo(info, out Vector2 bodyPosition, out float opacity);
                var clr = new Color(250, 250, 250, 0);
                var texture = Asset.Value;
                Main.playerDrawData.Add(new DrawData(texture, bodyPosition, info.drawPlayer.bodyFrame, clr, info.drawPlayer.headRotation, info.headOrigin, 1f, info.spriteEffects, 0) { shader = info.headArmorShader });
            }
        }
    }
}