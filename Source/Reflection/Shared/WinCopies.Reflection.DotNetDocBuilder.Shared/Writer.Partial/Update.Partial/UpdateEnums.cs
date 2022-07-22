using WinCopies.Data.SQL;
using WinCopies.Reflection.DotNetParser;

namespace WinCopies.Reflection.DotNetDocBuilder
{
    public partial class Writer
    {
        protected struct EnumCallback : IWriterCallback<Enum, DotNetEnum>, DotNetFix.IDisposable
        {
            private DBEntityCollection<UnderlyingType> _utColl;

            public bool IsDisposed => _utColl == null;

            public EnumCallback(in Writer writer) => _utColl = new DBEntityCollection<UnderlyingType>(writer.Connection);

            private void SetUnderlyingType(Enum @enum, DotNetEnum dotNetEnum) => @enum.UnderlyingType = new UnderlyingType(ThrowHelper.GetOrThrowIfDisposed(this, _utColl)) { Name = dotNetEnum.UnderlyingType.ToCSName() };

            public void OnGetItem(Enum item, Type type, DotNetEnum @enum, DBEntityCollection<Enum> enums) => SetUnderlyingType(item, @enum);
            public void OnAdded(Enum item, Type type, DotNetEnum dotNetType) { /* Left empty. */ }
            public void OnUpdated(Enum item, DotNetEnum dotNetType, DBEntityCollection<Enum> collection) => SetUnderlyingType(item, dotNetType);
            public void OnDeleting(Enum item, Type type) { /* Left empty. */ }

            public void Dispose()
            {
                if (IsDisposed) return;

                _utColl.Dispose();
                _utColl = null;
            }
        }

        public void UpdateEnums() => UpdateItems("Enum", false, Bool.True, item => item.Type, GetAllEnumsInPackages, Enum.GetNewEnum, CreateEnumFile, null, false, new EnumCallback(this), GetConstantUpdater<Enum, DotNetEnum>(e => e.Type, item => item.GetEnumUnderlyingType()));
    }
}
