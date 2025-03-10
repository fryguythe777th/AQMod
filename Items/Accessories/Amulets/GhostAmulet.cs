﻿using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Accessories.Amulets
{
    public class GhostAmulet : ModItem
    {
        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.accessory = true;
            item.rare = ItemRarityID.Blue;
            item.value = Item.sellPrice(silver: 10);
        }

        public override void UpdateInventory(Player player)
        {
            player.GetModPlayer<AQPlayer>().ghostAmuletHeld = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            var aQPlayer = player.GetModPlayer<AQPlayer>();
            aQPlayer.ghostAmulet = true;
            aQPlayer.ghostAmuletHeld = true;
        }
    }
}