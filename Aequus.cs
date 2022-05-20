using Aequus.Biomes;
using Aequus.Common;
using Aequus.Common.Networking;
using Aequus.Common.Utilities;
using Aequus.Content.CrossMod;
using Aequus.Content.Necromancy;
using Aequus.Items;
using Aequus.Items.Recipes;
using Aequus.NPCs;
using Aequus.Tiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;

namespace Aequus
{
    public class Aequus : Mod
    {
        internal delegate void LegacyDrawMethod(Texture2D texture, Vector2 position, Rectangle? frame, Color color, float scale, Vector2 origin, float rotation, SpriteEffects effects, float layerDepth);

        public const string TextureNone = "Aequus/Assets/None";
        public const string VanillaTexture = "Terraria/Images/";

        public static Aequus Instance { get; private set; }
        public static UserInterface InventoryInterface { get; private set; }
        public static UserInterface NPCTalkInterface { get; private set; }

        public static bool GameWorldActive => Main.instance.IsActive && !Main.gamePaused && !Main.gameInactive;
        public static bool HQ => ClientConfig.Instance.HighQuality;
        public static bool LogMore => ClientConfig.Instance.InfoDebugLogs;

        internal static Color GreenSlimeColor => ContentSamples.NpcsByNetId[NPCID.GreenSlime].color;
        internal static Color BlueSlimeColor => new Color(0, 80, 255, 100);

        public static Action ResetTileRenderPoints;
        public static Action DrawSpecialTilePoints;

        public override void Load()
        {
            Instance = this;
            if (Main.netMode != NetmodeID.Server)
            {
                InventoryInterface = new UserInterface();
                NPCTalkInterface = new UserInterface();
            }

            foreach (var t in AutoloadHelper.GetTypes(Code))
            {
                IOnModLoad.CheckAutoload(this, t);
            }

            On.Terraria.GameContent.Drawing.TileDrawing.PreDrawTiles += TileDrawing_PreDrawTiles;
            On.Terraria.GameContent.Drawing.TileDrawing.DrawReverseVines += TileDrawing_DrawReverseVines;
        }

        private void TileDrawing_DrawReverseVines(On.Terraria.GameContent.Drawing.TileDrawing.orig_DrawReverseVines orig, Terraria.GameContent.Drawing.TileDrawing self)
        {
            orig(self);
            DrawSpecialTilePoints?.Invoke();
        }

        private void TileDrawing_PreDrawTiles(On.Terraria.GameContent.Drawing.TileDrawing.orig_PreDrawTiles orig, Terraria.GameContent.Drawing.TileDrawing self, bool solidLayer, bool forRenderTargets, bool intoRenderTargets)
        {
            orig(self, solidLayer, forRenderTargets, intoRenderTargets);
            bool flag = intoRenderTargets || Lighting.UpdateEveryFrame;
            if (!solidLayer && flag)
            {
                ResetTileRenderPoints?.Invoke();
            }
        }

        public override void PostSetupContent()
        {
            foreach (var t in AutoloadHelper.GetTypes(Code))
            {
                IPostSetupContent.CheckAutoload(this, t);
            }
        }

        public override void AddRecipeGroups()
        {
            AequusRecipes.AddRecipeGroups();
        }

        public override void AddRecipes()
        {
            if (PolaritiesSupport.Polarities.Enabled)
            {
                MonsterBanners.BannerTypesHack.Add(TileID.Search.GetId("Polarities/BannerTile"));
            }
            AutoloadHelper.AutoloadOfType<IAddRecipes>(Code, this);
        }

        public override void PostAddRecipes()
        {
            AutoloadHelper.AutoloadOfType<IPostAddRecipes>(Code, this);
        }

        public override void Unload()
        {
            Instance = null;
            ResetTileRenderPoints = null;
            DrawSpecialTilePoints = null;
            InventoryInterface = null;
            NPCTalkInterface = null;
        }

        public override object Call(params object[] args)
        {
            switch ((string)args[0])
            {
                case "NecroStats":
                    return ModContent.GetInstance<NecromancyDatabase>().HandleModCall(this, args);

                case "Downed":
                    return ModContent.GetInstance<AequusWorld.DownedCalls>().HandleModCall(this, args);
            }
            return null;
        }

        public override void HandlePacket(BinaryReader reader, int whoAmI)
        {
            PacketType type = PacketSender.ReadPacketType(reader);

            var l = Instance.Logger;
            if (type != PacketType.Unused && type != PacketType.Unused_1)
            {
                l.Debug("Recieving Packet: " + type);
            }
            if (type == PacketType.Unused)
            {
                //int npc = reader.ReadInt32();
                //var globals = PacketSender.GetNetworkerGlobals(Main.npc[npc]);
                //for (int i = 0; i < globals.Length; i++)
                //{
                //    globals[i]?.Receive(npc, reader);
                //}
            }
            else if (type == PacketType.SyncNecromancyOwnerTier)
            {
                int npc = reader.ReadInt32();
                Main.npc[npc].GetGlobalNPC<NecromancyNPC>().zombieOwner = reader.ReadInt32();
                Main.npc[npc].GetGlobalNPC<NecromancyNPC>().zombieDebuffTier = reader.ReadSingle();
            }
            else if (type == PacketType.Unused_1)
            {
                //int projectileOwner = reader.ReadInt32();
                //int projectileIdentity = reader.ReadInt32();
                //int projectile = AequusHelpers.FindProjectileIdentity(projectileOwner, projectileIdentity);
                //l.Debug("Data for: " + projectile + ", " + Lang.GetProjectileName(Main.projectile[projectile].type));
                //var globals = PacketSender.GetNetworkerGlobals(Main.projectile[projectile]);
                //for (int i = 0; i < globals.Length; i++)
                //{
                //    globals[i].Receive(projectile, reader);
                //}
            }
            else if (type == PacketType.SoundQueue)
            {
                PacketReader.ReadSoundQueue(reader);
            }
            else if (type == PacketType.DemonSiegeSacrificeStatus)
            {
                DemonSiegeInvasion.EventSacrifice.ReadPacket(reader);
            }
            else if (type == PacketType.RequestDemonSiege)
            {
                DemonSiegeInvasion.HandleStartRequest(reader);
            }
            else if (type == PacketType.RemoveDemonSiege)
            {
                DemonSiegeInvasion.Sacrifices.Remove(new Point(reader.ReadUInt16(), reader.ReadUInt16()));
            }
            else if (type == PacketType.SyncDebuffs)
            {
                byte npc = reader.ReadByte();
                Main.npc[npc].GetGlobalNPC<NPCDebuffs>().Receive(npc, reader);
            }
        }
    }
}