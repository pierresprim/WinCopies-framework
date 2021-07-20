/* Copyright © Pierre Sprimont, 2021
*
* This file is part of the WinCopies Framework.
*
* The WinCopies Framework is free software: you can redistribute it and/or modify
* it under the terms of the GNU General Public License as published by
* the Free Software Foundation, either version 3 of the License, or
* (at your option) any later version.
*
* The WinCopies Framework is distributed in the hope that it will be useful,
* but WITHOUT ANY WARRANTY; without even the implied warranty of
* MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
* GNU General Public License for more details.
*
* You should have received a copy of the GNU General Public License
* along with the WinCopies Framework.  If not, see <https://www.gnu.org/licenses/>. */

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

using WinCopies.Commands;
using WinCopies.GUI.IO.Process;
using WinCopies.Util.Data;

using static System.Collections.Specialized.NotifyCollectionChangedAction;

using static WinCopies.Util.Desktop.UtilHelpers;

namespace WinCopies.GUI.IO
{
    namespace Controls.Process
    {
        public class ProcessManagerControl : System.Windows.Controls.Control
        {
            private static DependencyProperty Register<T>(in string propertyName) => Register<T, ProcessManagerControl>(propertyName);

            public static readonly DependencyProperty ProcessesProperty = Register<System.Collections.Generic.IEnumerable<IProcess>>(nameof(Processes));

            public System.Collections.Generic.IEnumerable<IProcess> Processes { get => (System.Collections.Generic.IEnumerable<IProcess>)GetValue(ProcessesProperty); set => SetValue(ProcessesProperty, value); }

            public static readonly DependencyProperty SelectedItemProperty = Register<IProcess>(nameof(SelectedItem));

            public IProcess SelectedItem { get => (IProcess)GetValue(SelectedItemProperty); set => SetValue(SelectedItemProperty, value); }

            public static readonly DependencyProperty ClearCompletedProcessesAutomaticallyProperty = Register<bool>(nameof(ClearCompletedProcessesAutomatically));

            public bool ClearCompletedProcessesAutomatically { get => (bool)GetValue(ClearCompletedProcessesAutomaticallyProperty); set => SetValue(ClearCompletedProcessesAutomaticallyProperty, value); }

            public static readonly DependencyProperty ClearCompletedProcessesProperty = Register<ICommand>(nameof(ClearCompletedProcesses));

            public ICommand ClearCompletedProcesses { get => (ICommand)GetValue(ClearCompletedProcessesProperty); set => SetValue(ClearCompletedProcessesProperty, value); }

            static ProcessManagerControl() => DefaultStyleKeyProperty.OverrideMetadata(typeof(ProcessManagerControl), new FrameworkPropertyMetadata(typeof(ProcessManagerControl)));
        }
    }

    namespace Process
    {
        public interface IProcessManager
        {
            System.Collections.Generic.IEnumerable<IProcess> Processes { get; set; }

            IProcess SelectedItem { get; set; }

            bool ClearCompletedProcessesAutomatically { get; set; }

            ICommand ClearCompletedProcesses { get; }
        }

        public class ProcessManager<TEnumerable, TItems> : ViewModelBase, IProcessManager where TEnumerable : System.Collections.Generic.IEnumerable<TItems> where TItems : IProcess
        {
            private TEnumerable _processes;
            private TItems _selectedItem;
            private bool _clearCompletedProcessesAutomatically;

            public TEnumerable Processes { get => _processes; set => UpdateValue(ref _processes, value, nameof(Processes)); }

            public TItems SelectedItem { get => _selectedItem; set => UpdateValue(ref _selectedItem, value, nameof(SelectedItem)); }

            public bool ClearCompletedProcessesAutomatically { get => _clearCompletedProcessesAutomatically; set => UpdateValue(ref _clearCompletedProcessesAutomatically, value, nameof(ClearCompletedProcessesAutomatically)); }

            public virtual ICommand ClearCompletedProcesses => null;

            IEnumerable<IProcess> IProcessManager.Processes { get => _processes.Select<TItems, IProcess>(process => process); set => Processes = (TEnumerable)value; }

            IProcess IProcessManager.SelectedItem { get => _selectedItem; set => SelectedItem = (TItems)value; }
        }

        public class ProcessManager<TItems> : ProcessManager<ObservableCollection<TItems>, TItems> where TItems : IProcess
        {
            public override ICommand ClearCompletedProcesses => new DelegateCommand(parameter =>
            {
                if (ClearCompletedProcessesAutomatically)
                {
                    foreach (TItems process in Processes)

                        if (process.Status == WinCopies.IO.Process.ProcessStatus.Succeeded)

                            return true;

                    return false;
                }

                return true;
            }, parameter => OnClearCompletedProcesses());

            protected virtual void OnClearCompletedProcesses()
            {
                for (int i = 0; i < Processes.Count;)

                    if (Processes[i].Status == WinCopies.IO.Process.ProcessStatus.Succeeded)

                        Processes.RemoveAt(i);

                    else

                        i++;
            }

            protected override void UpdateValue<T>(ref T value, in T newValue, in string propertyName)
            {
#if CS8
                static
#endif
                    TOut getValue<TOut>(in object _value) => (TOut)_value;

#if CS8
                static
#endif
                    ObservableCollection<TItems> getItems(in object _value) => getValue<ObservableCollection<TItems>>(_value);

                switch (propertyName)
                {
                    case nameof(ClearCompletedProcessesAutomatically):

                        if (getValue<bool>(newValue) && Processes != null)

                            AddEvents(Processes);

                        else

                            RemoveEvents(Processes);

                        break;

                    case nameof(Processes):

                        if (ClearCompletedProcessesAutomatically)
                        {
                            if (value != null)

                                RemoveEvents(getItems(value));

                            if (newValue != null)

                                AddEvents(getItems(newValue));
                        }

                        break;
                }

                base.UpdateValue(ref value, newValue, propertyName);
            }

            private void AddEvents(in ObservableCollection<TItems> items)
            {
                foreach (TItems _process in items)

                    AddEvent(_process);

                items.CollectionChanged += ProcessManager_CollectionChanged;
            }

            private void RemoveEvents(in ObservableCollection<TItems> items)
            {
                items.CollectionChanged -= ProcessManager_CollectionChanged;

                foreach (TItems process in items)

                    RemoveEvent(process);
            }

            private void AddEvent(in TItems process) => process.RunWorkerCompleted += Process_RunWorkerCompleted;

            private void RemoveEvent(in TItems process) => process.RunWorkerCompleted -= Process_RunWorkerCompleted;

            private void ProcessManager_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
            {
                switch (e.Action)
                {
                    case Add:

                        foreach (TItems process in e.NewItems)

                            AddEvent(process);

                        break;

                    case Remove:

                        foreach (TItems process in e.OldItems)

                            RemoveEvent(process);

                        break;

                    case Replace:

                        foreach (TItems process in e.OldItems)

                            RemoveEvent(process);

                        foreach (TItems _process in e.NewItems)

                            AddEvent(_process);

                        break;

                    case Reset:

                        foreach (TItems process in e.OldItems)

                            RemoveEvent(process);

                        break;
                }
            }

            private void Process_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
            {
                var process = (TItems)sender;

                if (process.Status == WinCopies.IO.Process.ProcessStatus.Succeeded)

                    _ = Processes.Remove(process);
            }
        }
    }
}
