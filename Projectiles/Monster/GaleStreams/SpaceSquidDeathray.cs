﻿using AQMod.Assets;
using AQMod.Effects;
using AQMod.Effects.Trails;
using AQMod.NPCs.Monsters.GaleStreams;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Projectiles.Monster.GaleStreams
{
    public class SpaceSquidDeathray : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            projectile.width = 40;
            projectile.height = 40;
            projectile.hostile = true;
            projectile.aiStyle = -1;
            projectile.timeLeft = 360;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
            projectile.manualDirectionChange = true;

            projectile.GetGlobalProjectile<AQProjectile>().canHeat = false;
            projectile.GetGlobalProjectile<AQProjectile>().temperature = -40;
        }

        public override void AI()
        {
            if (projectile.direction == 0)
                projectile.direction = -1;
            if (Main.expertMode)
            {
                if ((int)projectile.ai[1] == 0)
                {
                    projectile.width = (int)(projectile.width * 1.5f);
                    projectile.height = (int)(projectile.height * 1.5f);
                    projectile.ai[1]++;
                }
            }
            if ((int)(projectile.ai[0] - 1) > -1)
            {
                if (!Main.npc[(int)(projectile.ai[0] - 1)].active)
                {
                    projectile.Kill();
                }
                if (Main.npc[(int)(projectile.ai[0] - 1)].ai[1] > 300f)
                {
                    projectile.height -= 2;
                    if (Main.expertMode)
                    {
                        projectile.height -= 1;
                    }
                    if (projectile.height < 2 || Main.npc[(int)(projectile.ai[0] - 1)].ai[1] > 328f)
                    {
                        projectile.Kill();
                    }
                }
                projectile.Center = SpaceSquid.GetEyePosition(Main.npc[(int)(projectile.ai[0] - 1)]) + new Vector2(projectile.direction * 10f, 0f);
            }
        }

        public const int LaserLength = 2000;

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (projectile.direction == -1)
            {
                projHitbox.X -= LaserLength + projHitbox.Width;
                projHitbox.Width = LaserLength;
                if (targetHitbox.Intersects(projHitbox))
                {
                    return true;
                }
            }
            else
            {
                projHitbox.Width += LaserLength;
                if (targetHitbox.Intersects(projHitbox))
                {
                    return true;
                }
            }
            return base.Colliding(projHitbox, targetHitbox);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            var texture = Main.projectileTexture[projectile.type];
            var orig = texture.Size() / 2f;
            var drawPos = projectile.Center - Main.screenPosition;
            var drawColor = new Color(60, 255, 60, 0);
            var offset = new Vector2(projectile.width / 2f, projectile.height / 2f);
            var arr = new Vector2[] {
                    projectile.Center - Main.screenPosition,
                    projectile.Center + new Vector2(Main.screenWidth * projectile.direction, 0f) - Main.screenPosition,
                    projectile.Center + new Vector2(Main.screenWidth * 2f * projectile.direction, 0f) - Main.screenPosition, };
            VertexStrip.ReversedGravity(arr);
            var smokeLineColor = drawColor * ((float)Math.Sin(Main.GlobalTime * 12f) + 2f);
            if (AQConfigClient.c_EffectQuality > 0.2f)
            {
                int amount = (int)(30 * AQConfigClient.c_EffectQuality);
                var initialArr = new Vector2[amount];
                var center = projectile.Center;
                initialArr[0] = center - Main.screenPosition;
                for (int i = 1; i < amount; i++)
                {
                    initialArr[i] = center + new Vector2(200f / amount * i * projectile.direction, 0f) - Main.screenPosition;
                }
                VertexStrip.ReversedGravity(initialArr);
                VertexStrip.FullDraw(AQTextures.Trails[TrailTex.ThickLine], VertexStrip.TextureTrail,
                initialArr, (p) => new Vector2(projectile.height * ((1f - p) * (1f - p))), (p) => drawColor * (1f - p));
                initialArr[0] = center - Main.screenPosition;
                for (int i = 1; i < amount; i++)
                {
                    initialArr[i] = center + new Vector2(20f / amount * i * -projectile.direction, 0f) - Main.screenPosition;
                }
                VertexStrip.ReversedGravity(initialArr);
                VertexStrip.FullDraw(AQTextures.Trails[TrailTex.ThickLine], VertexStrip.TextureTrail,
                initialArr, (p) => new Vector2(projectile.height * ((1f - p) * (1f - p))), (p) => drawColor * (1f - p));
                VertexStrip.FullDraw(AQTextures.Trails[TrailTex.SmokeLine], VertexStrip.TextureTrail,
                initialArr, (p) => new Vector2(projectile.height * ((1f - p) * (1f - p))), (p) => smokeLineColor * (1f - p), Main.GlobalTime * 20f, 20f);
            }

            VertexStrip.FullDraw(AQTextures.Trails[TrailTex.ThickLine], VertexStrip.TextureTrail,
                arr, (p) => new Vector2(projectile.height), (p) => drawColor);
            VertexStrip.FullDraw(AQTextures.Trails[TrailTex.SmokeLine], VertexStrip.TextureTrail,
                arr, (p) => new Vector2(projectile.height), (p) => smokeLineColor, Main.GlobalTime * 0.5f, 4f);
            var spotlight = AQTextures.Lights[LightTex.Spotlight100x100];
            Main.spriteBatch.Draw(spotlight, drawPos, null, drawColor * 0.4f, projectile.rotation, spotlight.Size() / 2f, projectile.scale * (projectile.height / 32f), SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(spotlight, drawPos, null, drawColor, projectile.rotation, spotlight.Size() / 2f, projectile.scale * 0.5f * (projectile.height / 32f), SpriteEffects.None, 0f);
            return false;
        }
    }
}