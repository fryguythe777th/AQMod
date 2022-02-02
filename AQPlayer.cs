﻿using AQMod.Buffs.Debuffs;
using AQMod.Buffs.Temperature;
using AQMod.Common.Graphics;
using AQMod.Common.ID;
using AQMod.Content;
using AQMod.Content.Fishing;
using AQMod.Content.World;
using AQMod.Dusts;
using AQMod.Effects;
using AQMod.Effects.Particles;
using AQMod.Effects.ScreenEffects;
using AQMod.Items;
using AQMod.Items.Accessories.Amulets;
using AQMod.Items.Accessories.FishingSeals;
using AQMod.Items.Armor.Arachnotron;
using AQMod.Items.Quest.Angler;
using AQMod.Items.Tools.Axe;
using AQMod.NPCs;
using AQMod.Projectiles;
using AQMod.Projectiles.Summon;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Achievements;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.Utilities;

namespace AQMod
{
    public sealed class AQPlayer : ModPlayer
    {
        public const int MaxCelesteTorusOrbs = 5;
        public const int MaxArmor = 20;
        public const int MaxDye = MaxArmor / 2;
        public const int FRAME_HEIGHT = 56;
        public const int FRAME_COUNT = 20;
        public const float CELESTE_Z_MULT = 0.0157f;
        public const int ARACHNOTRON_OLD_POS_LENGTH = 8;
        public const byte TEMPERATURE_REGEN_NORMAL = 32;
        public const byte TEMPERATURE_REGEN_FROST_ARMOR_COLD_TEMP = 20;
        public const byte TEMPERATURE_REGEN_ON_HIT = 120;

        public static bool Fidget_Spinner_Force_Autoswing { get; internal set; }
        public static bool IsQuickBuffing { get; internal set; }

        public float discountPercentage;
        public bool blueSpheres;
        public bool hyperCrystal;
        public bool sparkling;
        public bool chloroTransfer;
        public bool altEvilDrops;
        public bool breadsoul;
        public bool moonShoes;
        public bool extractinator;
        public bool copperSeal;
        public bool silverSeal;
        public bool goldSeal;
        public bool canDash;
        public bool dartHead;
        public int dartHeadType;
        public int dartHeadDelay;
        public int dartTrapHatTimer;
        public int extraFlightTime;
        public int thunderbirdJumpTimer;
        public int thunderbirdLightningTimer;
        public bool dreadsoul;
        public bool arachnotron;
        public bool primeTime;
        public bool omori;
        public int omoriDeathTimer;
        public int spelunkerEquipTimer;
        public bool omegaStarite;
        public byte lootIterations;
        public bool wyvernAmulet;
        public bool voodooAmulet;
        public bool ghostAmulet;
        public bool extractinatorVisible;
        public float celesteTorusX;
        public float celesteTorusY;
        public float celesteTorusZ;
        public float celesteTorusRadius;
        public int celesteTorusDamage;
        public float celesteTorusKnockback;
        public int celesteTorusMaxRadius;
        public float celesteTorusSpeed;
        public float celesteTorusScale;
        public bool spicyEel;
        public bool striderPalms;
        public bool striderPalmsOld;
        public bool wyvernAmuletHeld;
        public bool voodooAmuletHeld;
        public bool ghostAmuletHeld;
        public bool degenerationRing;
        public ushort shieldLife;
        public bool notFrostburn;
        public bool bossrush;
        public bool bossrushOld;
        public float grabReachMult; // until 1.4 comes
        public bool neutronYogurt;
        public bool mothmanMask;
        public byte mothmanExplosionDelay;
        public sbyte temperature;
        public byte temperatureRegen;
        public bool pickBreak;
        public bool crabAx;
        public sbyte redSpriteWind;
        public byte extraHP;
        public bool fidgetSpinner;
        public bool cantUseMenaceUmbrellaJump;
        public bool ignoreMoons;
        public bool cosmicanon;
        public bool antiGravityItems;
        public bool hotAmulet;
        public bool coldAmulet;
        public bool shockCollar;
        public bool healBeforeDeath;
        public bool glowString;
        public bool pearlAmulet;
        public bool darkAmulet;
        public bool lightAmulet;
        public bool bloodthirst;
        public byte bloodthirstDelay;
        public bool spreadDebuffs;
        public bool shade;
        public bool undetectable;

        public float evilEnemyDR;
        public float holyEnemyDR;
        public int healEffectValueForSyncingTheThingOnTheServer;

        public bool setBonusLightbulb;

        public bool heartMoth;
        public bool anglerFish;

        public bool crimsonHands;
        public bool chomper;
        public bool piranhaPlant;
        public bool trapperImp;
        public bool starite;
        public bool monoxiderBird;

        public bool hasMinionCarry;
        public int headMinionCarryX;
        public int headMinionCarryY;
        public int headMinionCarryXOld;
        public int headMinionCarryYOld;
        public byte monoxiderCarry;

        public int popperType;
        public int popperBaitPower;
        public int fishingPowerCache;

        public int ExtractinatorCount;

        public bool IgnoreIgnoreMoons;

        public override void Initialize()
        {
            omoriDeathTimer = 1;
            arachnotron = false;
            lootIterations = 0;
            sparkling = false;
            headMinionCarryX = 0;
            headMinionCarryY = 0;
            headMinionCarryXOld = 0;
            headMinionCarryYOld = 0;
            monoxiderCarry = 0;
            notFrostburn = false;
            bossrush = false;
            bossrushOld = false;
            grabReachMult = 1f;
            temperature = 0;
            pickBreak = false;
            fidgetSpinner = false;
            cantUseMenaceUmbrellaJump = false;
            bloodthirstDelay = 0;
            healEffectValueForSyncingTheThingOnTheServer = 0;
        }

        public override void OnEnterWorld(Player player)
        {
            if (!Main.dayTime && Main.netMode != NetmodeID.MultiplayerClient && Main.myPlayer == player.whoAmI)
                SkyGlimmerEvent.InitNight();
        }

        public override TagCompound Save()
        {
            return new TagCompound()
            {
                ["extractinatorCount"] = ExtractinatorCount,
                ["IgnoreIgnoreMoons"] = IgnoreIgnoreMoons,
            };
        }

        public override void Load(TagCompound tag)
        {
            IgnoreIgnoreMoons = tag.GetBool("IgnoreIgnoreMoons");
            ExtractinatorCount = tag.GetInt("extractinatorCount");
        }

        public override void UpdateBiomeVisuals()
        {
            if (!Main.gamePaused && Main.instance.IsActive)
                ScreenShakeManager.Update();
        }

