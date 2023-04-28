/* Copyright © Pierre Sprimont, 2022
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

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;

using WinCopies.Collections;
using WinCopies.Collections.Generic;

using static WinCopies.Installer.Error;

namespace WinCopies.Installer
{
    public partial class ProcessPageViewModel
    {
        protected partial class ProcessDataViewModel
        {
            protected partial class BackgroundWorker
            {
                private enum FlagPosition : byte
                {
                    IsOK = 0,
                    HasError = 1,
                    InstallationFileChanged,
                    ClearTemporaryFiles
                }

                private partial class ProcessorInfo
                {
                    public readonly struct ProcessAction
                    {
                        public string Name { get; }
                        public Action Action { get; }
                        public Error Error { get; }
                        public bool Force { get; }

                        public ProcessAction(in string name, in bool force, in Error error, in Action action)
                        {
                            Name = name;
                            Force = force;
                            Error = error;
                            Action = action;
                        }
                    }

                    public delegate ProcessAction ActionProvider(Processor processor);

                    private readonly Processor _processor;
                    private readonly Collections.DotNetFix.Generic.IQueue<ActionProvider> _actionProviders = new Collections.DotNetFix.Generic.Queue<ActionProvider>();

                    private BackgroundWorker Worker { get; }

                    public EnumerableHelper<ITemporaryFileEnumerable>.IEnumerableLinkedList
#if CS8
                        ?
#endif
                        TemporaryFiles
                    { get; private set; }

                    public ProcessorInfo(in BackgroundWorker worker, in DoWorkEventArgs doWorkEventArgs)
                    {
                        Worker = worker;

                        _processor = new Processor(this, doWorkEventArgs);
                    }

                    public void AddAction(in ActionProvider actionProvider) => _actionProviders.Enqueue(actionProvider);

                    public void AddActions(in IEnumerable<ActionProvider> actionProviders)
                    {
                        foreach (ActionProvider actionProvider in actionProviders)

                            AddAction(actionProvider);
                    }

                    public void AddPrepareActions() => AddActions(_processor.GetPrepareActions());
                    public void AddInstallActions() => AddActions(_processor.GetInstallActions());

                    public void DoWork(in Func<Processor, Action, Action, Action> actionProvider)
                    {
                        void log(in string message) => _processor.Log(message);

                        ActionIn<string> incrementStep = (in string actionName) =>
                        {
                            log(actionName);

                            incrementStep = (in string stepName) =>
                            {
                                log($"Starting {stepName}");

                                Worker.ProcessData.StepName = stepName;

                                Worker.ProcessData.IncrementStep();
                            };
                        };

                        bool doWork(in ProcessAction processAction)
                        {
                            try
                            {
                                incrementStep(processAction.Name);

                                processAction.Action();

                                if (_processor.HasFatalErrorOccurred())

                                    return true;

                                log($"{processAction.Name} Completed\n");

                                return false;
                            }

                            catch (Exception ex)
                            {
                                _ = MessageBox.Show("An error occurred. Message:\n" + ex.Message, "Install Error", MessageBoxButton.OK, MessageBoxImage.Error);

                                _processor.Error = processAction.Error;
                            }

                            return true;
                        }

                        bool ok;

                        PredicateIn<ProcessAction> predicate = (in ProcessAction processAction) =>
                        {
                            ok = doWork(processAction);

                            predicate = (in ProcessAction _processAction) => (ok || _processAction.Force) && doWork(_processAction);

                            return ok;
                        };

                        if (doWork(new ProcessAction("Initialization", false, FatalError, actionProvider(_processor, () => TemporaryFiles = EnumerableHelper<ITemporaryFileEnumerable>.GetEnumerableLinkedList(), () => TemporaryFiles = null))))

                            return;

                        Worker.ProcessData.SetTotalSteps((byte)_actionProviders.Count);

                        foreach (ActionProvider _actionProvider in _actionProviders.GetDequeuerEnumerable())

                            if (predicate(_actionProvider(_processor)))

                                ok = false;
                    }
                }
            }
        }
    }
}