using MixItUp.Base;
using MixItUp.Base.Commands;
using MixItUp.Base.Model.Remote;
using MixItUp.WPF.Controls.Command;
using MixItUp.WPF.Controls.Dialogs;
using MixItUp.WPF.Controls.MainControls;
using MixItUp.WPF.Util;
using MixItUp.WPF.Windows.Command;
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

        private RemoteItemModel item;

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

        public void SetRemoteItem(RemoteItemModel item)
        {
            if (item != null && item.Command != null)
            {
                this.ClearCommand();

                this.AddCommandButton.Visibility = Visibility.Collapsed;
                this.ButtonRenderGrid.Visibility = Visibility.Visible;

                this.item = item;

                this.item.SetValuesFromCommand();

                this.item.Size = RemoteBoardItemSizeEnum.OneByOne;
                this.item.XPosition = this.xPosition;
                this.item.YPosition = this.yPosition;

                this.NameTextBlock.Text = this.item.Name;
                if (item is RemoteButtonItemModel)
                {
                    RemoteButtonItemModel button = this.item as RemoteButtonItemModel;
                    this.NameTextBlock.Foreground = new BrushConverter().ConvertFromString(button.TextColor) as SolidColorBrush;
                    this.BackgroundColor.Fill = new BrushConverter().ConvertFromString(button.BackgroundColor) as SolidColorBrush;
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
            this.ButtonRenderGrid.Visibility = Visibility.Collapsed;
        }

        private async void ButtonRenderGrid_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            await this.remoteControl.Window.RunAsyncOperation(async () =>
            {
                RemoteCommandButtonsDialogControl remoteCommandButtons = new RemoteCommandButtonsDialogControl(this.remoteControl.Window, this.item.Command);
                remoteCommandButtons.ActionsCompleted += RemoteCommandButtons_ActionsCompleted;
                await MessageBoxHelper.ShowCustomDialog(remoteCommandButtons);
                this.remoteControl.RefreshBoardsView();
            });
        }

        private void RemoteCommandButtons_ActionsCompleted(object sender, System.EventArgs e)
        {
            this.Dispatcher.Invoke(() =>
            {
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
            this.remoteControl.CurrentBoard.Items.Add(new RemoteButtonItemModel((RemoteCommand)e, this.xPosition, this.yPosition));
        }

        private async void Window_Closed(object sender, System.EventArgs e)
        {
            await this.remoteControl.Window.RunAsyncOperation(async () =>
            {
                await ChannelSession.SaveSettings();

                this.remoteControl.RefreshBoardsView();
            });
        }
    }
}
