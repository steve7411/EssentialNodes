using UnityEngine;
using Unity.VisualScripting;

namespace EssentialNodes
{
    [UnitCategory("EssentialNodes/Variables")]
    [TypeIcon(typeof(Unity.VisualScripting.Add<>))]
    public class ModifyVariable : Unit
    {
        [UnitHeaderInspectable]
        public VariableKind kind;

        [DoNotSerialize, PortLabelHidden]
        public ControlInput enter;

        [DoNotSerialize, PortLabelHidden]
        public ControlOutput exit;

        [DoNotSerialize, PortLabelHidden]
        public ValueInput name;

        [DoNotSerialize, NullMeansSelf, PortLabelHidden]
        public ValueInput gameObject;

        [DoNotSerialize]
        public ValueInput set;

        [DoNotSerialize]
        public ValueOutput get;

        object _currentValue;

        protected override void Definition()
        {
            enter = ControlInput(nameof(enter), Enter);
            exit = ControlOutput(nameof(exit));
            name = ValueInput<string>(nameof(name), "");
            if (kind == VariableKind.Object)
            {
                gameObject = ValueInput<GameObject>(nameof(gameObject), null).NullMeansSelf();
            }
            set = ValueInput<object>(nameof(set));
            get = ValueOutput<object>(nameof(get), _ => _currentValue);

            Succession(enter, exit);
        }

        VariableDeclarations GetSource(Flow flow)
        {
            switch (kind)
            {
                case VariableKind.Flow:
                    return flow.variables;
                case VariableKind.Graph:
                    return graph.variables;
                case VariableKind.Object:
                    return Variables.Object(flow.GetValue<GameObject>(gameObject));
                case VariableKind.Scene:
                    return Variables.ActiveScene;
                case VariableKind.Application:
                    return Variables.Application;
                case VariableKind.Saved:
                    return Variables.Saved;
            }
            return null;
        }

        ControlOutput Enter(Flow flow)
        {
            var name = flow.GetValue<string>(this.name);
            var source = GetSource(flow);
            if (source.IsDefined(name))
            {
                _currentValue = source.Get(name);
                source.Set(name, flow.GetValue<object>(set));
            }
            return exit;
        }
    }
}