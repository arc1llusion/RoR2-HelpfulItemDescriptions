namespace HelpfulItemDescriptions
{
    public static class ItemFormulas
    {
        public static readonly string Linear = $"{ItemConstants.NumberOfItems} * {ItemConstants.StackAmountIncrease}";

        public static readonly string Hyperbolic = $"(1 - 1 / (0.15 * {ItemConstants.NumberOfItems} + 1)) * 100";
    }
}
