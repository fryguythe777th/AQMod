﻿using Aequus.Common.Utilities;
using Aequus.Items.Weapons.Melee;
using Aequus.NPCs.Boss;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.Graphics.Effects;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Biomes.Glimmer
{
    public class GlimmerScene : ModSceneEffect
    {
        public static bool renderedUltimateSword;
        public static int cantTouchThis;
        public static Vector2 ultimateSwordWorldDrawLocation;
        public static int EatenAlpha;

        public override void Load()
        {
            if (!Main.dedServ)
            {
                SkyManager.Instance[GlimmerSky.Key] = new GlimmerSky();
            }
        }

        public override bool IsSceneEffectActive(Player player)
        {
            return player.InModBiome<GlimmerBiome>() || GlimmerBiome.omegaStarite != -1;
        }

        public override float GetWeight(Player player)
        {
            return GlimmerSystem.CalcTiles(player) < GlimmerBiome.UltraStariteSpawn ? 1f : 0.5f;
        }

        public override void SpecialVisuals(Player player, bool isActive)
        {
            if (isActive)
            {
                if (!SkyManager.Instance[GlimmerSky.Key].IsActive())
                {
                    SkyManager.Instance.Activate(GlimmerSky.Key);
                }
            }
            else
            {
                if (SkyManager.Instance[GlimmerSky.Key].IsActive())
                {
                    SkyManager.Instance.Deactivate(GlimmerSky.Key);
                }
            }
        }

        public static void Draw()
        {
            if (!GlimmerBiome.EventActive)
            {
                renderedUltimateSword = false;
                ultimateSwordWorldDrawLocation = Vector2.Zero;
                return;
            }
            var gotoPosition = GlimmerBiome.TileLocation.ToWorldCoordinates() + new Vector2(8f, -60f);
            if (!renderedUltimateSword)
            {
                ultimateSwordWorldDrawLocation = gotoPosition;
            }
            else
            {
                if (ultimateSwordWorldDrawLocation.X > gotoPosition.X)
                {
                    ultimateSwordWorldDrawLocation.X--;
                    if (ultimateSwordWorldDrawLocation.X < gotoPosition.X)
                    {
                        ultimateSwordWorldDrawLocation.X = gotoPosition.X;
                    }
                }
                else if (ultimateSwordWorldDrawLocation.X < gotoPosition.X)
                {
                    ultimateSwordWorldDrawLocation.X++;
                    if (ultimateSwordWorldDrawLocation.X > gotoPosition.X)
                    {
                        ultimateSwordWorldDrawLocation.X = gotoPosition.X;
                    }
                }

                if (ultimateSwordWorldDrawLocation.Y > gotoPosition.Y)
                {
                    ultimateSwordWorldDrawLocation.Y -= 4;
                    if (ultimateSwordWorldDrawLocation.Y < gotoPosition.Y)
                    {
                        ultimateSwordWorldDrawLocation.Y = gotoPosition.Y;
                    }
                }
                else if (ultimateSwordWorldDrawLocation.Y < gotoPosition.Y)
                {
                    ultimateSwordWorldDrawLocation.Y += 4;
                    if (ultimateSwordWorldDrawLocation.Y > gotoPosition.Y)
                    {
                        ultimateSwordWorldDrawLocation.Y = gotoPosition.Y;
                    }
                }
            }
            renderedUltimateSword = false;
            ScreenCulling.SetPadding(400);
            if (!ScreenCulling.OnScreenWorld(ultimateSwordWorldDrawLocation) && !ScreenCulling.OnScreenWorld(gotoPosition))
            {
                return;
            }

            var drawCoords = ultimateSwordWorldDrawLocation - Main.screenPosition + new Vector2(0f, AequusHelpers.Wave(Main.GlobalTimeWrappedHourly * 0.5f, -10f, 10f));
            Main.instance.LoadItem(ModContent.ItemType<UltimateSword>());
            var texture = TextureAssets.Item[ModContent.ItemType<UltimateSword>()].Value;
            Main.spriteBatch.Draw(texture, drawCoords, null, Color.White, MathHelper.PiOver4 * 3f, new Vector2(texture.Width, 0f), 1f, SpriteEffects.None, 0f);
            var interactionRect = new Rectangle((int)drawCoords.X - 12, (int)drawCoords.Y - 70, 24, 70);
            if (Main.SmartCursorIsUsed)
            {
                interactionRect.X -= 45;
                interactionRect.Y -= 32;
                interactionRect.Width += 90;
                interactionRect.Height += 64;
            }
            var mouseScreen = AequusHelpers.ScaledMouseScreen;
            bool hovering = interactionRect.Contains(mouseScreen.ToPoint());
            AequusHelpers.DrawRectangle(interactionRect, hovering ? Color.Yellow * 0.2f : Color.Red * 0.2f);
            if (hovering)
            {
                if (Main.SmartCursorIsUsed)
                {
                    Main.SmartCursorX = 0;
                    Main.SmartCursorY = 0;
                }
                var mouseWorld = mouseScreen + Main.screenPosition;
                if (!NPC.AnyNPCs(ModContent.NPCType<OmegaStarite>()) && Aequus.GameWorldActive && Main.LocalPlayer.IsInTileInteractionRange((int)mouseWorld.X / 16, (int)mouseWorld.Y / 16))
                {
                    var plr = Main.LocalPlayer;
                    plr.noThrow = 2;
                    plr.cursorItemIconEnabled = true;
                    plr.cursorItemIconID = ModContent.ItemType<UltimateSword>();
                    //var highlightTexture = ModContent.GetTexture(AQUtils.GetPath<UltimateSword>("_Highlight"));
                    //Main.spriteBatch.Draw(highlightTexture, drawPos - Main.screenPosition, frame, new Color(255, 255, 255, 255), MathHelper.PiOver4 * 3f, origin, 1f, SpriteEffects.None, 0f);
                    if (Main.mouseRight && Main.mouseRightRelease)
                    {
                        cantTouchThis = 480;
                        plr.tileInteractAttempted = true;
                        plr.tileInteractionHappened = true;
                        plr.releaseUseTile = false;
                        drawCoords += Main.screenPosition;
                        if (Main.netMode == NetmodeID.SinglePlayer)
                        {
                            NPC.SpawnBoss((int)drawCoords.X, (int)drawCoords.Y - 1600, ModContent.NPCType<OmegaStarite>(), Main.myPlayer);
                        }
                        else
                        {

                        }
                        SoundEngine.PlaySound(SoundID.Roar, Main.LocalPlayer.Center);
                    }
                }
            }
            renderedUltimateSword = true;
        }
    }
}