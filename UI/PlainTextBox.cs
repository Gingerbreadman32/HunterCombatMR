using HunterCombatMR.Enumerations;
using HunterCombatMR.Extensions;
using System.Collections.Generic;
using System.Linq;

namespace HunterCombatMR.UI
{
    public class PlainTextBox
        : TextBoxBase
    {
        #region Private Fields

        private char[] _availableCharacters;

        #endregion Private Fields

        #region Public Constructors

        public PlainTextBox(string hintText,
                    int maxLength = 0,
            bool isLarge = false,
            bool startHidden = false,
            string defaultText = null,
            InputPermissions characterPerms = InputPermissions.Any)
            : base(hintText, maxLength, isLarge, startHidden, defaultText)
        {
            SetInputPermissions(characterPerms);
        }

        #endregion Public Constructors

        #region Public Properties

        /// <summary>
        /// Limiting the types of characters this textbox can use
        /// </summary>
        public InputPermissions CharacterPermissions { get; protected set; }

        #endregion Public Properties

        #region Public Methods

        public void SetInputPermissions(InputPermissions inputPermissions)
        {
            CharacterPermissions = inputPermissions;
            var characterList = new List<char>();
            foreach (InputPermissions flag in inputPermissions.GetFlags())
            {
                characterList.AddRange(flag.GetAssociatedCharacters());
            }
            _availableCharacters = characterList.ToArray();
        }

        #endregion Public Methods

        #region Protected Methods

        protected override void DrawDetails(string newString)
        {
            if (newString != _currentString)
            {
                UpdateString(string.Concat(newString.Where(x => _availableCharacters.Contains(x))));
            }
        }

        #endregion Protected Methods
    }
}