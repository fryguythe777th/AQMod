﻿using Aequus.Common;
using Terraria;
using Terraria.ModLoader;

namespace Aequus.Items
{
    public class Hooks : ILoadable
    {
        void ILoadable.Load(Mod mod)
        {
            On.Terraria.Player.UpdateItemDye += IUpdateItemDye.Hook_UpdateItemDye;
        }


        void ILoadable.Unload()
        {
        }

        public interface IModifyFishingPower
        {
            /// <summary>
            /// 
            /// </summary>
            /// <param name="player"></param>
            /// <param name="fishing"></param>
            /// <param name="fishingRod"></param>
            /// <param name="fishingLevel"></param>
            void ModifyFishingPower(Player player, PlayerFishing fishing, Item fishingRod, ref float fishingLevel);
        }

        public interface IUpdateBank
        {
            void UpdateBank(Player player, AequusPlayer aequus, int slot, int bank);
        }

        public interface IUpdateItemDye
        {
            void UpdateItemDye(Player player, bool isNotInVanitySlot, bool isSetToHidden, Item armorItem, Item dyeItem);

            /// <summary>
            /// 
            /// </summary>
            /// <param name="orig"></param>
            /// <param name="self"></param>
            /// <param name="isNotInVanitySlot"></param>
            /// <param name="isSetToHidden"></param>
            /// <param name="armorItem">If you are an equipped item, this is you.</param>
            /// <param name="dyeItem">If you are a dye, this is you.</param>
            internal static void Hook_UpdateItemDye(On.Terraria.Player.orig_UpdateItemDye orig, Player self, bool isNotInVanitySlot, bool isSetToHidden, Item armorItem, Item dyeItem)
            {
                orig(self, isNotInVanitySlot, isSetToHidden, armorItem, dyeItem);
                if (armorItem.ModItem is IUpdateItemDye armorDye)
                {
                    armorDye.UpdateItemDye(self, isNotInVanitySlot, isSetToHidden, armorItem, dyeItem);
                }
                if (dyeItem.ModItem is IUpdateItemDye dye)
                {
                    dye.UpdateItemDye(self, isNotInVanitySlot, isSetToHidden, armorItem, dyeItem);
                }
            }
        }
    }
}