﻿using System;
using System.Data;

namespace HelpfulItemDescriptions
{
    public class BuffPropertyItem
    {
        public readonly string Property;

        public readonly int BaseAmount;

        public readonly int StackIncrease;

        public readonly string Formula;

        public readonly string Unit;

        public BuffPropertyItem(string property, int baseAmount, int stackIncrease = 0, string formula = null, string unit = "%")
        {
            Property = property;
            BaseAmount = baseAmount;
            StackIncrease = stackIncrease;
            Formula = formula;
            Unit = unit;

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
                result = Convert.ToDouble(new DataTable().Compute(evalString, null));
            }

            return new BuffEvaluateResult() { Property = Property, Result = result, Unit = Unit };
        }
    }
}