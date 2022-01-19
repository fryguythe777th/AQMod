﻿using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Accessories
{
    [AutoloadEquip(EquipType.Neck)]
    public class PearlAmulet : ModItem
    {
        public override void SetDefaults()
        {
            item.width = 28;
            item.height = 28;
            item.accessory = true;
            item.rare = AQItem.Rarities.CrabCreviceRare;
            item.value = AQItem.Prices.CrabCreviceValue;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            var aQPlayer = player.GetModPlayer<AQPlayer>();
        }
    }
}