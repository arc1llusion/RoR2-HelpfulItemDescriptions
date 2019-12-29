using System;

namespace HelpfulItemDescriptions
{
    public static class ItemConstants
    {
        /// <summary>
        /// Number of Items in the stack
        /// </summary>
        public const string NumberOfItems = "{NumberOfItems}";

        /// <summary>
        /// Amount that the stack increases by
        /// </summary>
        public const string StackAmountIncrease = "{StackAmountIncrease}";

        public static string ReplaceFields(BuffPropertyItem bpi, int stackCount)
        {
            if (bpi.Formula == null)
            {
                return String.Empty;
            }

            return bpi.Formula.Replace(NumberOfItems, stackCount.ToString())
                              .Replace(StackAmountIncrease, bpi.StackIncrease.ToString());
        }
    }
}
