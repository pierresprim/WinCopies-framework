namespace WinCopies.Temp.Collections.DotNetFix
{
    public interface IReadOnlyArray<out T> : WinCopies.Collections.DotNetFix.Generic.ICountableEnumerable<T>
    {
        T this[int index] { get; }
    }

    public interface IArray<T> : IReadOnlyArray<T>
    {
        new T this[int index] { get; set; }
    }
}