namespace HunterCombatMR.Utilities
{
    public static class OperatorUtils
    {
        public static bool IsValidComparisonOperator(string @operator)
        {
            switch (@operator)
            {
                case "=":
                case ">":
                case ">=":
                case "<=":
                case "<":
                case "!=":
                case "is":
                case "not":
                    return true;

                default:
                    return false;
            }
        }

        public static bool CheckWhiteSpace(string input,
            int keyIndex)
            => input.Length >= keyIndex + 2 && input[keyIndex - 1] == ' ' && input[keyIndex + 1] == ' ';
        
    }
}