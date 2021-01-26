using HunterCombatMR.AttackEngine.Services;
using System.Collections.Generic;
using System.Linq;
using Terraria;

namespace HunterCombatMR.AttackEngine.Models
{
    public class AttackSequence
    {
        #region Private Fields

        private const bool _historyDebug = false;

        #endregion Private Fields

        #region Public Constructors

        public AttackSequence(ComboAction startingAction,
            IEnumerable<ComboAction> actions,
            HunterCombatPlayer player,
            Item item)
        {
            ActionHistory = new Dictionary<int, string>();
            Actions = actions;
            CurrentAction = startingAction;
            PlayerPeforming = player;
            ItemUsing = item;
            AddAttackToHistory(CurrentAction);

            if (!Actions.Any(x => x.Name.Equals(startingAction.Name)))
                AddAction(startingAction);
        }

        #endregion Public Constructors

        #region Public Properties

        public IDictionary<int, string> ActionHistory { get; }
        public IEnumerable<ComboAction> Actions { get; private set; }
        public ComboSequenceManager ComboManager { get; }
        public ComboAction CurrentAction { get; private set; }

        public Item ItemUsing { get; }
        public HunterCombatPlayer PlayerPeforming { get; }

        #endregion Public Properties

        #region Public Methods

        public void AddAction(ComboAction action)
        {
            var oldList = new List<ComboAction>(Actions);
            InitializeAttack(action.Attack);

            oldList.Add(action);

            Actions = oldList;
        }

        public void AddAttackToHistory(string attackName)
        {
            ActionHistory.Add(ActionHistory.Count(), attackName);
            if (_historyDebug)
                Main.NewText(attackName, Microsoft.Xna.Framework.Color.White);
        }

        public void AddAttackToHistory(ComboAction attack)
        {
            AddAttackToHistory(attack.Name);
        }

        public void InitializeAttack(Attack attack)
        {
            attack.SetOwners(PlayerPeforming, ItemUsing);
        }

        public void Start()
        {
        }

        public void Update()
        {
            CurrentAction.Attack.Update();
        }

        #endregion Public Methods
    }
}