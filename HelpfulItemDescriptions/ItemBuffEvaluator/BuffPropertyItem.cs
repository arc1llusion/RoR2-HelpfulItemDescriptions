using NCalc;
using System;
using System.Data;

namespace HelpfulItemDescriptions
{
    public enum TextStyle
    {
        Damage,
        Healing,
        Stack,
        Utility,
        None
    }

    public class BuffPropertyItem
    {
        public readonly string Property;

        public readonly double BaseAmount;

        public readonly double StackIncrease;

        public readonly string Formula;

        public readonly string Unit;

        public readonly TextStyle TextStyle;

        public BuffPropertyItem(string property, double baseAmount, double stackIncrease = 0, string formula = null, string unit = "%", TextStyle style = TextStyle.None)
        {
            Property = property;
            BaseAmount = baseAmount;
            StackIncrease = stackIncrease;
            Formula = formula;
            Unit = unit;
            TextStyle = style;

            if (String.IsNullOrWhiteSpace(Formula))
            {
                Formula = ItemFormulas.Linear;
            }
        }

        public BuffEvaluateResult Evaluate(int stackCount)
        {
            double result = 0;

            if (stackCount == 0)
            {
                result = 0;
            }
            else if (stackCount == 1)
            {
                result = BaseAmount;
            }
            else
            {
                string evalString = ItemConstants.ReplaceFields(this, stackCount);
                Expression e = new Expression(evalString);
                
                result = Convert.ToDouble(e.Evaluate());
            }

            return new BuffEvaluateResult() { Property = Property, Result = result, Unit = Unit, TextStyle = TextStyle };
        }
    }
}
