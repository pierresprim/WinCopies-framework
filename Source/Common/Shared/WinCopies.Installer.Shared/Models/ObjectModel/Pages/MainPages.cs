using System;

using static WinCopies.Installer.Error;

namespace WinCopies.Installer
{
    /// <summary>
    /// Dummy interface for data template.
    /// </summary>
    public interface IMainPage : IInstallerPage
    {
        // Left empty.
    }

    public interface IStartPage : IMainPage, IBrowsableForwardInstallerPage<ILicenseAgreementPage>
    {
        // Left empty.
    }

    /// <summary>
    /// Dummy interface for data template.
    /// </summary>
    public interface IEndPage : IMainPage
    {
        // Left empty.
    }

    public abstract class MainPage<TPrevious, TNext> : InstallerPage<TNext> where TNext : IInstallerPage
    {
        public MainPage(in Installer installer) : base(installer) { /* Left empty. */ }
    }

    public abstract class StartPage : MainPage<object, ILicenseAgreementPage>, IStartPage
    {
        public override string Title => $"Welcome to {Installer.ProgramName} Setup";

        public override string Description => $"This setup will guide you through the installation of {Installer.ProgramName}.\n\nIt is recommended to close all non-essential running applications, so that you may not have to reboot your computer if system files will have to be updated.\n\nClick {NextStepName} when you are ready to continue.";

        public StartPage(in Installer installer) : base(installer) { /* Left empty. */ }
    }

    public abstract class EndPage : MainPage<object, IInstallerPage>, IEndPage
    {
        private readonly string _title;
        private string _description;

        public override string Title => _title;

        public override string Description => _description;

        public override string NextStepName => "_Quit";

        protected EndPage(Installer installer) : base(installer)
        {
            string title;

            Error error = Installer.Error;

            void setDescription(in string message) => _description = $"{message} {(installer.RequiresRestart ? "You have to reboot your computer to use the program." : "You can now launch the program.")}";

            void _setDescription(in string
#if CS8
                ?
#endif
                message) => setDescription($"The setup has been completed successfully. {message}");

            void setFatalErrorDescription(in string
#if CS8
                ?
#endif
                message) => _description = $"A {message}fatal error occurred. The setup could not complete because one or more errors could not be recovered. You have to retry the installation to use the program. You should also notice that, as a result of this type of error, the setup process could not be reversed, so some files that could have been written during the process have not been deleted. Please notice that some actions (like shortcut creation, for example) could also have been executed and, if any, none of them could be reversed.";

            switch (error)
            {
                case Succeeded:

                    title = "completed";

                    _setDescription(null);

                    break;

                case RecoveredError:

                    title = "completed with recovered errors";

                    _setDescription("All errors could be retrieved. ");

                    break;

                case NotRecoveredError:

                    title = "completed with errors";

                    setDescription($"The setup has not been completed successfully. You may experience some problems when you will use the program. It is also possible that some actions (like shortcut creation, for example) may not have been executed partially, or even fully. A complete reinstallation is highly recommended.");

                    break;

                case FatalError:

                    title = "failed to complete";
                    setFatalErrorDescription(null);

                    break;

                case SuperFatalError:

                    title = "failed to restore previous state";
                    setFatalErrorDescription("super ");

                    break;

                default:

                    throw new InvalidOperationException($"The enumeration value {error} is not supported.");
            }

            _title = "Setup " + title;
        }

        protected override IInstallerPage
#if CS8
            ?
#endif
            GetNextPage() => null;
    }
}
