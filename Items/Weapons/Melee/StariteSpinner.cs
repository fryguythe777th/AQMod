﻿using AQMod.Items.Materials.Energies;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Weapons.Melee
{
    public class StariteSpinner : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.Yoyo[item.type] = true;
            ItemID.Sets.GamepadExtraRange[item.type] = 12;
            ItemID.Sets.GamepadSmartQuickReach[item.type] = true;
        }

        public override void SetDefaults()
        {
            item.width = 24;
            item.height = 24;
            item.useAnimation = 25;
            item.useTime = 25;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.melee = true;
            item.damage = 20;
            item.knockBack = 2.11f;
            item.value = AQItem.Prices.GlimmerWeaponValue;
            item.UseSound = SoundID.Item1;
            item.rare = AQItem.Rarities.StariteWeaponRare;
            item.channel = true;
            item.noMelee = true;
            item.noUseGraphic = true;
            item.shootSpeed = 10f;
            item.shoot = ModContent.ProjectileType<Projectiles.Melee.StariteSpinner>();
        }

        public override void AddRecipes()
        {
            var r = new ModRecipe(mod);
            r.AddIngredient(ItemID.WoodYoyo);
            r.AddIngredient(ItemID.FallenStar, 8);
            r.AddIngredient(ModContent.ItemType<CosmicEnergy>(), 3);
            r.AddTile(TileID.Anvils);
            r.SetResult(this);
            r.AddRecipe();
        }
    }
}