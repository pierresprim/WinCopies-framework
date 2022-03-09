namespace WinCopies.Data.MySQL
{
    public class Parameter<T> : SQL.Parameter<T>
    {
        public Parameter(in string name, in T value) : base(name, value) { /* Left empty. */ }

        public override string ToString() => '@' + Name;
    }

    public class Condition<T> : SQL.Condition<T>
    {
        public Condition(string @operator = "=") : base(@operator) { /* Left empty. */ }

        protected override string OnNull() => "NULL";
    }
}
