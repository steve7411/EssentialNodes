using System.Collections;
using System.Linq;
using Unity.VisualScripting;

namespace EssentialNodes
{
    [UnitCategory("EssentialNodes/Collections")]
    [TypeIcon(typeof(Unity.VisualScripting.AotList))]
    public class First : Unit
    {
        [DoNotSerialize, PortLabelHidden]
        public ValueInput inputList;

        [DoNotSerialize]
        public ValueInput condition;

        [DoNotSerialize, PortLabelHidden]
        public ValueOutput result;

        [DoNotSerialize]
        public ValueOutput item;

        object _current;

        protected override void Definition()
        {
            inputList = ValueInput<IEnumerable>(nameof(inputList));
            condition = ValueInput<bool>(nameof(condition));
            result = ValueOutput<object>(nameof(result), GetResult);
            item = ValueOutput<object>(nameof(item), GetItem);

            Requirement(inputList, result);
            Requirement(condition, result);
        }

        object GetResult(Flow flow)
        {
            return flow.GetValue<IEnumerable>(inputList).Cast<object>()
                .First(item =>
                {
                    _current = item;
                    return flow.GetValue<bool>(condition);
                });
        }

        object GetItem(Flow flow) => _current;
    }
}