        private void UpdateTemperature()
        {
            if (temperature != 0)
            {
                sbyte minTemp = -100;
                sbyte maxTemp = 100;
                if (coldAmulet)
                {
                    minTemp = -60;
                }
                if (hotAmulet)
                {
                    maxTemp = 60;
                }
                if (temperature < minTemp)
                {
                    temperature = minTemp;
                }
                else if (temperature > maxTemp)
                {
                    temperature = maxTemp;
                }
                if (temperatureRegen == 0)
                {
                    temperatureRegen = TEMPERATURE_REGEN_NORMAL;
                    if (player.resistCold && temperature < 0)
                    {
                        temperatureRegen = TEMPERATURE_REGEN_FROST_ARMOR_COLD_TEMP;
                    }
                    if (temperature < 0)
                    {
                        temperature++;
                    }
                    else
                    {
                        temperature--;
                    }
                }
                else
                {
                    temperatureRegen--;
                }
            }
            else
            {
                temperatureRegen = TEMPERATURE_REGEN_ON_HIT;
            }
            if (EventGaleStreams.EventActive(player))
            {
                if (temperature < -60)
                {
                    player.AddBuff(ModContent.BuffType<Cold60>(), 4);
                }
                else if (temperature < -40)
                {
                    player.AddBuff(ModContent.BuffType<Cold40>(), 4);
                }
                else if (temperature < -20)
                {
                    player.AddBuff(ModContent.BuffType<Cold20>(), 4);
                }
                else if (temperature > 60)
                {
                    player.AddBuff(ModContent.BuffType<Hot60>(), 4);
                }
                else if (temperature > 40)
                {
                    player.AddBuff(ModContent.BuffType<Hot40>(), 4);
                }
                else if (temperature > 20)
                {
                    player.AddBuff(ModContent.BuffType<Hot20>(), 4);
                }
            }
        }

        public override void ResetEffects()
        {
            setBonusLightbulb = false;
            blueSpheres = false;
            discountPercentage = 0.8f;
            hyperCrystal = false;
            monoxiderBird = false;
            sparkling = false;
            moonShoes = false;
            canDash = !(player.setSolar || player.mount.Active);
            copperSeal = false;
            silverSeal = false;
            goldSeal = false;
            extraFlightTime = 0;
            dreadsoul = false;
            breadsoul = false;
            arachnotron = false;
            primeTime = false;
            omori = false;
            omegaStarite = false;
            lootIterations = 0;
            wyvernAmulet = false;
            voodooAmulet = false;
            ghostAmulet = false;
            extractinatorVisible = false;
            altEvilDrops = false;
            starite = false;
            spicyEel = false;
            striderPalmsOld = striderPalms;
            striderPalms = false;
            ghostAmuletHeld = InVanitySlot(player, ModContent.ItemType<GhostAmulet>());
            voodooAmuletHeld = InVanitySlot(player, ModContent.ItemType<VoodooAmulet>());
            wyvernAmuletHeld = InVanitySlot(player, ModContent.ItemType<WyvernAmulet>());
            shieldLife = 0;

            crimsonHands = false;
            chomper = false;
            piranhaPlant = false;
            trapperImp = false;

            headMinionCarryXOld = headMinionCarryX;
            headMinionCarryYOld = headMinionCarryY;
            headMinionCarryX = 0;
            headMinionCarryY = 0;
            monoxiderCarry = 0;

            heartMoth = false;
            anglerFish = false;

            notFrostburn = false;
            grabReachMult = 1f;
            mothmanMask = false;
            pickBreak = false;
            crabAx = false;
            fidgetSpinner = false;
            cantUseMenaceUmbrellaJump = false;
            cosmicanon = false;
            ignoreMoons = false;
            antiGravityItems = false;
            shockCollar = false;
            healBeforeDeath = false;
            glowString = false;
            pearlAmulet = false;
            lightAmulet = false;
            darkAmulet = false;
            bloodthirst = false;
            shade = false;
            spreadDebuffs = false;

            if (extraHP > 60) // to cap life max buffs at 60
            {
                extraHP = 60;
            }
            player.statLifeMax2 += extraHP;
            extraHP = 0;
            UpdateTemperature();
            hotAmulet = false;
            coldAmulet = false;
            if (mothmanExplosionDelay > 0)
                mothmanExplosionDelay--;
            if (bloodthirstDelay > 0)
                bloodthirstDelay--;
            bossrushOld = bossrush;
            bossrush = false;
            if (!dartHead)
                dartTrapHatTimer = 240;
            dartHead = false;
            if (thunderbirdJumpTimer > 0)
            {
                canDash = false;
                thunderbirdJumpTimer--;
            }
            if (thunderbirdLightningTimer > 0)
                thunderbirdLightningTimer--;
            if (canDash)
            {
                for (int i = 3; i < 8 + player.extraAccessorySlots; i++)
                {
                    Item item = player.armor[i];
                    if (item.type == ItemID.EoCShield || item.type == ItemID.MasterNinjaGear || item.type == ItemID.Tabi)
                    {
                        canDash = false;
                        break;
                    }
                }
            }
            if (healEffectValueForSyncingTheThingOnTheServer != 0 && Main.myPlayer == player.whoAmI)
            {
                player.HealEffect(healEffectValueForSyncingTheThingOnTheServer, broadcast: true);
                healEffectValueForSyncingTheThingOnTheServer = 0;
            }
        }

        public override void ProcessTriggers(TriggersSet triggersSet)
        {
            if (cosmicanon && AQMod.Keybinds.CosmicanonToggle.JustPressed)
            {
                IgnoreIgnoreMoons = !IgnoreIgnoreMoons;
                if (IgnoreIgnoreMoons)
                {
                    Main.NewText(Language.GetTextValue("Mods.AQMod.ToggleCosmicanon.False"), new Color(230, 230, 255, 255));
                }
                else
                {
                    Main.NewText(Language.GetTextValue("Mods.AQMod.ToggleCosmicanon.True"), new Color(230, 230, 255, 255));
                }
            }
        }

        public override void clientClone(ModPlayer clientClone)
        {
            var clone = (AQPlayer)clientClone;
            clone.celesteTorusX = celesteTorusX;
            clone.celesteTorusY = celesteTorusY;
            clone.celesteTorusZ = celesteTorusZ;
            clone.breadsoul = breadsoul;
            clone.dreadsoul = dreadsoul;
            clone.dartHead = dartHead;
            clone.dartHeadType = dartHeadType;
            clone.arachnotron = arachnotron;
            clone.blueSpheres = blueSpheres;
        }



