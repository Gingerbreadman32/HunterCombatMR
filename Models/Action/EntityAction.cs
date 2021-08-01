using HunterCombatMR.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HunterCombatMR.Models.Action
{
    public class EntityAction
        : Content
    {
        public EntityAction(string name)
            : base(name)
        {
        }

        public EntityAction(string name,
           string animationName)
            : base(name)
        {

        }

        public override IContent CreateNew(string internalName)
        {
            return new EntityAction(internalName);
        }
    }
}
