﻿using Aequus;
using Aequus.Common;
using Aequus.Common.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Reflection;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace Aequus
{
    /// <summary>
    /// A helper class which contains many useful methods
    /// </summary>
    public static class AequusHelpers
    {
        public const int NPCREGEN = 8;

        /// <summary>
        /// A static integer used for counting how many iterations for an iterative process has occured. Use this to prevent infinite loops, and always be sure to reset to 0 afterwards.
        /// </summary>
        public static int iterations;

        /// <summary>
        /// Caches <see cref="Main.invasionSize"/>
        /// </summary>
        public static StaticManipulator<int> Main_invasionSize { get; internal set; }
        /// <summary>
        /// Caches <see cref="Main.invasionType"/>
        /// </summary>
        public static StaticManipulator<int> Main_invasionType { get; internal set; }
        /// <summary>
        /// Caches <see cref="Main.bloodMoon"/>
        /// </summary>
        public static StaticManipulator<bool> Main_bloodMoon { get; internal set; }
        /// <summary>
        /// Caches <see cref="Main.eclipse"/>
        /// </summary>
        public static StaticManipulator<bool> Main_eclipse { get; internal set; }
        /// <summary>
        /// Caches <see cref="Main.dayTime"/>
        /// </summary>
        public static StaticManipulator<bool> Main_dayTime { get; internal set; }
        /// <summary>
        /// Determines whether or not the mouse has an item
        /// </summary>
        public static bool HasMouseItem => Main.mouseItem != null && !Main.mouseItem.IsAir;
        public static Matrix WorldViewPoint
        {
            get
            {
                GraphicsDevice graphics = Main.graphics.GraphicsDevice;
                Vector2 screenZoom = Main.GameViewMatrix.Zoom;
                int width = graphics.Viewport.Width;
                int height = graphics.Viewport.Height;

                var zoom = Matrix.CreateLookAt(Vector3.Zero, Vector3.UnitZ, Vector3.Up) *
                    Matrix.CreateTranslation(width / 2f, height / -2f, 0) *
                    Matrix.CreateRotationZ(MathHelper.Pi) * Matrix.CreateScale(screenZoom.X, screenZoom.Y, 1f);
                var projection = Matrix.CreateOrthographic(width, height, 0, 1000);
                return zoom * projection;
            }
        }

        public static void GetDrawInfo(this Projectile projectile, out Texture2D texture, out Vector2 offset, out Rectangle frame, out Vector2 origin, out int trailLength)
        {
            texture = TextureAssets.Projectile[projectile.type].Value;
            offset = projectile.Size / 2f;
            frame = projectile.Frame();
            origin = frame.Size() / 2f;
            trailLength = ProjectileID.Sets.TrailCacheLength[projectile.type];
        }

        public static Vector2 NextCircularFromRect(this UnifiedRandom rand, Rectangle rectangle)
        {
            return rectangle.Center.ToVector2() + rand.NextVector2Unit() * rand.NextFloat(rectangle.Size().Length() / 2f);
        }

        public static Point FluffizePoint(Point point, int fluff = 10)
        {
            point.Fluffize(fluff);
            return point;
        }

        public static void Fluffize(this ref Point point, int fluff = 10)
        {
            if (point.X < fluff)
            {
                point.X = fluff;
            }
            else if (point.X > Main.maxTilesX - fluff)
            {
                point.X = Main.maxTilesX - fluff;
            }
            if (point.Y < fluff)
            {
                point.Y = fluff;
            }
            else if (point.Y > Main.maxTilesY - fluff)
            {
                point.Y = Main.maxTilesY - fluff;
            }
        }

        public static void AddOrAdjust<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key, TValue value)
        {
            if (dict.ContainsKey(key))
            {
                dict[key] = value;
            }
            else
            {
                dict.Add(key, value);
            }
        }

        public static int FindProjectileIdentity(int owner, int identity)
        {
            int projectile = 1000;
            for (int i = 0; i < 1000; i++)
            {
                if (Main.projectile[i].owner == owner && Main.projectile[i].identity == identity && Main.projectile[i].active)
                {
                    projectile = i;
                    break;
                }
            }
            if (projectile == 1000)
            {
                for (int i = 0; i < 1000; i++)
                {
                    if (!Main.projectile[i].active)
                    {
                        projectile = i;
                        break;
                    }
                }
            }
            if (projectile == 1000)
            {
                projectile = Projectile.FindOldestProjectile();
            }
            return projectile;
        }

        public static T GetOrDefault<T>(this Dictionary<int, T> dict, int index, T Default = default(T))
        {
            if (dict.TryGetValue(index, out T value))
            {
                return value;
            }
            return Default;
        }

        public static List<int> AllWhichShareBanner(int type, bool vanillaOnly = false)
        {
            var list = new List<int>();
            int banner = ContentSamples.NpcsByNetId[type].ToBanner();
            if (banner == 0)
            {
                return list;
            }
            foreach (var n in ContentSamples.NpcsByNetId)
            {
                if (vanillaOnly && n.Key > Main.maxNPCTypes)
                {
                    continue;
                }
                if (banner == n.Value.ToBanner())
                {
                    list.Add(n.Key);
                }
            }
            return list;
        }

        public static int ToBanner(this NPC npc)
        {
            return Item.NPCtoBanner(npc.BannerID());
        }

        public static void AddRegen(this NPC npc, int regen)
        {
            if (regen < 0 && npc.lifeRegen > 0)
            {
                npc.lifeRegen = 0;
            }
            npc.lifeRegen += regen * NPCREGEN;
        }

        public static void Max(this (int, int) tuple, int value)
        {
            tuple.Item1 = Math.Max(tuple.Item1, value);
            tuple.Item2 = Math.Max(tuple.Item2, value);
        }

        public static byte TickDown(ref byte value, byte tickAmt = 1)
        {
            if (value > 0)
            {
                if (value - tickAmt < 0)
                {
                    value = 0;
                    return 0;
                }
                value -= tickAmt;
            }
            return value;
        }
        public static ushort TickDown(ref ushort value, ushort tickAmt = 1)
        {
            if (value > 0)
            {
                if (value - tickAmt < 0)
                {
                    value = 0;
                    return 0;
                }
                value -= tickAmt;
            }
            return value;
        }
        public static int TickDown(ref int value, uint tickAmt = 1)
        {
            if (value > 0)
            {
                value -= (int)tickAmt;
                if (value < 0)
                {
                    value = 0;
                }
            }
            return value;
        }

        public static bool IsProbablyACritter(this NPC npc)
        {
            return NPCID.Sets.CountsAsCritter[npc.type] || (npc.lifeMax < 5 && npc.lifeMax != 1);
        }

        public static bool IsTheDestroyer(this NPC npc)
        {
            return npc.type == NPCID.TheDestroyer || npc.type == NPCID.TheDestroyerBody || npc.type == NPCID.TheDestroyerTail;
        }

        public static Rectangle Frame(this Projectile projectile)
        {
            return TextureAssets.Projectile[projectile.type].Value.Frame(1, Main.projFrames[projectile.type], 0, projectile.frame);
        }

        public static float CalcProgress(int length, int i)
        {
            return 1f - 1f / length * i;
        }

        public static int TrailLength(this Projectile projectile)
        {
            return ProjectileID.Sets.TrailCacheLength[projectile.type];
        }

        public static void LoopingFrame(this Projectile projectile, int ticksPerFrame)
        {
            projectile.frameCounter++;
            if (projectile.frameCounter > ticksPerFrame)
            {
                projectile.frame = (projectile.frame + 1) % Main.projFrames[projectile.type];
                projectile.frameCounter = 0;
            }
        }

        public static T GetValue<T>(this PropertyInfo property, object obj)
        {
            return (T)property.GetValue(obj);
        }
        public static T GetValue<T>(this FieldInfo field, object obj)
        {
            return (T)field.GetValue(obj);
        }
        public static T ReflectiveCloneTo<T>(this T obj, T obj2)
        {
            return ReflectiveCloneTo(obj, obj2, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        }
        public static T ReflectiveCloneTo<T>(this T obj, T obj2, BindingFlags flags)
        {
            var t = typeof(T);
            foreach (var f in t.GetFields(flags))
            {
                if (!f.IsInitOnly)
                {
                    f.SetValue(obj2, f.GetValue(obj));
                }
            }
            foreach (var p in t.GetProperties(flags))
            {
                if (p.CanWrite)
                {
                    p.SetValue(obj2, p.GetValue(obj));
                }
            }
            return obj2;
        }

        public static NPC CreateSudo(NPC npc)
        {
            var npc2 = new NPC();
            npc2.SetDefaults(npc.type);
            for (int i = 0; i < npc.ai.Length; i++)
            {
                npc2.ai[i] = npc.ai[i];
            }
            for (int i = 0; i < npc.localAI.Length; i++)
            {
                npc2.localAI[i] = npc.localAI[i];
            }
            npc2.width = npc.width;
            npc2.height = npc.height;
            npc2.scale = npc.scale;
            npc2.frame = npc.frame;
            npc2.direction = npc.direction;
            npc2.spriteDirection = npc.spriteDirection;
            npc2.velocity = npc.velocity;
            npc2.rotation = npc.rotation;
            npc2.gfxOffY = npc.gfxOffY;

            var oldSlot = Main.npc[npc.whoAmI];
            try
            {
                npc2.position = npc.position;
                Main.npc[npc.whoAmI] = npc2;
                npc2.AI();
                Main.npc[npc.whoAmI] = oldSlot;
                npc2.position = npc.position;
            }
            catch
            {
                Main.npc[npc.whoAmI] = oldSlot;
            }
            return npc2;
        }

        public static AequusPlayer Aequus(this Player player)
        {
            return player.GetModPlayer<AequusPlayer>();
        }

        internal static void spawnNPC<T>(Vector2 where) where T : ModNPC
        {
            NPC.NewNPC(null, (int)where.X, (int)where.Y, ModContent.NPCType<T>());
        }

        public static void AddLifeRegen(this Player player, int regen)
        {
            bool badRegen = player.lifeRegen < 0;
            player.lifeRegen += regen;
            if (badRegen && player.lifeRegen > 0)
            {
                player.lifeRegen = 0;
            }
        }

        public static bool IsRectangleCollidingWithCircle(Vector2 circle, float circleRadius, Rectangle rectangle)
        {
            return Vector2.Distance(circle, rectangle.Center.ToVector2() + Vector2.Normalize(circle - rectangle.Center.ToVector2()) * rectangle.Size() / 2f) < circleRadius;
        }

        public static bool DeathrayHitbox(Vector2 center, Rectangle targetHitbox, float rotation, float length, float size, float startLength = 0f)
        {
            return DeathrayHitbox(center, targetHitbox, rotation.ToRotationVector2(), length, size, startLength);
        }
        public static bool DeathrayHitbox(Vector2 center, Rectangle targetHitbox, Vector2 normal, float length, float size, float startLength = 0f)
        {
            return DeathrayHitbox(center + normal * startLength, center + normal * startLength + normal * length, targetHitbox, size);
        }
        public static bool DeathrayHitbox(Vector2 from, Vector2 to, Rectangle targetHitbox, float size)
        {
            float _ = float.NaN;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), from, to, size, ref _);
        }

        public static bool IsThisTileOrIsACraftingStationOfThisTile(int craftingStationTile, int comparisonTile)
        {
            if (craftingStationTile == comparisonTile)
            {
                return true;
            }
            if (comparisonTile > Main.maxTileSets)
            {
                var adjTiles = TileLoader.GetTile(comparisonTile).AdjTiles;
                if (adjTiles != null && adjTiles.ContainsAny(craftingStationTile))
                {
                    return true;
                }
            }
            return false;
        }

        public static bool ContainsAny<T>(this IEnumerable<T> en, int en2)
        {
            foreach (var t in en)
            {
                if (t.Equals(en2))
                {
                    return true;
                }
            }
            return false;
        }
        public static bool ContainsAny<T>(this IEnumerable<T> en, IEnumerable<T> en2)
        {
            foreach (var t in en)
            {
                foreach (var t2 in en2)
                {
                    if (t.Equals(t2))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public static void PlaySound<T>() where T : ModSound
        {
            SoundEngine.PlaySound(From<T>());
        }
        public static void PlaySound<T>(Vector2 location) where T : ModSound
        {
            SoundEngine.PlaySound(From<T>(), location);
        }
        public static LegacySoundStyle From<T>() where T : ModSound
        {
            return SoundLoader.GetLegacySoundSlot(typeof(T).Namespace.Replace('.', '/') + "/" + ModContent.GetInstance<T>().Name);
        }
        public static void PlaySound(SoundType type, string name)
        {
            if (type != SoundType.Sound)
            {
                name = type.ToString() + "/" + name;
            }
            var slot = SoundLoader.GetLegacySoundSlot("Aequus/Sounds/" + name);
            SoundEngine.PlaySound(slot);
        }
        public static void PlaySound(SoundType type, string name, Vector2 position)
        {
            if (Main.dedServ)
            {
                return;
            }
            if (type != SoundType.Sound)
            {
                name = type.ToString() + "/" + name;
            }
            var slot = SoundLoader.GetLegacySoundSlot("Aequus/Sounds/" + name);
            SoundEngine.PlaySound(slot, position);
        }
        public static void PlaySound(SoundType type, string name, Vector2 position, float volume = 1f, float pitch = 0f)
        {
            if (Main.dedServ)
            {
                return;
            }
            if (type != SoundType.Sound)
            {
                name = type.ToString() + "/" + name;
            }
            var slot = SoundLoader.GetLegacySoundSlot("Aequus/Sounds/" + name);
            SoundEngine.PlaySound(slot.SoundId, (int)position.X, (int)position.Y, slot.Style, volume, pitch);
        }
        public static void PlaySound(this LegacySoundStyle value, Vector2 position, float volume, float pitch)
        {
            SoundEngine.PlaySound(value.SoundId, (int)position.X, (int)position.Y, value.Style, volume, pitch);
        }
        public static void PlaySound(this LegacySoundStyle value, Vector2 position, float volume)
        {
            SoundEngine.PlaySound(value.SoundId, (int)position.X, (int)position.Y, value.Style, volume);
        }
        public static void PlaySound(this LegacySoundStyle value, Vector2 position)
        {
            SoundEngine.PlaySound(value, position);
        }
        public static void PlaySound(this LegacySoundStyle value)
        {
            SoundEngine.PlaySound(value);
        }

        public static void DrawTrail(this ModProjectile modProjectile, Action<Vector2, float> draw)
        {
            int trailLength = ProjectileID.Sets.TrailCacheLength[modProjectile.Type];
            var offset = new Vector2(modProjectile.Projectile.width / 2f, modProjectile.Projectile.height / 2f);
            for (int i = 0; i < trailLength; i++)
            {
                draw(modProjectile.Projectile.oldPos[i] + offset, 1f - 1f / trailLength * i);
            }
        }

        public static void SetTrail(this ModProjectile modProjectile, int length = -1)
        {
            if (length > 0)
            {
                ProjectileID.Sets.TrailCacheLength[modProjectile.Type] = length;
            }
            ProjectileID.Sets.TrailingMode[modProjectile.Type] = 2;
        }

        public static void GetItemDrawData(this Item item, out Rectangle frame)
        {
            frame = Main.itemAnimations[item.type] == null ? TextureAssets.Item[item.type].Value.Frame() : Main.itemAnimations[item.type].GetFrame(TextureAssets.Item[item.type].Value);
        }

        public static Vector2 ClosestDistance(this Rectangle rect, Vector2 other)
        {
            var center = rect.Center.ToVector2();
            var n = Vector2.Normalize(other - center);
            float x = Math.Min((other.X - center.X).Abs(), rect.Width / 2f);
            float y = Math.Min((other.Y - center.Y).Abs(), rect.Height / 2f);
            return center + n * new Vector2(x, y);
        }

        public static void Active(this Tile tile, bool value)
        {
            tile.HasTile = value;
        }

        public static void SyncNPC(NPC npc)
        {
            if (Main.netMode != NetmodeID.SinglePlayer)
                NetMessage.SendData(MessageID.SyncNPC, Main.myPlayer, -1, null, npc.whoAmI);
        }

        public static int CheckForPlayers(Rectangle rectangle)
        {
            for (int i = 0; i < Main.maxPlayers; i++)
            {
                if (Main.player[i].active && !Main.player[i].dead && rectangle.Intersects(Main.player[i].getRect()))
                {
                    return i;
                }
            }
            return -1;
        }

        public static bool ShouldDoEffects(Vector2 location)
        {
            return Main.netMode != NetmodeID.Server && (new Vector2(Main.screenPosition.X + Main.screenWidth / 2f, Main.screenPosition.Y + Main.screenHeight / 2f) - location).Length() < 2000f;
        }

        public static int RollVanillaSwordPrefix(Item item, UnifiedRandom rand)
        {
            int choice = rand.Next(40);
            switch (choice)
            {
                default:
                    return PrefixID.Large;
                case 1:
                    return PrefixID.Massive;
                case 2:
                    return PrefixID.Dangerous;
                case 3:
                    return 4;
                case 4:
                    return 5;
                case 5:
                    return 6;
                case 6:
                    return 7;
                case 7:
                    return 8;
                case 8:
                    return 9;
                case 9:
                    return 10;
                case 10:
                    return 11;
                case 11:
                    return 12;
                case 12:
                    return 13;
                case 13:
                    return 14;
                case 14:
                    return 15;
                case 15:
                    return 36;
                case 16:
                    return 37;
                case 17:
                    return 38;
                case 18:
                    return 53;
                case 19:
                    return 54;
                case 20:
                    return 55;
                case 21:
                    return 39;
                case 22:
                    return 40;
                case 23:
                    return 56;
                case 24:
                    return 41;
                case 25:
                    return 57;
                case 26:
                    return 42;
                case 27:
                    return 43;
                case 28:
                    return 44;
                case 29:
                    return 45;
                case 30:
                    return 46;
                case 31:
                    return 47;
                case 32:
                    return 48;
                case 33:
                    return 49;
                case 34:
                    return 50;
                case 35:
                    return 51;
                case 36:
                    return 59;
                case 37:
                    return PrefixID.Demonic;
                case 38:
                    return PrefixID.Zealous;
                case 39:
                    return PrefixID.Legendary;
            }
        }
        public static int RollSwordPrefix(Item item, UnifiedRandom rand)
        {
            int num = RollVanillaSwordPrefix(item, rand);
            PrefixLoader.Roll(item, ref num, 40, rand, PrefixCategory.AnyWeapon, PrefixCategory.Melee);
            return num;
        }

        public static void MeleeScale(Projectile proj)
        {
            float scale = Main.player[proj.owner].GetAdjustedItemScale(Main.player[proj.owner].HeldItem);
            if (scale != 1f)
            {
                proj.scale *= scale;
                proj.width = (int)(proj.width * proj.scale);
                proj.height = (int)(proj.height * proj.scale);
            }
        }

        public static Color MaxRGBA(this Color color, byte amt)
        {
            return color.MaxRGBA(amt, amt);
        }
        public static Color MaxRGBA(this Color color, byte amt, byte a)
        {
            return color.MaxRGBA(amt, amt, amt, a);
        }
        public static Color MaxRGBA(this Color color, byte r, byte g, byte b, byte a)
        {
            color.R = Math.Max(color.R, r);
            color.G = Math.Max(color.G, g);
            color.B = Math.Max(color.B, b);
            color.A = Math.Max(color.A, a);
            return color;
        }

        public static Vector2[] CircularVector(int amt, float angleAddition = 0f)
        {
            return Array.ConvertAll(Circular(amt, angleAddition), (f) => f.ToRotationVector2());
        }
        public static float[] Circular(int amt, float angleAddition = 0f)
        {
            var v = new float[amt];
            float f = MathHelper.TwoPi / amt;
            for (int i = 0; i < amt; i++)
            {
                v[i] = (f * i + angleAddition) % MathHelper.TwoPi;
            }
            return v;
        }

        public static void SetLiquidSpeeds(this NPC npc, float water = 0.5f, float lava = 0.5f, float honey = 0.25f)
        {
            return;
            // TODO: Check if these fields still exist
#pragma warning disable CS0162 // Unreachable code detected
            typeof(NPC).GetField("waterMovementSpeed", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(npc, water);
            typeof(NPC).GetField("lavaMovementSpeed", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(npc, lava);
            typeof(NPC).GetField("honeyMovementSpeed", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(npc, honey);
#pragma warning restore CS0162 // Unreachable code detected
        }

        public static T ModItem<T>(this Item item) where T : ModItem
        {
            return (T)item.ModItem;
        }
        public static T ModProjectile<T>(this Projectile projectile) where T : ModProjectile
        {
            return (T)projectile.ModProjectile;
        }
        public static T ModNPC<T>(this NPC npc) where T : ModNPC
        {
            return (T)npc.ModNPC;
        }

        public static void ScreenFlip(Vector2[] value)
        {
            for (int i = 0; i < value.Length; i++)
            {
                value[i] = ScreenFlip(value[i]);
            }
        }
        public static Vector2 ScreenFlip(Vector2 value)
        {
            return new Vector2(value.X, ScreenFlip(value.Y));
        }
        public static float ScreenFlip(float value)
        {
            return -value + Main.screenHeight;
        }

        public static Color LerpBetween(Color[] colors, float amount)
        {
            if (amount < 0f)
            {
                amount %= colors.Length;
                amount = colors.Length - amount;
            }
            int index = (int)amount;
            return Color.Lerp(colors[index % colors.Length], colors[(index + 1) % colors.Length], amount % 1f);
            //try
            //{
            //}
            //catch
            //{
            //    Main.NewText(index);
            //    Main.NewText(index % colors.Length, Color.Blue);
            //    Main.NewText((index + 1) % colors.Length, Color.Red);
            //    Main.NewText(amount, Color.BlanchedAlmond);
            //    Main.NewText(amount % colors.Length, Color.AliceBlue);
            //    Main.NewText(colors.Length, Main.DiscoColor);
            //}
            //return Color.Black;
        }

        public static int TimedBasedOn(int timer, int ticksPer, int loop)
        {
            timer %= ticksPer * loop;
            return timer / ticksPer;
        }

        public static Vector2 InventoryItemGetCorner(Vector2 position, Rectangle itemFrame, float itemScale)
        {
            return position + itemFrame.Size() / 2f * itemScale;
        }
        public static void DrawUIBack(SpriteBatch spriteBatch, Texture2D texture, Vector2 position, Rectangle itemFrame, float itemScale, Color color, float progress = 1f)
        {
            int frameY = (int)(texture.Height * progress);
            var uiFrame = new Rectangle(0, texture.Height - frameY, texture.Width, frameY);
            position.Y += uiFrame.Y * Main.inventoryScale;
            var center = InventoryItemGetCorner(position, itemFrame, itemScale);
            spriteBatch.Draw(texture, center, uiFrame, color, 0f, texture.Size() / 2f, Main.inventoryScale, SpriteEffects.None, 0f);
        }

        public static Item DefaultItem(int type)
        {
            var item = new Item();
            item.SetDefaults(type);
            return item;
        }

        public static bool Insert(this Chest chest, int itemType, int index)
        {
            return chest.Insert(itemType, 1, index);
        }
        public static bool Insert(this Chest chest, int itemType, int itemStack, int index)
        {
            var item = DefaultItem(itemType);
            item.stack = itemStack;
            return InsertIntoUnresizableArray(chest.item, item, index);
        }
        public static bool InsertIntoUnresizableArray<T>(T[] arr, T value, int index)
        {
            if (index >= arr.Length)
            {
                return false;
            }
            for (int j = arr.Length - 1; j > index; j--)
            {
                arr[j] = arr[j - 1];
            }
            arr[index] = value;
            return true;
        }

        public static bool UpdateProjActive(Projectile projectile, ref bool active)
        {
            if (Main.player[projectile.owner].dead)
                active = false;
            if (active)
                projectile.timeLeft = 2;
            return active;
        }

        public static void SetResearch(this ModItem modItem, int amt)
        {
            SetResearch(modItem.Type, amt);
        }
        public static void SetResearch(int type, int amt)
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[type] = amt;
        }

        public static int FindTargetWithLineOfSight(Vector2 position, int width = 2, int height = 2, float maxRange = 800f, object me = null, Func<int, bool> validCheck = null)
        {
            float num = maxRange;
            int result = -1;
            for (int i = 0; i < 200; i++)
            {
                NPC nPC = Main.npc[i];
                if (nPC.CanBeChasedBy(me) && (validCheck == null || validCheck.Invoke(i)))
                {
                    float num2 = Vector2.Distance(position, Main.npc[i].Center);
                    if (num2 < num && Collision.CanHit(position, width, height, nPC.position, nPC.width, nPC.height))
                    {
                        num = num2;
                        result = i;
                    }
                }
            }
            return result;
        }

        public static int RollHigherFromLuck(this Player player, int amt)
        {
            return amt - player.RollLuck(amt);
        }

        public static Color UseR(this Color color, int R) => new Color(R, color.G, color.B, color.A);
        public static Color UseR(this Color color, float R) => new Color((int)(R * 255), color.G, color.B, color.A);

        public static Color UseG(this Color color, int G) => new Color(color.R, G, color.B, color.A);
        public static Color UseG(this Color color, float G) => new Color(color.R, (int)(G * 255), color.B, color.A);

        public static Color UseB(this Color color, int B) => new Color(color.R, color.G, B, color.A);
        public static Color UseB(this Color color, float B) => new Color(color.R, color.G, (int)(B * 255), color.A);

        public static Color UseA(this Color color, int alpha) => new Color(color.R, color.G, color.B, alpha);
        public static Color UseA(this Color color, float alpha) => new Color(color.R, color.G, color.B, (int)(alpha * 255));

        public static float FromByte(byte value, float maximum)
        {
            return value * maximum / 255f;
        }
        public static float FromByte(byte value, float minimum, float maximum)
        {
            return minimum + value * (maximum - minimum) / 255f;
        }

        public static bool CloseEnough(this float comparison, float intendedValue, float closeEnoughMargin = 1f)
        {
            return (comparison - intendedValue).Abs() <= closeEnoughMargin;
        }

        public static float Wave(float time, float minimum, float maximum)
        {
            return minimum + ((float)Math.Sin(time) + 1f) / 2f * (maximum - minimum);
        }

        public static bool SolidTop(this Tile tile)
        {
            return Main.tileSolidTop[tile.TileType];
        }

        public static bool IsSolid(this Tile tile)
        {
            return tile.HasTile && Main.tileSolid[tile.TileType];
        }

        public static bool Solid(this Tile tile)
        {
            return Main.tileSolid[tile.TileType];
        }

        public static int Abs(this int value)
        {
            return value < 0 ? -value : value;
        }
        public static float Abs(this float value)
        {
            return value < 0f ? -value : value;
        }

        public static string GetPath(this object obj)
        {
            return GetPath(obj.GetType());
        }
        public static string GetPath<T>()
        {
            return GetPath(typeof(T));
        }
        public static string GetPath(Type t)
        {
            return t.Namespace.Replace('.', '/') + "/" + t.Name;
        }

        public class Loader : IOnModLoad
        {
            void ILoadable.Load(Mod mod)
            {
            }

            void IOnModLoad.OnModLoad(Aequus aequus)
            {
                Main_invasionSize = new StaticManipulator<int>(() => ref Main.invasionSize);
                Main_invasionType = new StaticManipulator<int>(() => ref Main.invasionType);
                Main_bloodMoon = new StaticManipulator<bool>(() => ref Main.bloodMoon);
                Main_eclipse = new StaticManipulator<bool>(() => ref Main.eclipse);
                Main_dayTime = new StaticManipulator<bool>(() => ref Main.dayTime);
            }

            void ILoadable.Unload()
            {
                Main_invasionSize = null;
                Main_invasionType = null;
                Main_bloodMoon = null;
                Main_eclipse = null;
                Main_dayTime = null;
            }
        }
    }
}