        public override bool PreItemCheck()
        {
            if (Main.myPlayer == player.whoAmI)
            {
                var item = player.inventory[player.selectedItem];
                Fidget_Spinner_Force_Autoswing = false;
                if (fidgetSpinner && player.selectedItem < Main.maxInventory && (Main.mouseItem == null || Main.mouseItem.type <= ItemID.None))
                {
                    if (CanForceAutoswing(player, item, ignoreChanneled: false))
                    {
                        Fidget_Spinner_Force_Autoswing = true;
                        item.autoReuse = true;
                    }
                }
                bool canMine = CanReach(player, item);
                if (player.noBuilding)
                    canMine = false;
                if (Main.mouseRight || !canMine)
                    crabAx = false;
                else if (!crabAx)
                    crabAx = item.type == ModContent.ItemType<Crabax>();
                if (crabAx && (item.axe > 0))
                {
                    if (Main.tile[Player.tileTargetX, Player.tileTargetY].active() && player.toolTime <= 1 && player.itemAnimation > 0 && player.controlUseItem)
                    {
                        var rectangle = new Rectangle((int)(player.position.X + player.width / 2) / 16, (int)(player.position.Y + player.height / 2) / 16, 30, 30);
                        rectangle.X -= rectangle.Width / 2;
                        rectangle.Y -= rectangle.Height / 2;
                        int hitCount = 0;
                        const int HitCountMax = 8;
                        if (rectangle.X > 10 && rectangle.X < Main.maxTilesX - 10 && rectangle.Y > 10 && rectangle.Y < Main.maxTilesY - 10)
                        {
                            for (int i = rectangle.X; i < rectangle.X + rectangle.Width; i++)
                            {
                                for (int j = rectangle.Y; j < rectangle.Y + rectangle.Height; j++)
                                {
                                    if (Main.tile[i, j] == null)
                                    {
                                        Main.tile[i, j] = new Tile();
                                        continue;
                                    }
                                    if (Main.tile[i, j].active() && Main.tileAxe[Main.tile[i, j].type])
                                    {
                                        int tileID = player.hitTile.HitObject(i, j, 1);
                                        int tileDamage = 0;
                                        if (Main.tile[i, j].type == 80)
                                        {
                                            tileDamage += item.axe * 3;
                                        }
                                        else
                                        {
                                            TileLoader.MineDamage(item.axe, ref tileDamage);
                                        }
                                        if (Main.tile[i, j].type == TileID.Trees)
                                        {
                                            int treeStumpX = i;
                                            int treeStumpY = j;

                                            if (Main.tile[treeStumpX, treeStumpY].frameY >= 198 && Main.tile[treeStumpX, treeStumpY].frameX == 44)
                                            {
                                                treeStumpX++;
                                            }
                                            if (Main.tile[treeStumpX, treeStumpY].frameX == 66 && Main.tile[treeStumpX, treeStumpY].frameY <= 44)
                                            {
                                                treeStumpX++;
                                            }
                                            if (Main.tile[treeStumpX, treeStumpY].frameX == 44 && Main.tile[treeStumpX, treeStumpY].frameY >= 132 && Main.tile[treeStumpX, treeStumpY].frameY <= 176)
                                            {
                                                treeStumpX++;
                                            }
                                            if (Main.tile[treeStumpX, treeStumpY].frameY >= 198 && Main.tile[treeStumpX, treeStumpY].frameX == 66)
                                            {
                                                treeStumpX--;
                                            }
                                            if (Main.tile[treeStumpX, treeStumpY].frameX == 88 && Main.tile[treeStumpX, treeStumpY].frameY >= 66 && Main.tile[treeStumpX, treeStumpY].frameY <= 110)
                                            {
                                                treeStumpX--;
                                            }
                                            if (Main.tile[treeStumpX, treeStumpY].frameX == 22 && Main.tile[treeStumpX, treeStumpY].frameY >= 132 && Main.tile[treeStumpX, treeStumpY].frameY <= 176)
                                            {
                                                treeStumpX--;
                                            }

                                            i = treeStumpX + 2; // skips the current index and the next one, since this entire tree has been completed
                                            j = rectangle.Y;

                                            for (; Main.tile[treeStumpX, treeStumpY].active() && Main.tile[treeStumpX, treeStumpY].type == TileID.Trees && Main.tile[treeStumpX, treeStumpY + 1].type == TileID.Trees; treeStumpY++)
                                            {
                                            }

                                            if (Player.tileTargetX == treeStumpX && Player.tileTargetY == treeStumpY)
                                            {
                                                break;
                                            }

                                            AchievementsHelper.CurrentlyMining = true;
                                            if (!WorldGen.CanKillTile(treeStumpX, treeStumpY))
                                            {
                                                tileDamage = 0;
                                            }
                                            tileID = player.hitTile.HitObject(treeStumpX, treeStumpY, 1);
                                            if (player.hitTile.AddDamage(tileID, tileDamage) >= 100)
                                            {
                                                player.hitTile.Clear(tileID);
                                                WorldGen.KillTile(treeStumpX, treeStumpY);
                                                if (Main.netMode == NetmodeID.MultiplayerClient)
                                                {
                                                    NetMessage.SendData(MessageID.TileChange, -1, -1, null, 0, treeStumpX, treeStumpY);
                                                }
                                            }
                                            else
                                            {
                                                WorldGen.KillTile(i, j, fail: true);
                                                if (Main.netMode == NetmodeID.MultiplayerClient)
                                                {
                                                    NetMessage.SendData(MessageID.TileChange, -1, -1, null, 0, treeStumpX, treeStumpY, 1f);
                                                }
                                            }
                                            if (tileDamage != 0)
                                            {
                                                player.hitTile.Prune();
                                                hitCount++;
                                                if (hitCount > HitCountMax)
                                                {
                                                    break;
                                                }
                                            }
                                            AchievementsHelper.CurrentlyMining = false;
                                            continue;
                                        }
                                        else if (Main.tile[i, j].type == TileID.PalmTree)
                                        {
                                            int treeStumpX = i;
                                            int treeStumpY = j;

                                            for (; Main.tile[treeStumpX, treeStumpY].active() && Main.tile[treeStumpX, treeStumpY].type == TileID.PalmTree && Main.tile[treeStumpX, treeStumpY + 1].type == TileID.PalmTree; treeStumpY++)
                                            {
                                            }

                                            i = treeStumpX + 2; // skips the current index and the next one, since this entire tree has been completed
                                            j = rectangle.Y;

                                            if (Player.tileTargetX == treeStumpX && Player.tileTargetY == treeStumpY)
                                            {
                                                break;
                                            }

                                            AchievementsHelper.CurrentlyMining = true;
                                            if (!WorldGen.CanKillTile(treeStumpX, treeStumpY))
                                            {
                                                tileDamage = 0;
                                            }
                                            tileID = player.hitTile.HitObject(treeStumpX, treeStumpY, 1);
                                            if (player.hitTile.AddDamage(tileID, tileDamage) >= 100)
                                            {
                                                player.hitTile.Clear(tileID);
                                                WorldGen.KillTile(treeStumpX, treeStumpY);
                                                if (Main.netMode == NetmodeID.MultiplayerClient)
                                                {
                                                    NetMessage.SendData(MessageID.TileChange, -1, -1, null, 0, treeStumpX, treeStumpY);
                                                }
                                            }
                                            else
                                            {
                                                WorldGen.KillTile(i, j, fail: true);
                                                if (Main.netMode == NetmodeID.MultiplayerClient)
                                                {
                                                    NetMessage.SendData(MessageID.TileChange, -1, -1, null, 0, treeStumpX, treeStumpY, 1f);
                                                }
                                            }
                                            if (tileDamage != 0)
                                            {
                                                player.hitTile.Prune();
                                                hitCount++;
                                                if (hitCount > HitCountMax)
                                                {
                                                    break;
                                                }
                                            }
                                            AchievementsHelper.CurrentlyMining = false;
                                            continue;
                                        }
                                        else
                                        {
                                            AchievementsHelper.CurrentlyMining = true;
                                            if (!WorldGen.CanKillTile(i, j))
                                            {
                                                tileDamage = 0;
                                            }
                                            if (player.hitTile.AddDamage(tileID, tileDamage) >= 100)
                                            {
                                                player.hitTile.Clear(tileID);
                                                WorldGen.KillTile(i, j);
                                                if (Main.netMode == NetmodeID.MultiplayerClient)
                                                {
                                                    NetMessage.SendData(MessageID.TileChange, -1, -1, null, 0, i, j);
                                                }
                                            }
                                            else
                                            {
                                                WorldGen.KillTile(i, j, fail: true);
                                                if (Main.netMode == NetmodeID.MultiplayerClient)
                                                {
                                                    NetMessage.SendData(MessageID.TileChange, -1, -1, null, 0, i, j, 1f);
                                                }
                                            }
                                            if (tileDamage != 0)
                                            {
                                                player.hitTile.Prune();
                                                hitCount++;
                                                if (hitCount > HitCountMax)
                                                {
                                                    break;
                                                }
                                            }
                                            AchievementsHelper.CurrentlyMining = false;
                                        }
                                    }
                                }
                                if (hitCount > HitCountMax)
                                {
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            return base.PreItemCheck();
        }

        public override void PostItemCheck()
        {
            if (Main.myPlayer == player.whoAmI)
            {
                if (Fidget_Spinner_Force_Autoswing)
                {
                    player.inventory[player.selectedItem].autoReuse = false;
                    Fidget_Spinner_Force_Autoswing = false;
                }
            }
        }

        public override float UseTimeMultiplier(Item item)
        {
            if (Fidget_Spinner_Force_Autoswing && item.damage > 0 && !item.channel)
            {
                if (item.useTime <= 10)
                {
                    return 0.3f;
                }
                if (item.useTime < 32)
                {
                    return 0.6f;
                }
            }
            return base.UseTimeMultiplier(item);
        }

        public override void SendClientChanges(ModPlayer clientPlayer)
        {
            var clone = (AQPlayer)clientPlayer;
            if (clone.bossrush)
            {
                SyncPlayer(-1, player.whoAmI, true);
            }
            else
            {
            }
        }

        public override void UpdateDead()
        {
            omori = false;
            blueSpheres = false;
            sparkling = false;
            monoxiderCarry = 0;
            temperature = 0;
            temperatureRegen = TEMPERATURE_REGEN_ON_HIT;
            notFrostburn = false;
            mothmanExplosionDelay = 0;
            dartHeadDelay = 0;
            bloodthirstDelay = 0;
            healEffectValueForSyncingTheThingOnTheServer = 0;
        }

        public override bool Shoot(Item item, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            if (chloroTransfer && type == ProjectileID.Bullet && Main.rand.NextBool(8))
                type = ProjectileID.ChlorophyteBullet;
            return true;
        }

        public override bool PreKill(double damage, int hitDirection, bool pvp, ref bool playSound, ref bool genGore, ref PlayerDeathReason damageSource)
        {
            if (healBeforeDeath && player.potionDelay <= 0)
            {
                if (player.statLife < 0)
                {
                    player.statLife = 1;
                }
                player.QuickHeal();
                return false;
            }
            if (omori)
            {
                if (omoriDeathTimer <= 0)
                {
                    Main.PlaySound(SoundID.Item60, player.position);
                    player.statLife = 1;
                    player.immune = true;
                    player.immuneTime = 120;
                    omoriDeathTimer = 18000;
                    return false;
                }
            }
            return true;
        }

        public override void PostUpdateBuffs()
        {
            monoxiderCarry = 0;
            var monoxider = ModContent.ProjectileType<Monoxider>();
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile p = Main.projectile[i];
                if (p.active && p.type == monoxider && p.ai[0] > 0f)
                    monoxiderCarry++;
            }
        }

        public override void UpdateEquips(ref bool wallSpeedBuff, ref bool tileSpeedBuff, ref bool tileRangeBuff)
        {
            if (Main.myPlayer == player.whoAmI)
            {
                for (int i = 0; i < Chest.maxItems; i++)
                {
                    if (player.bank.item[i].type > Main.maxItemTypes && player.bank.item[i].modItem is IUpdatePiggybank update)
                        update.UpdatePiggyBank(player, i);
                    if (player.bank2.item[i].type > Main.maxItemTypes && player.bank2.item[i].modItem is IUpdatePlayerSafe update2)
                        update2.UpdatePlayerSafe(player, i);
                }
            }
            UpdateCelesteTorus();
            if (player.wingsLogic > 0)
                player.wingTimeMax += extraFlightTime;
        }

        public override void PostUpdateEquips()
        {
            if (dartHead)
            {
                if (player.velocity.Y == 0f)
                    dartTrapHatTimer--;
                if (dartTrapHatTimer <= 0)
                {
                    dartTrapHatTimer = dartHeadDelay;
                    int damage = player.GetWeaponDamage(player.armor[0]);
                    var spawnPosition = player.gravDir == -1
                        ? player.position + new Vector2(player.width / 2f + 8f * player.direction, player.height)
                        : player.position + new Vector2(player.width / 2f + 8f * player.direction, 0f);
                    int p = Projectile.NewProjectile(spawnPosition, new Vector2(10f, 0f) * player.direction, dartHeadType, damage, player.armor[0].knockBack * player.minionKB, player.whoAmI);
                    Main.projectile[p].hostile = false;
                    Main.projectile[p].friendly = true;
                }
            }
            if (arachnotron)
            {
                if (Main.myPlayer == player.whoAmI)
                {
                    int type = ModContent.ProjectileType<ArachnotronLegs>();
                    if (player.ownedProjectileCounts[type] <= 0)
                    {
                        int p = Projectile.NewProjectile(player.Center, Vector2.Zero, type, 33, 1f, player.whoAmI);
                        Main.projectile[p].netUpdate = true;
                    }
                }
            }
            if (omori)
            {
                if (omoriDeathTimer > 0)
                {
                    omoriDeathTimer--;
                    if (omoriDeathTimer == 0 && Main.myPlayer == player.whoAmI)
                        Main.PlaySound(SoundID.MaxMana, (int)player.position.X, (int)player.position.Y, 1, 0.85f, -6f);
                }
                int type = ModContent.ProjectileType<Friend>();
                if (player.ownedProjectileCounts[type] < 3)
                {
                    for (int i = 0; i < Main.maxProjectiles; i++)
                    {
                        if (Main.projectile[i].active && Main.projectile[i].type == type && Main.projectile[i].owner == player.whoAmI)
                            Main.projectile[i].Kill();
                    }
                    for (int i = 0; i < 3; i++)
                    {
                        Projectile.NewProjectile(player.Center, Vector2.Zero, type, 66, 4f, player.whoAmI, 1f + i);
                    }
                }
            }
            else
            {
                if (omoriDeathTimer <= 0)
                    omoriDeathTimer = 1;
            }
            if (antiGravityItems)
            {
                EquivalenceMachineItemManager.AntiGravityNearbyItems(player.Center, 2f * grabReachMult, 20, player);
            }
            if (spicyEel)
            {
                player.accRunSpeed *= 1.1f;
                player.moveSpeed *= 1.1f;
            }
        }

        public override void ModifyHitByProjectile(Projectile proj, ref int damage, ref bool crit)
        {
            if (proj.trap && setBonusLightbulb)
            {
                damage = (int)(damage * 0.7f);
            }
            if (extractinator && AQProjectile.Sets.DamageReductionExtractor[proj.type])
            {
                damage /= 4;
            }
            var aQProjectile = proj.GetGlobalProjectile<AQProjectile>();
            if (aQProjectile.temperature != 0)
            {
                InflictTemperature(aQProjectile.temperature);
            }
        }

        public override void ModifyHitByNPC(NPC npc, ref int damage, ref bool crit)
        {
            switch (npc.type)
            {
                case NPCID.Mothron:
                case NPCID.MothronSpawn:
                case NPCID.MothronEgg:
                case NPCID.CultistBoss:
                case NPCID.CultistBossClone:
                case NPCID.CultistDragonBody1:
                case NPCID.CultistDragonBody2:
                case NPCID.CultistDragonBody3:
                case NPCID.CultistDragonBody4:
                case NPCID.CultistDragonHead:
                case NPCID.CultistDragonTail:
                case NPCID.AncientCultistSquidhead:
                    {
                        if (mothmanMask)
                            damage /= 2;
                    }
                    break;
            }
            if (lightAmulet && AQNPC.Sets.Holy[npc.type])
            {
                damage = (int)(damage * 0.9f);
            }
            if (darkAmulet && AQNPC.Sets.Unholy[npc.type])
            {
                damage = (int)(damage * 0.9f);
            }
            var npcTemperature = npc.GetGlobalNPC<NPCTemperatureManager>();
            if (npcTemperature.temperature != 0)
            {
                InflictTemperature(npcTemperature.temperature);
            }
        }

        public override void OnHitNPC(Item item, NPC target, int damage, float knockback, bool crit)
        {
            var center = player.Center;
            var targetCenter = target.Center;
            if (item.melee)
            {
                if (hyperCrystal)
                {
                    target.AddBuff(ModContent.BuffType<Sparkling>(), 120);
                    if (crit)
                        DoHyperCrystalChannel(target, damage, knockback, center, targetCenter);
                }
                if (primeTime)
                {
                    if (player.potionDelay <= 0)
                    {
                        player.AddBuff(ModContent.BuffType<Buffs.PrimeTime>(), 600);
                        player.AddBuff(BuffID.PotionSickness, player.potionDelayTime);
                    }
                }
            }
            OnHitNPCEffects(target, targetCenter, damage, knockback, crit);
        }

        public override void OnHitNPCWithProj(Projectile proj, NPC target, int damage, float knockback, bool crit)
        {
            var center = player.Center;
            var targetCenter = target.Center;
            if (proj.melee && proj.whoAmI == player.heldProj && proj.aiStyle != 99)
            {
                if (hyperCrystal)
                {
                    target.AddBuff(ModContent.BuffType<Sparkling>(), 120);
                    if (crit)
                        DoHyperCrystalChannel(target, damage, knockback, center, targetCenter);
                }
                if (primeTime)
                {
                    if (player.potionDelay <= 0)
                    {
                        player.AddBuff(ModContent.BuffType<Buffs.PrimeTime>(), 600);
                        player.AddBuff(BuffID.PotionSickness, player.potionDelayTime);
                    }
                }
            }
            OnHitNPCEffects(target, targetCenter, damage, knockback, crit);
        }

        public override void UpdateBadLifeRegen()
        {
            if (sparkling)
            {
                if (player.lifeRegen > 0)
                    player.lifeRegen = 0;
                player.lifeRegenTime = 0;
                player.lifeRegen -= 40;
            }
            if (notFrostburn)
            {
                if (player.lifeRegen > 0)
                    player.lifeRegen = 0;
                player.lifeRegenTime = 0;
                player.lifeRegen -= 10;
            }
        }

        public override void AnglerQuestReward(float rareMultiplier, List<Item> rewardItems)
        {
            if (Main.myPlayer == player.whoAmI && AQConfigClient.Instance.ShowCompletedQuestsCount)
                CombatText.NewText(player.getRect(), Color.Aqua, player.anglerQuestsFinished);
            var item = new Item();
            if (player.anglerQuestsFinished == 2)
            {
                item.SetDefaults(ModContent.ItemType<CopperSeal>());
                rewardItems.Add(item.Clone());
                item = new Item();
            }
            else if (player.anglerQuestsFinished == 10)
            {
                item.SetDefaults(ModContent.ItemType<SilverSeal>());
                rewardItems.Add(item.Clone());
                item = new Item();
            }
            else if (player.anglerQuestsFinished == 20)
            {
                item.SetDefaults(ModContent.ItemType<GoldSeal>());
                rewardItems.Add(item.Clone());
                item = new Item();
            }
        }

        public override void CatchFish(Item fishingRod, Item bait, int power, int liquidType, int poolSize, int worldLayer, int questFish, ref int caughtType, ref bool junk)
        {
            if (questFish > Main.maxItems && ItemLoader.GetItem(questFish) is AnglerQuestItem anglerQuestItem)
            {
                if (anglerQuestItem.FishingLocation.CatchFish(player, fishingRod, bait, power, liquidType, poolSize, worldLayer))
                {
                    caughtType = questFish;
                    junk = false;
                    return;
                }
            }
            int fish = FishLoader.PoolFish(player, this, fishingRod, bait, power, liquidType, poolSize, worldLayer, questFish, caughtType);
            if (fish != 0)
            {
                caughtType = fish;
                junk = false;
                return;
            }
        }

        public override void ModifyScreenPosition()
        {
            ScreenShakeManager.ModifyScreenPosition();
        }

        public static bool CanBossChannel(NPC npc)
        {
            if (npc.chaseable || npc.dontTakeDamage)
            {
                return false;
            }
            return npc.boss || AQNPC.Sets.BossRelatedEnemy[npc.type];
        }

        public void DoHyperCrystalChannel(NPC target, int damage, float knockback, Vector2 center, Vector2 targCenter)
        {
            if (target.SpawnedFromStatue || target.type == NPCID.TargetDummy || CanBossChannel(target))
                return;
            int boss = -1;
            float closestDist = 1200f;
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];
                if (npc.active && CanBossChannel(npc))
                {
                    float dist = (npc.Center - center).Length();
                    if (dist < closestDist)
                    {
                        boss = i;
                        closestDist = dist;
                    }
                }
            }
            if (boss != -1)
            {
                int dmg = damage > target.lifeMax ? target.lifeMax : damage;
                var normal = Vector2.Normalize(Main.npc[boss].Center - targCenter);
                int size = 4;
                var type = ModContent.DustType<MonoDust>();
                Vector2 position = target.Center - new Vector2(size / 2);
                int length = (int)(Main.npc[boss].Center - targCenter).Length();
                if (Main.myPlayer == player.whoAmI && AQConfigClient.c_TonsofScreenShakes)
                {
                    if (length < 800)
                        ScreenShakeManager.AddShake(new BasicScreenShake(12, AQGraphics.MultIntensity((800 - length) / 128)));
                }
                int dustLength = length / size;
                const float offset = MathHelper.TwoPi / 3f;
                for (int i = 0; i < dustLength; i++)
                {
                    Vector2 pos = position + normal * (i * size);
                    for (int j = 0; j < 6; j++)
                    {
                        int d = Dust.NewDust(pos, size, size, type);
                        float positionLength = Main.dust[d].position.Length() / 32f;
                        Main.dust[d].color = new Color(
                            (float)Math.Sin(positionLength) + 1f,
                            (float)Math.Sin(positionLength + offset) + 1f,
                            (float)Math.Sin(positionLength + offset * 2f) + 1f,
                            0.5f);
                    }
                }
                for (int i = 0; i < 8; i++)
                {
                    Vector2 normal2 = new Vector2(1f, 0f).RotatedBy(MathHelper.PiOver4 * i);
                    for (int j = 0; j < 4; j++)
                    {

                        float positionLength1 = (targCenter + normal2 * (j * 8f)).Length() / 32f;
                        var color = new Color(
                            (float)Math.Sin(positionLength1) + 1f,
                            (float)Math.Sin(positionLength1 + offset) + 1f,
                            (float)Math.Sin(positionLength1 + offset * 2f) + 1f,
                            0.5f);
                        int d = Dust.NewDust(targCenter, 1, 1, type, default, default, default, color);
                        Main.dust[d].velocity = normal2 * (j * 3.5f);
                    }
                }
                Projectile.NewProjectile(Main.npc[boss].Center, Vector2.Zero, ModContent.ProjectileType<HyperCrystalExplosion>(), dmg * 2, knockback * 2, player.whoAmI);
            }
        }

        private void OnHitNPCEffects(NPC target, Vector2 targetCenter, int damage, float knockback, bool crit)
        {
            if (mothmanMask && mothmanExplosionDelay == 0 && player.statLife >= player.statLifeMax2 && crit && target.type != NPCID.TargetDummy)
            {
                target.AddBuff(ModContent.BuffType<BlueFire>(), 480);
                if (Main.myPlayer == player.whoAmI)
                {
                    Main.PlaySound(SoundID.Item74, targetCenter);
                    int amount = (int)(25 * AQConfigClient.c_EffectIntensity);
                    if (AQConfigClient.c_EffectQuality < 1f)
                    {
                        amount = (int)(amount * AQConfigClient.c_EffectQuality);
                    }
                    var pos = target.position - new Vector2(2f, 2f);
                    var rect = new Rectangle((int)pos.X, (int)pos.Y, target.width + 4, target.height + 4);
                    for (int i = 0; i < amount; i++)
                    {
                        var dustPos = new Vector2(Main.rand.Next(rect.X, rect.X + rect.Width), Main.rand.Next(rect.Y, rect.Y + rect.Height));
                        var velocity = new Vector2(Main.rand.NextFloat(-8f, 8f), Main.rand.NextFloat(-10f, 2f).Abs());
                        Particle.PostDrawPlayers.AddParticle(
                            new EmberParticle(dustPos, velocity,
                            new Color(0.5f, Main.rand.NextFloat(0.2f, 0.6f), Main.rand.NextFloat(0.8f, 1f), 0f), Main.rand.NextFloat(0.8f, 1.1f)));
                        Particle.PostDrawPlayers.AddParticle(
                            new EmberParticle(dustPos, velocity,
                            new Color(0.5f, Main.rand.NextFloat(0.2f, 0.6f), Main.rand.NextFloat(0.8f, 1f), 0f) * 0.2f, 1.5f));
                    }
                    amount = (int)(120 * AQConfigClient.c_EffectIntensity);
                    if (AQConfigClient.c_EffectQuality < 1f)
                    {
                        amount = (int)(amount * AQConfigClient.c_EffectQuality);
                    }
                    if (AQConfigClient.c_Screenshakes)
                    {
                        ScreenShakeManager.AddShake(new BasicScreenShake(16, 8));
                    }
                    mothmanExplosionDelay = 60;
                    int p = Projectile.NewProjectile(targetCenter, Vector2.Normalize(targetCenter - player.Center), ModContent.ProjectileType<MothmanCritExplosion>(), damage * 2, knockback * 1.5f, player.whoAmI, 0f, target.whoAmI);
                    var size = Main.projectile[p].Size;
                    float radius = size.Length() / 5f;
                    for (int i = 0; i < amount; i++)
                    {
                        var offset = new Vector2(Main.rand.NextFloat(radius), 0f).RotatedBy(Main.rand.NextFloat(-MathHelper.Pi, MathHelper.Pi));
                        var normal = Vector2.Normalize(offset);
                        var dustPos = targetCenter + offset;
                        var velocity = normal * Main.rand.NextFloat(6f, 12f);
                        Particle.PostDrawPlayers.AddParticle(
                            new EmberParticle(dustPos, velocity,
                            new Color(0.5f, Main.rand.NextFloat(0.2f, 0.6f), Main.rand.NextFloat(0.8f, 1f), 0f), Main.rand.NextFloat(0.8f, 1.1f)));
                        Particle.PostDrawPlayers.AddParticle(
                            new EmberParticle(dustPos, velocity,
                            new Color(0.5f, Main.rand.NextFloat(0.2f, 0.6f), Main.rand.NextFloat(0.8f, 1f), 0f) * 0.2f, 1.5f));
                        if (Main.rand.NextBool(14))
                        {
                            var sparkleClr = new Color(0.5f, Main.rand.NextFloat(0.2f, 0.6f), Main.rand.NextFloat(0.8f, 1f), 0f);
                            Particle.PostDrawPlayers.AddParticle(
                                new SparkleParticle(dustPos, velocity,
                                sparkleClr, 1.5f));
                            Particle.PostDrawPlayers.AddParticle(
                                new SparkleParticle(dustPos, velocity,
                                sparkleClr * 0.5f, 1f)
                                { rotation = MathHelper.PiOver4 });
                        }
                    }
                }
            }
            if (spreadDebuffs)
            {
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    if (Main.npc[i].active && target.Distance(Main.npc[i].Center) < 200f)
                    {
                        SpreadDebuffs(target, Main.npc[i]);
                    }
                }
            }
        }

        private static void SpreadDebuffs(NPC spreader, NPC npc)
        {
            List<Color> poofColors = new List<Color>();
            for (int i = 0; i < spreader.buffType.Length; i++)
            {
                if (spreader.buffType[i] > 0 && !AQBuff.Sets.CantBeSpreadToOtherNPCs[spreader.buffType[i]] && !npc.buffImmune[spreader.buffType[i]])
                {
                    int b = npc.FindBuffIndex(spreader.buffType[i]);
                    if (b == -1)
                    {
                        npc.AddBuff(spreader.buffType[i], spreader.buffTime[i] * (Main.rand.NextBool() ? 1 : 2));
                        var color = BuffColorCache.GetColorFromBuffID(spreader.buffType[i]);
                        if (color != Color.Transparent)
                        {
                            poofColors.Add(color);
                        }
                    }
                    else
                    {
                        if (npc.buffTime[b] < spreader.buffTime[i])
                        {
                            npc.buffTime[b] = spreader.buffTime[i] * (Main.rand.NextBool() ? 1 : 2);
                        }
                    }
                }
            }
            if (poofColors.Count > 0)
            {
                for (int i = 0; i < 16; i++)
                {
                    DustSpawnPatterns.SpawnDustAnMakeVelocityGoAwayFromOrigin(npc.position, npc.width, npc.height, ModContent.DustType<MonoDust>(), 
                        Main.rand.NextFloat(0.1f, 0.25f), poofColors[Main.rand.Next(poofColors.Count)], Main.rand.NextFloat(0.9f, 1.1f));
                }
                for (int i = 0; i < 12; i++)
                {
                    DustSpawnPatterns.SpawnDustAnMakeVelocityGoAwayFromOrigin(npc.position, npc.width, npc.height, ModContent.DustType<MonoDust>(), 
                        Main.rand.NextFloat(npc.width / 16f, npc.width / 12f), poofColors[Main.rand.Next(poofColors.Count)], Main.rand.NextFloat(0.9f, 1.1f));
                }
                for (int i = 0; i < 3; i++)
                {
                    DustSpawnPatterns.SpawnDustAnMakeVelocityGoAwayFromOrigin(npc.position, npc.width, npc.height, ModContent.DustType<MonoSparkleDust>(), 
                        Main.rand.NextFloat(npc.width / 12f, npc.width / 8f), poofColors[Main.rand.Next(poofColors.Count)], Main.rand.NextFloat(0.9f, 1.1f));
                }
            }
        }

        public void InflictTemperature(sbyte newTemperature)
        {
            temperatureRegen = TEMPERATURE_REGEN_ON_HIT;
            if (player.resistCold && newTemperature < 0)
            {
                newTemperature /= 2;
            }
            if (temperature < 0)
            {
                if (newTemperature < 0)
                {
                    if (temperature > newTemperature)
                    {
                        temperature = newTemperature;
                    }
                }
                else
                {
                    temperature /= 2;
                }
            }
            else if (temperature > 0)
            {
                if (newTemperature > 0)
                {
                    if (temperature < newTemperature)
                    {
                        temperature = newTemperature;
                    }
                }
                else
                {
                    temperature /= 2;
                }
            }
            else
            {
                temperature = (sbyte)(newTemperature / 2);
            }
            if (Main.expertMode)
            {
                if (newTemperature < 0)
                {
                    temperature -= 3;
                }
                else
                {
                    temperature += 3;
                }
            }
            else
            {
                if (newTemperature < 0)
                {
                    temperature -= 1;
                }
                else
                {
                    temperature += 1;
                }
            }
        }

        public Vector3 GetCelesteTorusPositionOffset(int i)
        {
            return Vector3.Transform(new Vector3(celesteTorusRadius, 0f, 0f), Matrix.CreateFromYawPitchRoll(celesteTorusX, celesteTorusY, celesteTorusZ + MathHelper.TwoPi / 5 * i));
        }

        public void UpdateCelesteTorus()
        {
            if (blueSpheres)
            {
                float playerPercent = player.statLife / (float)player.statLifeMax2;
                celesteTorusMaxRadius = GetCelesteTorusMaxRadius(playerPercent);
                celesteTorusRadius = MathHelper.Lerp(celesteTorusRadius, celesteTorusMaxRadius, 0.1f);
                celesteTorusDamage = GetCelesteTorusDamage();
                celesteTorusKnockback = GetCelesteTorusKnockback();

                celesteTorusScale = 1f + celesteTorusRadius * 0.006f + celesteTorusDamage * 0.009f + celesteTorusKnockback * 0.0015f;

                var type = ModContent.ProjectileType<CelesteTorusCollider>();
                if (Main.myPlayer == player.whoAmI && player.ownedProjectileCounts[type] <= 0)
                {
                    Projectile.NewProjectile(player.Center, Vector2.Zero, type, celesteTorusDamage, celesteTorusKnockback, player.whoAmI);
                }
                else
                {
                    for (int i = 0; i < Main.maxProjectiles; i++)
                    {
                        if (Main.projectile[i].active && Main.projectile[i].owner == player.whoAmI && Main.projectile[i].type == ModContent.ProjectileType<CelesteTorusCollider>())
                        {
                            Main.projectile[i].damage = celesteTorusDamage;
                            Main.projectile[i].knockBack = celesteTorusKnockback;
                            break;
                        }
                    }
                }
                var center = player.Center;
                bool danger = false;
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    if (Main.npc[i].IsntFriendly() && Vector2.Distance(Main.npc[i].Center, center) < 2000f)
                    {
                        danger = true;
                        break;
                    }
                }

                if (danger)
                {
                    celesteTorusSpeed = 0.04f + (1f - playerPercent) * 0.0314f;
                    celesteTorusX = celesteTorusX.AngleLerp(0f, 0.01f);
                    celesteTorusY = celesteTorusY.AngleLerp(0f, 0.0075f);
                    celesteTorusZ += celesteTorusSpeed;
                }
                else
                {
                    celesteTorusSpeed = 0.0314f;
                    celesteTorusX += 0.0157f;
                    celesteTorusY += 0.01f;
                    celesteTorusZ += celesteTorusSpeed;
                }
            }
            else
            {
                celesteTorusDamage = 0;
                celesteTorusKnockback = 0f;
                celesteTorusMaxRadius = 0;
                celesteTorusRadius = 0f;
                celesteTorusScale = 1f;
                celesteTorusSpeed = 0f;
                celesteTorusX = 0f;
                celesteTorusY = 0f;
                celesteTorusZ = 0f;
            }
        }

        public int GetCelesteTorusMaxRadius(float playerPercent)
        {
            return (int)((float)Math.Sqrt(player.width * player.height) + 20f + player.wingTimeMax * 0.15f + player.wingTime * 0.15f + (1f - playerPercent) * 90f + player.statDefense);
        }

        public int GetCelesteTorusDamage()
        {
            return 25 + (int)(player.statDefense / 1.5f + player.endurance * 80f);
        }

        public float GetCelesteTorusKnockback()
        {
            return 6.5f + player.velocity.Length() * 0.8f;
        }

        public void SetMinionCarryPos(int x, int y)
        {
            hasMinionCarry = true;
            headMinionCarryX = x;
            headMinionCarryY = y;
        }

        public Vector2 GetHeadCarryPosition()
        {
            int x;
            if (headMinionCarryX != 0)
            {
                x = headMinionCarryX;
            }
            else if (headMinionCarryXOld != 0)
            {
                x = headMinionCarryXOld;
            }
            else
            {
                x = (int)player.position.X + player.width / 2;
            }
            int y;
            if (headMinionCarryY != 0)
            {
                y = headMinionCarryY;
            }
            else if (headMinionCarryYOld != 0)
            {
                y = headMinionCarryYOld;
            }
            else
            {
                y = (int)player.position.Y + player.height / 2;
            }
            return new Vector2(x, y);
        }

        /// <summary>
        /// Item is the item, int is the index.
        /// </summary>
        /// <param name="action"></param>
        public void ArmorAction(Action<Item, int> action)
        {
            for (int i = 0; i < 3; i++)
            {
                action(player.armor[i], i);
            }
        }

        /// <summary>
        /// Item is the item, bool is the hide flag, int is the index.
        /// </summary>
        /// <param name="action"></param>
        public void AccessoryAction(Action<Item, bool, int> action)
        {
            for (int i = 3; i < 8 + player.extraAccessorySlots; i++)
            {
                action(player.armor[i], player.hideVisual[i], i);
            }
        }

        public static void HeadMinionSummonCheck(int player, int type)
        {
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                if (Main.projectile[i].active && Main.projectile[i].type != type && AQProjectile.Sets.MinionHeadType[Main.projectile[i].type] && Main.projectile[i].owner == player)
                    Main.projectile[i].Kill();
            }
        }

