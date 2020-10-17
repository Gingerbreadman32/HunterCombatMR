using HunterCombatMR.Enumerations;
using HunterCombatMR.AttackEngine.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HunterCombatMR.AttackEngine.Services
{
    public sealed class ComboSequenceManager
    {
        public void CheckAttackComboStatus(Attack currentAttack,
            IEnumerable<Attack> possibleAttacks,
            bool immediateSwitch = true)
        {
            var bufferedAttacks = new List<Attack>();
        }
    }
}
