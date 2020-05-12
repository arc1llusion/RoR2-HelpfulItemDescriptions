using RoR2;
using System.Collections.Generic;
using System.Text;

namespace HelpfulItemDescriptions
{
    public class ItemBuffEvaluator
    {
        public readonly ItemIndex ItemIndex;

        private List<BuffPropertyItem> _properties;

        public ItemBuffEvaluator(ItemIndex ii, List<BuffPropertyItem> buffProperties)
        {
            ItemIndex = ii;
            _properties = buffProperties;
        }

        public BuffEvaluateResult[] Evaluate(int stackCount)
        {
            BuffEvaluateResult[] result = new BuffEvaluateResult[_properties.Count];

            for(var i = 0; i < result.Length; ++i)
            {
                result[i] = _properties[i].Evaluate(stackCount);
            }

            return result;
        }
    }
}
