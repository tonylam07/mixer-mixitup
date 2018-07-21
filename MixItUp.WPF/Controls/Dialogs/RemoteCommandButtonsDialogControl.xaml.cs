using MixItUp.Base;
using MixItUp.Base.Commands;
using MixItUp.WPF.Controls.Command;
using MixItUp.WPF.Util;
using MixItUp.WPF.Windows;
using MixItUp.WPF.Windows.Command;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace MixItUp.WPF.Controls.Dialogs
{
    /// <summary>
    /// Interaction logic for RemoteCommandButtonsDialogControl.xaml
    /// </summary>
    public partial class RemoteCommandButtonsDialogControl : UserControl
    {
        public event EventHandler ActionsCompleted = delegate { };

        private LoadingWindowBase window;
        private RemoteCommand command;

        public RemoteCommandButtonsDialogControl(LoadingWindowBase window, RemoteCommand command)
        {
            this.window = window;
            this.command = command;

            InitializeComponent();

            this.CommandNameTextBlock.Text = this.command.Name;
            this.CommandButtons.DataContext = this.command;
            this.CommandButtons.AskBeforeDelete = false;
        }

        private async void CommandButtons_EditClicked(object sender, RoutedEventArgs e)
        {
            MessageBoxHelper.CloseDialog();

            await Task.Delay(300);

            CommandButtonsControl commandButtonsControl = (CommandButtonsControl)sender;
            RemoteCommand command = commandButtonsControl.GetCommandFromCommandButtons<RemoteCommand>(sender);
            if (command != null)
            {
                CommandWindow window = new CommandWindow(new RemoteCommandDetailsControl(command));
                window.Closed += Window_Closed;
                window.Show();
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            this.ActionsCompleted(this, new EventArgs());
        }

        private async void CommandButtons_DeleteClicked(object sender, RoutedEventArgs e)
        {
            MessageBoxHelper.CloseDialog();

            await Task.Delay(300);

            await this.window.RunAsyncOperation(async () =>
            {
                if (await MessageBoxHelper.ShowConfirmationDialog("Are you sure you want to delete this command?"))
                {
                    CommandButtonsControl commandButtonsControl = (CommandButtonsControl)sender;
                    RemoteCommand command = commandButtonsControl.GetCommandFromCommandButtons<RemoteCommand>(sender);
                    if (command != null)
                    {
                        ChannelSession.Settings.RemoteCommands.Remove(command);
                        await ChannelSession.SaveSettings();
                        this.ActionsCompleted(this, new EventArgs());
                    }
                }
            });
        }
    }
}