        public static bool HasFoodBuff(int player)
        {
            for (int i = 0; i < Player.MaxBuffs; i++)
            {
                if (Main.player[player].buffTime[i] > 0 && AQBuff.Sets.IsFoodBuff[Main.player[player].buffType[i]])
                {
                    return true;
                }
            }
            return false;
        }

        public static bool PlayerCrit(int critChance, UnifiedRandom rand)
        {
            if (critChance >= 100)
                return true;
            if (critChance <= 0)
                return false;
            return rand.NextBool(100 - critChance);
        }

        public static bool InVanitySlot(Player player, int type)
        {
            for (int i = MaxDye; i < MaxArmor; i++)
            {
                if (player.armor[i].type == type)
                    return true;
            }
            return false;
        }

        public static bool CanReach(Player player)
            => CanReach(player, Player.tileTargetX, Player.tileTargetY);
        public static bool CanReach(Player player, int x, int y)
            => !player.noBuilding && player.position.X / 16f - Player.tileRangeX - player.blockRange <= x
            && (player.position.X + player.width) / 16f + Player.tileRangeX - 1f + player.blockRange >= x
            && player.position.Y / 16f - Player.tileRangeY - player.blockRange <= y
            && (player.position.Y + player.height) / 16f + Player.tileRangeY + 2f + player.blockRange >= y;
        public static bool CanReach(Player player, Item item)
            => CanReach(player, item, Player.tileTargetX, Player.tileTargetY);
        public static bool CanReach(Player player, Item item, int x, int y)
            => player.position.X / 16f - Player.tileRangeX - item.tileBoost <= x
            && (player.position.X + player.width) / 16f + Player.tileRangeX + item.tileBoost - 1f >= x
            && player.position.Y / 16f - Player.tileRangeY - item.tileBoost <= y
            && (player.position.Y + player.height) / 16f + Player.tileRangeY + item.tileBoost - 2f >= y;

