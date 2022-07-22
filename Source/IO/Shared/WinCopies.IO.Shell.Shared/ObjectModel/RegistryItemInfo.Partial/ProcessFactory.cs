using System;
using System.Security;

using WinCopies.IO.Process;
using WinCopies.IO.Process.ObjectModel;
using WinCopies.IO.PropertySystem;
using WinCopies.Util;

namespace WinCopies.IO.ObjectModel
{
    public partial class RegistryItemInfo
    {
        protected class _ProcessFactory : ProcessInfoNullableProcessFactory
        {
            protected class _NewItemProcessCommands : IProcessCommand
            {
                private IRegistryItemInfo _registryItemInfo;

                public bool IsDisposed => _registryItemInfo == null;

                public string Name { get; } = "New key";

                public string Caption { get; } = "Key name:";

                public _NewItemProcessCommands(in IRegistryItemInfo registryItemInfo) => _registryItemInfo = registryItemInfo;

                public bool CanExecute(System.Collections.Generic.IEnumerable<IBrowsableObjectInfo> items) => _registryItemInfo.ObjectProperties.RegistryItemType == RegistryItemType.Key && !_registryItemInfo.InnerObject.Name.StartsWith(Microsoft.Win32.Registry.LocalMachine.Name);

                public bool TryExecute(string parameter, System.Collections.Generic.IEnumerable<IBrowsableObjectInfo> items, out IProcessParameters result)
                {
                    result = null;

                    try
                    {
                        _ = _registryItemInfo.InnerObject.CreateSubKey(parameter);

                        return true;
                    }

                    catch (Exception ex) when (ex.Is(false, typeof(SecurityException), typeof(UnauthorizedAccessException), typeof(System.IO.IOException)))
                    {
                        return false;
                    }
                }

                public IProcessParameters Execute(string parameter, System.Collections.Generic.IEnumerable<IBrowsableObjectInfo> items) => TryExecute(parameter, items, out IProcessParameters result)
                        ? result
                        : throw new InvalidOperationException(Shell.Resources.ExceptionMessages.CouldNotCreateItem);

                public void Dispose() => _registryItemInfo = null;

                ~_NewItemProcessCommands() => Dispose();
            }

            private bool _isDisposed;

            public sealed override bool IsDisposed => _isDisposed;

            public sealed override IProcessCommand NewItemProcessCommand { get; }

            public override IProcessCommand
#if CS8
                ?
#endif
                RenameItemProcessCommand => null;

            public _ProcessFactory(in IRegistryItemInfo registryItemInfo) => NewItemProcessCommand = new _NewItemProcessCommands(registryItemInfo);

            public override IProcess GetProcess(ProcessFactorySelectorDictionaryParameters processParameters) => throw new NotSupportedException();

            public override IProcess
#if CS8
                ?
#endif
            TryGetProcess(ProcessFactorySelectorDictionaryParameters processParameters) => null;

            public sealed override bool CanPaste(uint count) => false;

            protected override void DisposeManaged() => _isDisposed = true;
            protected override void DisposeUnmanaged() { /* Left empty. */ }
        }
    }
}
