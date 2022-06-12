﻿using Terraria;
using Terraria.ModLoader;

namespace Aequus.Buffs.Debuffs
{
    public class SoulStolen : ModBuff
    {
        public override string Texture => Aequus.VanillaTexture + "Buff";

        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.buffNoSave[Type] = true;
        }
    }
}