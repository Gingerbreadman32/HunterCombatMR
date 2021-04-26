using System.Linq;

namespace HunterCombatMR.UI.Elements
{
    public class NumberInputBox
        : TextBoxBase
    {
        private static char[] _mathNumbers = "1234567890-".ToCharArray();

        private char[] _availableCharacters;

        public NumberInputBox(string hintText,
            int maxLength = 0,
            bool isLarge = false,
            bool startHidden = false,
            string defaultText = null,
            int decimalPlaces = 0)
            : base(hintText, maxLength, isLarge, startHidden, defaultText)
        {
            _availableCharacters = _mathNumbers;
            if (decimalPlaces > 0)
                _availableCharacters[_availableCharacters.Length] = '.';
        }

        public bool PositiveIntegersOnly { get; set; }

        public bool ZeroNotAllowed { get; set; }

        protected override void DrawDetails(string newString)
        {
            float textValue = ZeroNotAllowed ? 1 : 0;

            if (newString == _currentString)
                return;

            if (newString.Length == 0)
            {
                newString = textValue.ToString();
                UpdateString(string.Concat(newString.Where(x => _availableCharacters.Contains(x))));
                return;
            }

            if (newString.Contains("+"))
            {
                newString = newString.Replace("+", "");
                if (float.TryParse(newString, out textValue))
                {
                    textValue++;
                    newString = textValue.ToString();
                }
            }

            if (newString.Length + 1 > MaxLength && !newString.Contains("-"))
            {
                newString = newString.Substring(0, MaxLength - 1);
            }

            int wrongNegative = newString.LastIndexOf("-");

            if (wrongNegative > 0)
            {
                newString = newString.Remove(wrongNegative);
                if (!PositiveIntegersOnly && float.TryParse(newString, out textValue))
                {
                    textValue *= -1;
                    newString = textValue.ToString();
                    UpdateString(string.Concat(newString.Where(x => _availableCharacters.Contains(x))));
                    return;
                }
            }

            if (float.TryParse(newString, out textValue))
            {
                newString = textValue.ToString();
            }
            else
            {
                newString = ZeroNotAllowed ? "1" : "0";
            }

            UpdateString(string.Concat(newString.Where(x => _availableCharacters.Contains(x))));
        }
    }
}