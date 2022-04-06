﻿using Aequus.Common;
using Aequus.Content.Invasions;
using Aequus.NPCs.Boss;
using Aequus.NPCs.Monsters.Sky;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.Utilities;

namespace Aequus.NPCs
{
    public sealed class NPCSpawns : GlobalNPC
    {
        public override void EditSpawnRate(Player player, ref int spawnRate, ref int maxSpawns)
        {
            if (NoSpawns_CheckBosses(player) || NoSpawns_CheckEvents(player))
            {
                spawnRate *= 10000;
                maxSpawns = 0;
                return;
            }
            if (player.ZoneSkyHeight && GaleStreams.TimeForMeteorSpawns())
            {
                spawnRate /= 2;
                maxSpawns *= 2;
            }
            if (player.GetModPlayer<AequusPlayer>().eventGaleStreams)
            {
                spawnRate /= 2;
            }
            if (IsClose<RedSprite>(player) || IsClose<SpaceSquid>(player))
            {
                spawnRate *= 3;
                maxSpawns = Math.Min(maxSpawns, 2);
            }
        }

        private bool NoSpawns_CheckBosses(Player player)
        {
            return IsClose<OmegaStarite>(player) || IsClose<OmegaStarite>(player);
        }
        private bool NoSpawns_CheckEvents(Player player)
        {
            return Glimmer.Status == InvasionStatus.Ending && player.position.Y < Main.worldSurface * 16f;
        }

        public override void EditSpawnPool(IDictionary<int, float> pool, NPCSpawnInfo spawnInfo)
        {
            if (spawnInfo.Player.ZoneSkyHeight && GaleStreams.TimeForMeteorSpawns())
            {
                AdjustSpawns(pool, 0.75f);
                pool.Add(ModContent.NPCType<Meteor>(), 2f);
            }
            if (spawnInfo.Player.GetModPlayer<AequusPlayer>().eventGaleStreams && !spawnInfo.PlayerSafe)
            {
                AdjustSpawns(pool, MathHelper.Lerp(1f, 0.25f, SpawnCondition.Sky.Chance));
                if (WorldFlags.HardmodeTier && !(IsClose<RedSprite>(spawnInfo.Player) || IsClose<SpaceSquid>(spawnInfo.Player)))
                {
                    pool.Add(ModContent.NPCType<RedSprite>(), 0.06f * SpawnCondition.Sky.Chance);
                    pool.Add(ModContent.NPCType<SpaceSquid>(), 0.06f * SpawnCondition.Sky.Chance);
                }
                if (NPC.CountNPCS(ModContent.NPCType<Vraine>()) < 2)
                    pool.Add(ModContent.NPCType<Vraine>(), 1f * SpawnCondition.Sky.Chance);
                if (WorldGen.SolidTile(spawnInfo.SpawnTileX, spawnInfo.SpawnTileY))
                {
                    pool.Add(ModContent.NPCType<WhiteSlime>(), 0.3f * SpawnCondition.Sky.Chance);
                }
                pool.Add(ModContent.NPCType<StreamingBalloon>(), 0.6f * SpawnCondition.Sky.Chance);
            }
        }

        private void AdjustSpawns(IDictionary<int, float> pool, float amt)
        {
            var enumerator = pool.GetEnumerator();
            while (enumerator.MoveNext())
            {
                pool[enumerator.Current.Key] *= amt;
            }
        }

        public static bool IsClose<T>(Player player) where T : ModNPC
        {
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                if (Main.npc[i].active && Main.npc[i].type == ModContent.NPCType<T>())
                {
                    if (Main.npc[i].Distance(player.Center) < 2000f)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}