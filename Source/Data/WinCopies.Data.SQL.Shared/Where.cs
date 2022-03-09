using System.Collections;
using System.Text;

using WinCopies.Collections;
using WinCopies.Collections.DotNetFix.Generic;
using WinCopies.Temp;
using static WinCopies.ThrowHelper;

namespace WinCopies.Data.SQL
{
    public interface IOperable
    {
        string? Operator { get; }
    }

    public abstract class Operable : IOperable
    {
        public string? Operator { get; set; }

        protected Operable(string? @operator) => Operator = @operator;
    }

    public interface IParameter
    {
        string Name { get; }

        object? Value { get; }
    }

    public interface IParameter<T> : IParameter
    {
        new T Value { get; }

        object? IParameter.Value => Value;
    }

    public abstract class Parameter<T> : IParameter<T>
    {
        public string Name { get; }

        public T Value { get; }

        public Parameter(in string name, in T value)
        {
            Name = name;

            Value = value;
        }
    }

    public interface IColumnCondition
    {
        KeyValuePair<string?, string> Column { get; }

        IParameter? Parameter { get; }
    }

    public struct NullCondition
    {
        public string Operator { get; }

        public string NullValue { get; }

        public NullCondition(in string @operator, in string nullValue)
        {
            Operator = @operator;

            NullValue = nullValue;
        }
    }

    public struct ColumnCondition<T> : IColumnCondition
    {
        public KeyValuePair<string?, string> Column { get; }

        public Parameter<T>? Parameter { get; }

        IParameter? IColumnCondition.Parameter => Parameter;

        public ColumnCondition(in KeyValuePair<string?, string> column, in Parameter<T>? parameter)
        {
            Column = column;

            Parameter = parameter;
        }

        public ColumnCondition(in string column, in Parameter<T>? parameter) : this(new KeyValuePair<string?, string>(null, column), parameter) { /* Left empty. */ }
    }

    public interface ICondition : IOperable
    {
        IColumnCondition InnerCondition { get; }

        string ToString(string decorator);
    }

    public interface ICondition<T> : ICondition
    {
        new ColumnCondition<T> InnerCondition { get; }

        IColumnCondition ICondition.InnerCondition => InnerCondition;
    }

    public abstract class Condition<T> : Operable, ICondition<T>
    {
        public ColumnCondition<T> InnerCondition { get; set; }

        public Condition(string @operator = "=") : base(@operator) { /* Left empty. */ }

        protected abstract string OnNull();

        public string ToString(string? decorator)
        {
            Parameter<T>? parameter = InnerCondition.Parameter;

            string surround(in string item) => item.Surround(decorator);

            return $"{(InnerCondition.Column.Key == null ? surround(InnerCondition.Column.Value) : $"{surround(InnerCondition.Column.Key)}.{surround(InnerCondition.Column.Value)}")} {Operator} {(parameter == null ? OnNull() : parameter.ToString())}";
        }

        public override string ToString() => ToString(null);
    }

    public interface IConditionGroup : IOperable, System.Collections.Generic.IEnumerable<IParameter>
    {
        IUIntCountableEnumerable<ICondition>? Conditions { get; set; }

        IUIntCountableEnumerable<IConditionGroup>? ConditionGroups { get; set; }

        IUIntCountableEnumerable<KeyValuePair<SQLColumn, ISelect>>? Selects { get; set; }

        bool HasConditions
        {
            get
            {
                static bool checkCount(in IUIntCountable? collection) => collection != null && collection.Count > 0;

                return checkCount(Conditions) || checkCount(ConditionGroups) || checkCount(Selects);
            }
        }

        System.Collections.Generic.IEnumerable<IParameter> GetParameters()
        {
            if (Conditions != null)
            {
                System.Collections.Generic.IEnumerable<IParameter> enumerate()
                {
                    IParameter? parameter;

                    foreach (ICondition? condition in Conditions)
                    {
                        if ((parameter = condition?.InnerCondition.Parameter) == null)

                            continue;

                        yield return parameter;
                    }
                }

                foreach (IParameter parameter in enumerate())

                    yield return parameter;
            }

            if (ConditionGroups != null)
            {
                System.Collections.Generic.IEnumerable<IParameter> parameters;

                foreach (IConditionGroup? conditionGroup in ConditionGroups)
                {
                    if (conditionGroup == null)

                        continue;

                    parameters = conditionGroup.GetParameters();

                    foreach (IParameter parameter in parameters)

                        yield return parameter;
                }
            }

            if (Selects != null)
            {
                IConditionGroup? conditionGroup;

                foreach (KeyValuePair<SQLColumn, ISelect> select in Selects)
                {
                    conditionGroup = select.Value.ConditionGroup;

                    if (conditionGroup == null)

                        continue;

                    foreach (IParameter parameter in conditionGroup)

                        yield return parameter;
                }
            }
        }

        System.Collections.Generic.IEnumerator<IParameter> System.Collections.Generic.IEnumerable<IParameter>.GetEnumerator() => GetParameters().GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    public class ConditionGroup : Operable, IConditionGroup
    {
        public IUIntCountableEnumerable<ICondition>? Conditions { get; set; }

        public IUIntCountableEnumerable<IConditionGroup>? ConditionGroups { get; set; }

        public IUIntCountableEnumerable<KeyValuePair<SQLColumn, ISelect>>? Selects { get; set; }

        public ConditionGroup(string? @operator) : base(@operator) { /* Left empty. */ }

        public override string ToString()
        {
            var sb = new StringBuilder();

            void _append(in object obj) => sb.Append($"({obj})");

            ActionIn<object> append = (in object obj) =>
            {
                append = (in object obj) =>
                {
                    _ = sb.Append($" {Operator} ");

                    _append(obj);
                };

                _append(obj);
            };

            if (Conditions != null)

                foreach (ICondition? condition in Conditions)

                    append(condition);

            if (ConditionGroups != null)

                foreach (IConditionGroup? conditionGroup in ConditionGroups)

                    append(conditionGroup);

            if (Selects != null)

                foreach (KeyValuePair<SQLColumn, ISelect> select in Selects)

                    append($"{select.Key} = ({select.Value})");

            return sb.ToString();
        }
    }
}
