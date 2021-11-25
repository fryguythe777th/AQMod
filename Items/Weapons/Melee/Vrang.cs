﻿using AQMod.Common;
using AQMod.Common.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Weapons.Melee
{
    public class Vrang : ModItem
    {        
        private static int _temperature;
        private float _temp;

        public override void SetDefaults()
        {
            item.width = 24;
            item.height = 24;
            item.useAnimation = 24;
            item.useTime = 24;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.melee = true;
            item.damage = 39;
            item.knockBack = 3f;
            item.value = AQItem.AtmosphericCurrentsValue;
            item.UseSound = SoundID.Item1;
            item.rare = ItemRarityID.Pink;
            item.channel = true;
            item.noMelee = true;
            item.noUseGraphic = true;
            item.shootSpeed = 24f;
            item.shoot = ModContent.ProjectileType<Projectiles.Melee.Vrang>();
            item.maxStack = 11;
            _temperature = 1;
        }

        public override bool AltFunctionUse(Player player)
        {
            return true;
        }

        public override bool CanUseItem(Player player)
        {
            if (!base.CanUseItem(player))
            {
                return false;
            }
            if (player.altFunctionUse == 2)
            {
                _temperature = -1;
            }
            else
            {
                _temperature = 1;
            }
            return true;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            if (item.stack > 1)
            {
                int boomerangsLeft = item.stack - 1;
                Main.NewText(boomerangsLeft);
                int half = boomerangsLeft / 2;
                int rightProj = Projectile.NewProjectile(position, new Vector2(speedX, speedY), type, damage, knockBack, player.whoAmI, 1f);
                var aQProj = Main.projectile[rightProj].GetGlobalProjectile<AQProjectile>();
                sbyte setTemperature = 10;
                bool canFreeze = false;
                bool canHeat = true;
                if (player.altFunctionUse == 2)
                {
                    setTemperature = -10;
                    canFreeze = true;
                    canHeat = false;
                }
                int leftProj = rightProj;
                aQProj.temperature = setTemperature;
                aQProj.canFreeze = canFreeze;
                aQProj.canHeat = canHeat;
                for (int i = 0; i < half; i++)
                {
                    int p = Projectile.NewProjectile(position + new Vector2(32f * i, 0f), new Vector2(speedX, speedY), type, damage, knockBack, player.whoAmI, 
                        -Main.projectile[rightProj].identity);
                    aQProj = Main.projectile[p].GetGlobalProjectile<AQProjectile>();
                    aQProj.temperature = setTemperature;
                    aQProj.canFreeze = canFreeze;
                    aQProj.canHeat = canHeat;
                    rightProj = p;
                    p = Projectile.NewProjectile(position + new Vector2(-32f * i, 0f), new Vector2(speedX, speedY), type, damage, knockBack, player.whoAmI, 
                        -Main.projectile[leftProj].identity);
                    aQProj = Main.projectile[p].GetGlobalProjectile<AQProjectile>();
                    aQProj.temperature = setTemperature;
                    aQProj.canFreeze = canFreeze;
                    aQProj.canHeat = canHeat;
                    leftProj = p;
                    boomerangsLeft--;
                }
                if (boomerangsLeft == 1)
                {
                    int p = Projectile.NewProjectile(position, new Vector2(speedX, speedY), type, damage, knockBack, player.whoAmI,
                        -Main.projectile[rightProj].identity);
                    aQProj = Main.projectile[rightProj].GetGlobalProjectile<AQProjectile>();
                    aQProj.temperature = setTemperature;
                    aQProj.canFreeze = canFreeze;
                    aQProj.canHeat = canHeat;
                }
                return false;
            }
            return base.Shoot(player, ref position, ref speedX, ref speedY, ref type, ref damage, ref knockBack);
        }

        private void updateTemperature()
        {
            _temp = MathHelper.Lerp(_temp, _temperature, 0.01f);
        }

        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            updateTemperature();
            Texture2D texture;
            if (_temp < 0f)
            {
                texture = ModContent.GetTexture(this.GetPath("_Cold"));
            }
            else if (_temp > 0f)
            {
                texture = ModContent.GetTexture(this.GetPath("_Hot"));
            }
            else
            {
                return true;
            }
            Main.spriteBatch.Draw(Main.itemTexture[item.type], position, frame, drawColor, 0f, origin, scale, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(texture, position, frame, drawColor * _temp.Abs(), 0f, origin, scale, SpriteEffects.None, 0f);
            return false;
        }
    }
}