﻿using Terraria;
using Terraria.ModLoader;

namespace AQMod.Common.NoHitting
{
    public class NoHitProj : GlobalProjectile
    {
        public override void OnHitPlayer(Projectile projectile, Player target, int damage, bool crit)
        {
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                if (Main.npc[i].active)
                    Main.npc[i].GetGlobalNPC<NoHitManager>().hitPlayer[target.whoAmI] = true;
            }
        }
    }
}