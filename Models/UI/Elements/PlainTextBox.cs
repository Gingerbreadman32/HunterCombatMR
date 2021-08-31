using HunterCombatMR.Enumerations;
using HunterCombatMR.Extensions;
using System.Collections.Generic;
using System.Linq;

namespace HunterCombatMR.Models.UI.Elements
{
    public class PlainTextBox
        : TextBoxBase
    {
        private char[] _availableCharacters;

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

        /// <summary>
        /// Limiting the types of characters this textbox can use
        /// </summary>
        public InputPermissions CharacterPermissions { get; protected set; }

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

        protected override void DrawDetails(string newString)
        {
            if (newString != _currentString)
            {
                UpdateString(string.Concat(newString.Where(x => _availableCharacters.Contains(x))));
            }
        }
    }
}