﻿using AQMod.Common;
using AQMod.Common.CrossMod.BossChecklist;
using AQMod.Content.WorldEvents.ProgressBars;
using AQMod.Localization;
using AQMod.NPCs.Monsters.GaleStreams;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace AQMod.Content.WorldEvents.GaleStreams
{
    public sealed class GaleStreams : WorldEvent
    {
        internal override EventProgressBar ProgressBar => new GaleStreamsProgressBar();
        internal override EventEntry? BossChecklistEntry => new EventEntry(
            () => WorldDefeats.DownedGaleStreams,
            6.66f,
            new List<int>() {
                ModContent.NPCType<Vraine>(),
                ModContent.NPCType<WhiteSlime>(),
                ModContent.NPCType<StreamingBalloon>(),
                ModContent.NPCType<RedSprite>(),
            },
            AQText.chooselocalizationtext(en_US: "Gale Streams", zh_Hans: "紊流风暴"),
            0,
            new List<int>()
            {
                ItemID.NimbusRod,
                ModContent.ItemType<Items.Weapons.Melee.Vrang>(),
                ModContent.ItemType<Items.Weapons.Magic.Umystick>(),
                ModContent.ItemType<Items.Tools.Fishing.Nimrod>(),
                ModContent.ItemType<Items.Materials.Energies.AtmosphericEnergy>(),
                ModContent.ItemType<Items.Materials.Fluorescence>(),
                ModContent.ItemType<Items.Foods.GaleStreams.PeeledCarrot>(),
                ModContent.ItemType<Items.Foods.GaleStreams.CinnamonRoll>(),
            },
            new List<int>()
            {
                ModContent.ItemType<Items.Vanities.Dyes.CensorDye>(),
                ModContent.ItemType<Items.Vanities.Dyes.RedSpriteDye>(),
            },
            AQText.chooselocalizationtext(
                en_US: "Begins when the wind is above 40 mph, and ends when it's less than 34 mph. Will also end if the wind goes above 300 mph. You can modify the speed of the wind using [i:" + ModContent.ItemType<Items.Tools.TheFan>() + "]",
                zh_Hans: "风速大于40 mph时开始, 风速小于34 mph时结束. 你可以使用 [i:" + ModContent.ItemType<Items.Tools.TheFan>() + "] 更改风速"),
            "AQMod/Assets/BossChecklist/GaleStreams",
            "AQMod/Assets/EventIcons/GaleStreams");

        public static Color HotCurrentColor => new Color(43, 148, 240, 255);
        public static Color ColdCurrentColor => new Color(255, 94, 31, 255);
        public static Color NeutralCurrentColor => new Color(255, 255, 255, 255);

        public static bool IsActive { get; private set; }
        public static bool EndEvent;
        public static bool EventActive(Player player)
        {
            return IsActive && InSpace(player);
        }

        public static void ProgressEvent(Player player, int points)
        {
            if (!AQMod.SudoHardmode || player.dead || !player.active || EndEvent)
            {
                return;
            }
            Main.windSpeedSet += Math.Sign(Main.windSpeedSet) * points / 100f;
            if (Main.windSpeedSet >= 3f)
            {
                WorldDefeats.DownedGaleStreams = true;
                if (Main.netMode == NetmodeID.Server)
                {
                    NetMessage.SendData(MessageID.WorldData);
                }
                Main.windSpeedSet = 3f;
                EndEvent = true;
            }
            else if (Main.windSpeedSet <= -3f)
            {
                WorldDefeats.DownedGaleStreams = true;
                if (Main.netMode == NetmodeID.Server)
                {
                    NetMessage.SendData(MessageID.WorldData);
                }
                Main.windSpeedSet = -3f;
                EndEvent = true;
            }
            Main.windSpeedTemp = Main.windSpeedSet;
        }

        public static bool MeteorTime()
        {
            if (Main.time < 3600)
                return true;
            if (Main.dayTime)
            {
                return Main.time > Main.dayLength - 3600;
            }
            return Main.time > Main.nightLength - 3600;
        }

        public static bool InMeteorSpawnZone(float y)
        {
            return y < 2000f;
        }

        public static bool InSpace(Player player)
        {
            return InSpace(player.position.Y);
        }

        public static bool InSpace(float y)
        {
            return y < 3000f; // 187.5 tiles
        }

        public override TagCompound Save()
        {
            return new TagCompound() 
            {
                ["EndEvent"] = EndEvent,
            };
        }

        public override void Load(TagCompound tag)
        {
            EndEvent = tag.GetBool("EndEvent");
        }

        public override void PostUpdate()
        {
            IsActive = ImitatedWindyDay.IsItAHappyWindyDay_WindyEnough;
        }

        public static bool CanCrashMeteor(int x, int y, int size = 40)
        {
            int minX = x - size / 2;
            int maxX = x + size / 2;
            int minY = y - size / 2;
            int maxY = y + size / 2;
            for (int i = minX; i < maxX; i++)
            {
                for (int j = minY; j < maxY; j++)
                {
                    if (Main.tile[i, j] == null)
                    {
                        Main.tile[i, j] = new Tile();
                        continue;
                    }
                    if (Main.tileContainer[Main.tile[i, j].type])
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public static void CrashMeteor(int x, int y, int size = 40, int scatter = 1, int scatterAmount = 4, int scatterChance = 25, int holeSizeDivider = 3, bool doEffects = true, ushort tileType = TileID.Meteorite)
        {
            int circularSize = size - 8 - scatter;
            int halfSize = circularSize / 2;
            int minX = x - halfSize;
            int maxX = x + halfSize;
            int minY = y - halfSize;
            int maxY = y + halfSize;
            // draws the circle of the meteorite
            for (int i = minX; i < maxX; i++)
            {
                for (int j = minY; j < maxY; j++)
                {
                    if (Main.tile[i, j] == null)
                    {
                        Main.tile[i, j] = new Tile();
                        continue;
                    }
                    int iX = i - x;
                    int iY = j - y;
                    int distance = (int)Math.Sqrt(iX * iX + iY * iY);
                    if (distance < halfSize)
                    {
                        bool active = Main.tile[i, j].active();
                        int type = Main.tile[i, j].type;
                        if (active && type != tileType)
                        {
                            WorldGen.KillTile(i, j, fail: false, effectOnly: false, noItem: false);
                            if (Main.tileSolid[type])
                            {
                                Main.tile[i, j].active(active: true);
                            }
                        }
                        Main.tile[i, j].type = tileType;
                    }
                }
            }

            halfSize = size / 2 - scatter;
            minX = x - halfSize;
            maxX = x + halfSize;
            minY = y - halfSize;
            maxY = y + halfSize;

            // does some scatter on the outside
            for (int i = minX; i < maxX; i++)
            {
                for (int j = minY; j < maxY; j++)
                {
                    if (Main.tile[i, j] == null)
                    {
                        Main.tile[i, j] = new Tile();
                        continue;
                    }
                    int iX = i - x;
                    int iY = j - y;
                    int distance = (int)Math.Sqrt(iX * iX + iY * iY);
                    if (distance < halfSize && WorldGen.genRand.NextBool(25))
                    {
                        int scatterX = AQUtils.NextVRand(Main.rand, -scatter, scatter);
                        int scatterY = AQUtils.NextVRand(Main.rand, -scatter, scatter);
                        bool active = Main.tile[i + scatterX, j + scatterY].active();
                        int type = Main.tile[i + scatterX, j + scatterY].type;
                        if (type != tileType)
                        {
                            WorldGen.KillTile(i + scatterX, j + scatterY, fail: false, effectOnly: false, noItem: false);
                            if (active && Main.tileSolid[type])
                            {
                                Main.tile[i + scatterX, j + scatterY].active(active: true);
                            }
                        }
                        Main.tile[i + scatterX, j + scatterY].type = tileType;
                    }
                }
            }

            circularSize = size / holeSizeDivider;
            halfSize = circularSize / 2;
            minX = x - circularSize;
            maxX = x + circularSize;
            minY = y - circularSize;
            maxY = y + circularSize;

            // carves a hole in the middle
            for (int i = minX; i < maxX; i++)
            {
                for (int j = minY; j < maxY; j++)
                {
                    if (Main.tile[i, j] == null)
                    {
                        Main.tile[i, j] = new Tile();
                        continue;
                    }
                    int iX = i - x;
                    int iY = j - y;
                    int distance = (int)Math.Sqrt(iX * iX + iY * iY);
                    if (distance < halfSize)
                    {
                        Main.tile[i, j].active(active: false);
                    }
                }
            }

            // does some scatter in the center of the meteorite
            for (int k = 0; k < scatterAmount; k++)
            {
                for (int i = minX; i < maxX; i++)
                {
                    for (int j = minY; j < maxY; j++)
                    {
                        if (Main.tile[i, j] == null)
                        {
                            Main.tile[i, j] = new Tile();
                            continue;
                        }
                        int iX = i - x;
                        int iY = j - y;
                        int distance = (int)Math.Sqrt(iX * iX + iY * iY);
                        if (distance < halfSize)
                        {
                            int scatterX = AQUtils.NextVRand(Main.rand, -1, 1);
                            int scatterY = AQUtils.NextVRand(Main.rand, -1, 1);
                            Main.tile[i + scatterX, j + scatterY].active(active: false);
                        }
                    }
                }
            }

            halfSize = size / 2;
            minX = x - halfSize;
            maxX = x + halfSize;
            minY = y - halfSize;
            maxY = y + halfSize;
            // runs square tile frame on everything here
            for (int i = minX; i < maxX; i++)
            {
                for (int j = minY; j < maxY; j++)
                {
                    if (Main.tile[i, j] == null)
                    {
                        Main.tile[i, j] = new Tile();
                        continue;
                    }
                    int iX = i - x;
                    int iY = j - y;
                    int distance = (int)Math.Sqrt(iX * iX + iY * iY);
                    if (distance < halfSize)
                    {
                        if (Main.tile[i, j].active() && Main.tile[i, j].type == tileType)
                        {
                            WorldGen.SquareTileFrame(i, j, true);
                        }
                    }
                }
            }

            AQMod.BroadcastMessage(Lang.gen[59].Key, Constants.ChatColors.EventMessage);
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                NetMessage.SendTileSquare(-1, minX, minY, size);
            }
        }
    }
}