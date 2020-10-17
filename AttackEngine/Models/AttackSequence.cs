using HunterCombatMR.AttackEngine.Services;
using System.Collections.Generic;
using System.Linq;
using Terraria;

namespace HunterCombatMR.AttackEngine.Models
{
    public class AttackSequence
    {
        private const bool _historyDebug = false;

        public int currentAttackIndex { get; set; }

        public ComboSequenceManager ComboManager { get; set; }

        public IDictionary<int, string> AttackHistory { get; set; }

        public ICollection<Attack> PossibleAttacks { get; set; }

        public Attack CurrentAttack { get; set; }

        public Player PlayerPeforming { get; set; }

        public Item ItemUsing { get; set; }

        public AttackSequence(string startingAttackName,
            IEnumerable<string> possibleAttacksInSequence,
            Player player,
            Item item)
        {
            currentAttackIndex = 0;
            AttackHistory = new Dictionary<int, string>();
            PlayerPeforming = player;
            ItemUsing = item;
            SetPossibleAttacksFromNames(possibleAttacksInSequence);
            SetCurrentAttackFromName(startingAttackName);
            AddAttackToHistory(CurrentAttack);
        }

        public void Update()
        {
            CurrentAttack.Update();
        }

        public void InitializeAttack(Attack attack)
        {
            HunterCombatMR.AnimationKeyFrameManager.FillAnimationKeyFrames(attack.Animation, attack.FrameProfile);
            PlayerPeforming.GetModPlayer<HunterCombatPlayer>().AttackState = Enumerations.PlayerAttackState.AttackStart;

            if (!attack.Animation.IsPlaying)
                attack.Animation.StartAnimation();

            attack.SetOwners(PlayerPeforming, ItemUsing);
        }

        public void SetPossibleAttacksFromNames(IEnumerable<string> attackNames)
        {
            PossibleAttacks = new List<Attack>();
            foreach (string name in attackNames)
            {
                AddPossibleAttackFromName(name);
            }
        }

        public void AddPossibleAttackFromName(string attackName)
        {
            var attack = HunterCombatMR.LoadedAttacks.First(x => x.Name.Equals(attackName));

            PossibleAttacks.Add(attack);
        }

        public void SetCurrentAttackFromName(string attackName)
        {
            CurrentAttack = PossibleAttacks.First(x => x.Name.Equals(attackName));
            if (!CurrentAttack.Animation.IsInitialized)
                InitializeAttack(CurrentAttack);
        }

        public void AddAttackToHistory(string attackName)
        {
            AttackHistory.Add(currentAttackIndex, attackName);
            if (_historyDebug)
                Main.NewText(attackName, Microsoft.Xna.Framework.Color.White);
            currentAttackIndex++;
        }

        public void AddAttackToHistory(Attack attack)
        {
            AddAttackToHistory(attack.Name);
        }
    }
}