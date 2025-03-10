﻿using AQMod.Assets.LegacyItemOverlays;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Weapons.Melee
{
    public class HellsBoon : ModItem
    {
        public override void SetStaticDefaults()
        {
            if (!Main.dedServ)
                AQMod.ItemOverlays.Register(new LegacyGlowmaskOverlay(AQUtils.GetPath(this) + "_Glow", new Color(200, 200, 200, 0)), item.type);
        }

        public override void SetDefaults()
        {
            item.width = 40;
            item.height = 40;
            item.damage = 22;
            item.useTime = 38;
            item.useAnimation = 19;
            item.autoReuse = true;
            item.rare = AQItem.Rarities.GoreNestRare;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.UseSound = SoundID.Item1;
            item.value = Item.sellPrice(gold: 1);
            item.melee = true;
            item.knockBack = 3f;
            item.shootSpeed = 35f;
            item.shoot = ModContent.ProjectileType<Projectiles.Melee.HellsBoon>();
            item.scale = 1.25f;
        }

        public override void OnHitNPC(Player player, NPC target, int damage, float knockBack, bool crit)
        {
            Buffs.Debuffs.CorruptionHellfire.Inflict(target, 240);
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return AQItem.Reps.DemonSiegeItem_GetAlpha(lightColor);
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            Projectiles.Melee.HellsBoon.SpawnCluster(Main.MouseWorld, (int)(item.shootSpeed / player.meleeSpeed), damage, knockBack, player);
            return false;
        }
    }
}