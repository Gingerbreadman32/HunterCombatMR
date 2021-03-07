using HunterCombatMR.AttackEngine.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;

namespace HunterCombatMR.Items
{
    public abstract class SwordAndShieldBase
        : ModItem
    {
        protected virtual void InitilizeAttacks(HunterCombatPlayer player)
        {
            // Restructure this to be less confusing. Figure out a way for it to be a little less concrete so its easy to manage
            var RunningSlash = new ComboAction(HunterCombatMR.Instance.GetLoadedAttack("RunningSlash"));
            var DoubleSlash = new ComboAction(HunterCombatMR.Instance.GetLoadedAttack("DoubleSlash"));
        }
    }
}
