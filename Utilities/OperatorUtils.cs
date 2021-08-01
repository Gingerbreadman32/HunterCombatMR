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
    }
}