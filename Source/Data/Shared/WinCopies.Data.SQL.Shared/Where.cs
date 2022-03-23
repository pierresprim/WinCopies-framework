using System.Collections;
using System.Collections.Generic;
using System.Text;

using WinCopies.Collections;
using WinCopies.Collections.DotNetFix.Generic;
using WinCopies.Temp;

namespace WinCopies.Data.SQL
{
    public interface IOperable
    {
        string
#if CS8
            ?
#endif
            Operator
        { get; }
    }

    public abstract class Operable : IOperable
    {
        public string
#if CS8
            ?
#endif
            Operator
        { get; set; }

        protected Operable(string
#if CS8
            ?
#endif
            @operator) => Operator = @operator;
    }

    public interface IParameter
    {
        string Name { get; }

        object
#if CS8
            ?
#endif
            Value
        { get; }
    }

    public interface IParameter<T> : IParameter
    {
        new T Value { get; }

#if CS8
        object? IParameter.Value => Value;
#endif
    }

    public abstract class Parameter<T> : IParameter<T>
    {
        public string Name { get; }

        public T Value { get; }

#if !CS8
        object IParameter.Value => Value;
#endif

        public Parameter(in string name, in T value)
        {
            Name = name;

            Value = value;
        }
    }

    public interface IColumnCondition
    {
        KeyValuePair<string
#if CS8
            ?
#endif
            , string> Column
        { get; }

        IParameter
#if CS8
            ?
#endif
            Parameter
        { get; }
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
        public KeyValuePair<string
#if CS8
            ?
#endif
            , string> Column
        { get; }

        public Parameter<T>
#if CS8
            ?
#endif
            Parameter
        { get; }

        IParameter
#if CS8
            ?
#endif
            IColumnCondition.Parameter => Parameter;

        public ColumnCondition(in KeyValuePair<string
#if CS8
            ?
#endif
            , string> column, in Parameter<T>
#if CS8
            ?
#endif
            parameter)
        {
            Column = column;

            Parameter = parameter;
        }

        public ColumnCondition(in string column, in Parameter<T>
#if CS8
            ?
#endif
            parameter) : this(new KeyValuePair<string
#if CS8
            ?
#endif
            , string>(null, column), parameter)
        { /* Left empty. */ }
    }

    public interface ICondition : IOperable
    {
        IColumnCondition InnerCondition { get; }

        string ToString(string decorator);
    }

    public interface ICondition<T> : ICondition
    {
        new ColumnCondition<T> InnerCondition { get; }

#if CS8
        IColumnCondition ICondition.InnerCondition => InnerCondition;
#endif
    }

    public abstract class Condition<T> : Operable, ICondition<T>
    {
        public ColumnCondition<T> InnerCondition { get; set; }

#if !CS8
        IColumnCondition ICondition.InnerCondition => InnerCondition;
#endif

        public Condition(string @operator = "=") : base(@operator) { /* Left empty. */ }

        protected abstract string OnNull();

        public string ToString(string
#if CS8
            ?
#endif
            decorator)
        {
            Parameter<T>
#if CS8
            ?
#endif
            parameter = InnerCondition.Parameter;

            string surround(in string item) => item.Surround(decorator);

            return $"{(InnerCondition.Column.Key == null ? surround(InnerCondition.Column.Value) : $"{surround(InnerCondition.Column.Key)}.{surround(InnerCondition.Column.Value)}")} {Operator} {(parameter == null ? OnNull() : parameter.ToString())}";
        }

        public override string ToString() => ToString(null);
    }

    public interface IConditionGroup : IOperable, System.Collections.Generic.IEnumerable<IParameter>
    {
        IUIntCountableEnumerable<ICondition>
#if CS8
            ?
#endif
            Conditions
        { get; set; }

        IUIntCountableEnumerable<IConditionGroup>
#if CS8
            ?
#endif
            ConditionGroups
        { get; set; }

        IUIntCountableEnumerable<KeyValuePair<SQLColumn, ISelect>>
#if CS8
            ?
#endif
            Selects
        { get; set; }

#if CS8
        bool HasConditions
        {
            get
            {
                static bool checkCount(in IUIntCountable collection) => collection != null && collection.Count > 0;

                return checkCount(Conditions) || checkCount(ConditionGroups) || checkCount(Selects);
            }
        }
#endif

        System.Collections.Generic.IEnumerable<IParameter> GetParameters()
#if CS8
            => DBEntityCollection.GetParameters(this);

        System.Collections.Generic.IEnumerator<IParameter> System.Collections.Generic.IEnumerable<IParameter>.GetEnumerator() => GetParameters().GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator()
#endif
        ;
    }

    public class ConditionGroup : Operable, IConditionGroup
    {
        public IUIntCountableEnumerable<ICondition>
#if CS8
            ?
#endif
            Conditions
        { get; set; }

        public IUIntCountableEnumerable<IConditionGroup>
#if CS8
            ?
#endif
            ConditionGroups
        { get; set; }

        public IUIntCountableEnumerable<KeyValuePair<SQLColumn, ISelect>>
#if CS8
            ?
#endif
            Selects
        { get; set; }

        public ConditionGroup(string
#if CS8
            ?
#endif
            @operator) : base(@operator) { /* Left empty. */ }

#if !CS8
        public IEnumerable<IParameter> GetParameters() => DBEntityCollection.GetParameters(this);

        private System.Collections.Generic.IEnumerator<IParameter> GetEnumerator() => GetParameters().GetEnumerator();

        System.Collections.Generic.IEnumerator<IParameter> IEnumerable<IParameter>.GetEnumerator() => GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
#endif

        public override string ToString()
        {
            var sb = new StringBuilder();

            void _append(in object obj) => sb.Append($"({obj})");

            ActionIn<object> append = (in object obj) =>
            {
                append = (in object _obj) =>
                {
                    _ = sb.Append($" {Operator} ");

                    _append(_obj);
                };

                _append(obj);
            };

            if (Conditions != null)

                foreach (ICondition
#if CS8
            ?
#endif
            condition in Conditions)

                    append(condition);

            if (ConditionGroups != null)

                foreach (IConditionGroup
#if CS8
            ?
#endif
            conditionGroup in ConditionGroups)

                    append(conditionGroup);

            if (Selects != null)

                foreach (KeyValuePair<SQLColumn, ISelect> select in Selects)

                    append($"{select.Key} = ({select.Value})");

            return sb.ToString();
        }
    }
}