        public static bool ConsumeItem_CheckMouseToo(Player player, int type)
        {
            var item = player.ItemInHand();
            if (item != null && item.type == type)
            {
                if (ItemLoader.ConsumeItem(item, player))
                {
                    item.stack--;
                    if (item.stack <= 0)
                    {
                        item.TurnToAir();
                    }
                    return true;
                }
            }
            return player.ConsumeItem(type);
        }

        public static bool CanForceAutoswing(Player player, Item item, bool ignoreChanneled = false)
        {
            if (AQItem.Sets.CantForceAutoswing[item.type])
            {
                return false;
            }
            if (!item.autoReuse && item.useTime != 0 && item.useTime == item.useAnimation)
            {
                if (!ignoreChanneled && (item.channel || item.noUseGraphic))
                {
                    return player.ownedProjectileCounts[item.shoot] < item.stack;
                }
                return player.altFunctionUse != 2;
            }
            return false;
        }

        public static bool IgnoreMoons()
        {
            for (int i = 0; i < Main.maxPlayers; i++)
            {
                if (Main.player[i].active && Main.player[i].GetModPlayer<AQPlayer>().ignoreMoons) // dead players also allow moons to be disabled
                {
                    return true;
                }
            }
            return false;
        }

        public static bool ShouldDoFadingBecauseOfToolsOrSomething(Player player)
        {
            if (player.HeldItem == null || player.HeldItem.type <= ItemID.None)
            {
                return false;
            }
            var item = player.HeldItem;
            return item.pick > 0 || item.axe > 0 || item.hammer > 0 || item.createTile > TileID.Dirt || item.createWall > 0 || item.tileWand > 0;
        }
    }
}