﻿using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Accessories
{
    [AutoloadEquip(EquipType.Neck)]
    public class HeatedAmulet : ModItem
    {
        public override void SetDefaults()
        {
            item.width = 28;
            item.height = 28;
            item.accessory = true;
            item.defense = 2;
            item.rare = AQItem.Rarities.GaleStreamsRare;
            item.value = AQItem.Prices.GaleStreamsValue;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<AQPlayer>().hotAmulet = true;
            player.fireWalk = true;
            player.buffImmune[BuffID.OnFire] = true;
            player.buffImmune[BuffID.Burning] = true;
            player.buffImmune[BuffID.CursedInferno] = true;
            player.buffImmune[BuffID.ShadowFlame] = true;
            player.buffImmune[ModContent.BuffType<Buffs.Debuffs.BlueFire>()] = true;
            player.buffImmune[ModContent.BuffType<Buffs.Debuffs.Sparkling>()] = true;
            player.buffImmune[ModContent.BuffType<Buffs.Debuffs.CorruptionHellfire>()] = true;
            player.buffImmune[ModContent.BuffType<Buffs.Debuffs.CrimsonHellfire>()] = true;
        }

        public override void AddRecipes()
        {
            var r = new ModRecipe(mod);
            r.AddIngredient(ModContent.ItemType<DegenerationRing>());
            r.AddIngredient(ItemID.Shackle);
            r.AddIngredient(ModContent.ItemType<Materials.Energies.AtmosphericEnergy>(), 5);
            r.AddIngredient(ModContent.ItemType<Materials.Fluorescence>(), 20);
            r.AddIngredient(ItemID.SoulofFlight, 20);
            r.AddTile(TileID.MythrilAnvil);
            r.SetResult(this);
            r.AddRecipe();
        }
    }
}