﻿using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Accessories
{
    public class Breadsoul : ModItem
    {
        public override void SetDefaults()
        {
            item.width = 24;
            item.height = 24;
            item.accessory = true;
            item.rare = ItemRarityID.Yellow;
            item.value = Item.sellPrice(gold: 10);
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(255, 255, 255, 200 - item.alpha);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.npcTypeNoAggro[NPCID.DungeonSpirit] = true;
            player.npcTypeNoAggro[ModContent.NPCType<NPCs.Monsters.Heckto>()] = true;
            var aQPlayer = player.GetModPlayer<AQPlayer>();
            aQPlayer.grabReachMult += 0.25f;
            aQPlayer.breadsoul = true;
        }
    }
}