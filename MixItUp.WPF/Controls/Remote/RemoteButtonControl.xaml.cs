using MixItUp.Base;
using MixItUp.Base.Commands;
using MixItUp.Base.Model.Remote;
using MixItUp.WPF.Controls.Command;
using MixItUp.WPF.Controls.MainControls;
using MixItUp.WPF.Windows.Command;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace MixItUp.WPF.Controls.Remote
{
    /// <summary>
    /// Interaction logic for RemoteButtonControl.xaml
    /// </summary>
    public partial class RemoteButtonControl : UserControl
    {
        private RemoteControl remoteControl;
        private int xPosition;
        private int yPosition;

        private RemoteBoardItemModel item;

        public RemoteButtonControl()
        {
            InitializeComponent();
        }

        public void Initialize(RemoteControl remoteControl, int xPosition, int yPosition)
        {
            this.remoteControl = remoteControl;
            this.xPosition = xPosition;
            this.yPosition = yPosition;
        }

        public void SetRemoteItem(RemoteBoardItemModel item)
        {
            if (item != null && item.Command != null)
            {
                this.ClearCommand();

                this.AddCommandButton.Visibility = Visibility.Collapsed;
                this.CommandButtonsPopup.Visibility = Visibility.Visible;

                this.item = item;

                this.item.SetValuesFromCommand();

                this.item.Size = RemoteBoardItemSizeEnum.OneByOne;
                this.item.XPosition = this.xPosition;
                this.item.YPosition = this.yPosition;

                this.NameTextBlock.Text = this.item.Name;
                if (item is RemoteBoardButtonModel)
                {
                    RemoteBoardButtonModel button = this.item as RemoteBoardButtonModel;
                    this.NameTextBlock.Foreground = new BrushConverter().ConvertFromString(button.TextColor) as SolidColorBrush;
                    this.BackgroundColor.Fill = new BrushConverter().ConvertFromString(button.BackgroundColor) as SolidColorBrush;

                    //this.CommandButtons.DataContext = this.item.Command;
                }
                return;
            }
        }

        public void ClearCommand()
        {
            this.item = null;
            this.NameTextBlock.Text = "";
            this.NameTextBlock.Foreground = Brushes.Black;
            this.BackgroundColor.Fill = Brushes.Transparent;

            this.AddCommandButton.Visibility = Visibility.Visible;
            this.CommandButtonsPopup.Visibility = Visibility.Collapsed;
        }

        private void CommandButtons_EditClicked(object sender, RoutedEventArgs e)
        {
            CommandButtonsControl commandButtonsControl = (CommandButtonsControl)sender;
            RemoteCommand command = commandButtonsControl.GetCommandFromCommandButtons<RemoteCommand>(sender);
            if (command != null)
            {
                CommandWindow window = new CommandWindow(new RemoteCommandDetailsControl(command));
                window.Closed += Window_Closed;
                window.Show();
            }
        }

        private async void CommandButtons_DeleteClicked(object sender, RoutedEventArgs e)
        {
            await this.remoteControl.Window.RunAsyncOperation(async () =>
            {
                CommandButtonsControl commandButtonsControl = (CommandButtonsControl)sender;
                RemoteCommand command = commandButtonsControl.GetCommandFromCommandButtons<RemoteCommand>(sender);
                if (command != null)
                {
                    ChannelSession.Settings.RemoteCommands.Remove(command);
                    await ChannelSession.SaveSettings();

                    this.ClearCommand();
                    this.remoteControl.RefreshBoardsView();
                }
            });
        }

        private async void Window_Closed(object sender, System.EventArgs e)
        {
            await this.remoteControl.Window.RunAsyncOperation(async () =>
            {
                await ChannelSession.SaveSettings();

                this.remoteControl.RefreshBoardsView();
            });
        }

        private void AddCommandButton_Click(object sender, RoutedEventArgs e)
        {
            CommandWindow window = new CommandWindow(new RemoteCommandDetailsControl());
            window.CommandSaveSuccessfully += Window_CommandSaveSuccessfully;
            window.Closed += Window_Closed;
            window.Show();
        }

        private void Window_CommandSaveSuccessfully(object sender, CommandBase e)
        {
            this.remoteControl.CurrentBoard.Items.Add(new RemoteBoardButtonModel((RemoteCommand)e, this.xPosition, this.yPosition));
        }
    }
}
