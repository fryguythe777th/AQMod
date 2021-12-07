﻿using AQMod.Assets.LegacyItemOverlays;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Foods
{
    public class NeutronJuice : ModItem, ISpecialFood
    {
        public override void SetStaticDefaults()
        {
            if (!Main.dedServ)
                AQMod.ItemOverlays.Register(new GlowmaskOverlay(this.GetPath("_Glow")), item.type);
        }

        public override void SetDefaults()
        {
            item.width = 10;
            item.height = 10;
            item.useStyle = ItemUseStyleID.EatingUsing;
            item.useAnimation = 15;
            item.useTime = 15;
            item.useTurn = true;
            item.UseSound = SoundID.Item3;
            item.maxStack = 999;
            item.consumable = true;
            item.rare = ItemRarityID.Blue;
            item.value = Item.buyPrice(silver: 20);
            item.buffType = BuffID.WellFed;
            item.buffTime = 36000;
        }

        int ISpecialFood.ChangeBuff(Player player)
        {
            return ModContent.BuffType<Buffs.Foods.NeutronYogurt>();
        }
    }
}