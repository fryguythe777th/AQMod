﻿using Aequus.Common;
using Aequus.Common.ItemDrops;
using Aequus.Content.Necromancy;
using Aequus.Items.Accessories.Summon.Necro;
using Aequus.Items.Consumables;
using Aequus.Items.Consumables.Foods;
using Aequus.Items.Weapons.Ranged;
using Aequus.NPCs.AIs;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.NPCs.Monsters
{
    public class BloodMimic : LegacyAIMimic
    {
        private const int SPAWNRECTANGLE_SIZE = 20;

        public bool spawnedGroup;

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 6;

            NecromancyDatabase.NPCs.Add(Type, GhostInfo.One.WithAggro(AggroForcer.BloodMoon));
        }

        public override void SetDefaults()
        {
            NPC.width = 24;
            NPC.height = 24;
            NPC.aiStyle = -1;
            NPC.damage = 25;
            NPC.defense = 10;
            NPC.lifeMax = 125;
            NPC.HitSound = SoundID.NPCHit16;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.value = Item.buyPrice(silver: 50);
            NPC.knockBackResist = 0.4f;
            NPC.rarity = 1;
            Banner = Item.NPCtoBanner(NPCID.Mimic);
            BannerItem = ItemID.MimicBanner;

            spawnedGroup = false;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            this.CreateEntry(database, bestiaryEntry)
                .AddMainSpawn(BestiaryBuilder.BloodMoon);
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            this.CreateLoot(npcLoot)
                .SetCondition(new LastAliveCondition(Type))

                .AddOptions(chance: 1, 
                ModContent.ItemType<CrusadersCrossbow>(), 
                ModContent.ItemType<BloodiedBucket>())

                .AddOptions(chance: 2, 
                ItemID.MoneyTrough, 
                ItemID.SharkToothNecklace)

                .Add(ItemID.AdhesiveBandage, chance: 8, stack: 1)
                .RegisterCondition()

                .Add<SuspiciousLookingSteak>(chance: 5, stack: 1)
                .Add<PotionOfResurrection>(chance: 5, stack: 1);
        }

        //public override void NPCLoot()
        //{
        //    Rectangle rect = NPC.getRect();
        //    if (NPC.CountNPCS(NPC.type) <= 1)
        //    {
        //        var choices = new List<int> { ModContent.ItemType<TargeoftheBlodded>(), ModContent.ItemType<CrusadersCrossbow>(), };
        //        if (NPC.downedBoss3)
        //            choices.Add(ModContent.ItemType<ATM>());
        //        if (NPC.downedPlantBoss)
        //            choices.Add(ModContent.ItemType<VampireHook>());
        //        int choice = Main.rand.Next(choices.Count);
        //        Item.NewItem(rect, choices[choice]);
        //    }
        //}

        //private void randDrops()
        //{
        //    if (Main.rand.NextBool(5))
        //        Item.NewItem(NPC.getRect(), ModContent.ItemType<BloodshedPotion>());
        //}

        protected override int GetJumpTimer() => NPC.ai[1] == 0f ? 5 : 10;

        public override void HitEffect(int hitDirection, double damage)
        {
            if (Main.netMode == NetmodeID.Server)
            {
                return;
            }

            if (NPC.life <= 0)
            {
                for (int i = 0; i < 50; i++)
                {
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Blood, hitDirection * 2);
                }
                NPC.DeathGore("BloodMimic_0");
                NPC.DeathGore("BloodMimic_1");
                NPC.DeathGore("BloodMimic_1").rotation += MathHelper.Pi;
            }
            for (int i = 0; i < 3; i++)
            {
                Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Blood, hitDirection * 2);
            }
        }

        protected override void Jump()
        {
            if (!Main.bloodMoon)
            {
                NPC.direction = -NPC.direction;
                NPC.spriteDirection = -NPC.spriteDirection;
            }
            if (NPC.ai[1] == 2f)
            {
                NPC.velocity.X = NPC.direction * 5f;
                NPC.velocity.Y = -4f;
                NPC.ai[1] = 0f;
            }
            else
            {
                NPC.velocity.X = NPC.direction * 7f;
                NPC.velocity.Y = -2f;
            }
        }

        public override void AI()
        {
            if (!spawnedGroup)
            {
                var tileLocation = NPC.Center.ToTileCoordinates();
                SpawnGroup(tileLocation.X, tileLocation.Y);
                spawnedGroup = true;
            }
            if (NPC.life < NPC.lifeMax)
            {
                NPC.alpha = 0;
            }
            if (NPC.alpha > 0)
            {
                NPC.alpha -= 20;
                if (NPC.alpha < 0)
                {
                    NPC.alpha = 0;
                }
            }
            if (NPC.ai[0] < 0f)
            {
                NPC.ai[0]++;
                NPC.velocity.X = 0f;
            }
            base.AI();
        }

        public void SpawnGroup(int tileX, int tileY)
        {
            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                return;
            }
            int spawnCount = Main.rand.Next(3, 8);
            var player = Main.player[Player.FindClosest(new Vector2(tileX, tileY) * 16f, 16, 16)];
            Rectangle playerSights = new Rectangle((int)player.position.X / 16 - NPC.safeRangeX, (int)player.position.Y / 16 - NPC.safeRangeY, NPC.safeRangeX * 2, NPC.safeRangeY * 2);
            var source = new EntitySource_SpawnNPC("Aequus:BloodMimic");

            if ((int)NPC.ai[1] == 0)
            {
                NPC.ai[1] = Main.rand.Next(3, 8);
            }

            if ((int)NPC.ai[1] > 1)
            {
                var checkTangle = new Rectangle(tileX - SPAWNRECTANGLE_SIZE, tileY - SPAWNRECTANGLE_SIZE, SPAWNRECTANGLE_SIZE * 2, SPAWNRECTANGLE_SIZE * 2);
                for (int i = 0; i < 50; i++)
                {
                    int x = Main.rand.Next(checkTangle.X, checkTangle.X + checkTangle.Width);
                    int y = Main.rand.Next(checkTangle.Y, checkTangle.Y + checkTangle.Height);

                    for (int k = 0; !Main.tile[x, y].IsSolid() && k < 20; k++)
                    {
                        y++;
                    }

                    y -= 2;

                    if (i != 49)
                    {
                        if (new Rectangle(x, y, 2, 2).Contains(playerSights))
                            continue;
                    }

                    for (int k = 0; k < 2; k++)
                    {
                        for (int l = 0; l < 2; l++)
                        {
                            if (Main.tile[x + k, y + l].IsSolid())
                            {
                                goto Continue;
                            }
                        }
                    }

                    for (int k = 0; k < 2; k++)
                    {
                        if (!Main.tile[x + k, y + 2].IsSolid() || Main.tile[x + k, y + 2].IsHalfBlock || Main.tile[x + k, y + 2].Slope != 0)
                        {
                            goto Continue;
                        }
                    }

                    var npcChecktangle = new Rectangle(x * 16, y * 16, 32, 32);
                    for (int m = 0; m < Main.maxNPCs; m++)
                    {
                        if (Main.npc[m].active && Main.npc[m].type == Type && npcChecktangle.Contains(Main.npc[m].Hitbox))
                        {
                            goto Continue;
                        }
                    }

                    var n = NPC.NewNPCDirect(source, new Vector2(x * 16f, y * 16f), Type, NPC.whoAmI, -20f, NPC.ai[1] - 1f);
                    n.alpha = 255;
                    n.Bottom = new Vector2(x * 16f + 16f, y * 16f + 32f - 2f);
                    break;

                Continue:
                    continue;
                }
            }


            NPC.ai[0] = -20f;
            NPC.Center = new Vector2(tileX * 16f + 8f, tileY * 16f - 8f);
            NPC.alpha = 255;
            NPC.netUpdate = true;
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            if (target.thorns <= 0f)
            {
                target.immuneTime /= 2;
                if (target.starCloakCooldown < 60)
                {
                    target.starCloakCooldown = 60;
                }
                NPC.velocity *= -0.98f;
                NPC.position += NPC.velocity;
                NPC.netUpdate = true;
            }
            target.AddBuff(BuffID.Bleeding, 60);
        }
    }
}