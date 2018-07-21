using ImageProcessor;
using ImageProcessor.Imaging.Formats;
using MixItUp.Base;
using MixItUp.Base.Commands;
using MixItUp.Base.Model.Remote;
using MixItUp.Base.Services;
using MixItUp.WPF.Controls.Command;
using MixItUp.WPF.Util;
using MixItUp.WPF.Windows.Command;
using System;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace MixItUp.WPF.Controls.MainControls
{
    /// <summary>
    /// Interaction logic for RemoteControl.xaml
    /// </summary>
    public partial class RemoteControl : MainControlBase
    {
        public RemoteBoardModel CurrentBoard { get; private set; }

        private ObservableCollection<RemoteBoardModel> boards = new ObservableCollection<RemoteBoardModel>();

        public RemoteControl()
        {
            InitializeComponent();
        }

        protected override Task InitializeInternal()
        {
            foreach (RemoteBoardModel board in ChannelSession.Settings.RemoteBoards)
            {
                foreach (RemoteItemModel item in board.Items.Where(i => i.Command == null).ToList())
                {
                    board.Items.Remove(item);
                }

                foreach (RemoteItemModel item in board.Items)
                {
                    item.SetValuesFromCommand();
                }
            }

            this.BoardNameComboBox.ItemsSource = this.boards;

            this.Button00.Initialize(this, 0, 0);
            this.Button10.Initialize(this, 1, 0);
            this.Button20.Initialize(this, 2, 0);
            this.Button30.Initialize(this, 3, 0);
            this.Button40.Initialize(this, 4, 0);
            this.Button01.Initialize(this, 0, 1);
            this.Button11.Initialize(this, 1, 1);
            this.Button21.Initialize(this, 2, 1);
            this.Button31.Initialize(this, 3, 1);
            this.Button41.Initialize(this, 4, 1);
            this.Button02.Initialize(this, 0, 2);
            this.Button12.Initialize(this, 1, 2);
            this.Button22.Initialize(this, 2, 2);
            this.Button32.Initialize(this, 3, 2);
            this.Button42.Initialize(this, 4, 2);

            this.RefreshBoardsView();

            return base.InitializeInternal();
        }

        public void RefreshBoardsView()
        {
            this.boards.Clear();
            foreach (RemoteBoardModel board in ChannelSession.Settings.RemoteBoards)
            {
                this.boards.Add(board);
            }

            if (this.boards.Count > 0)
            {
                if (this.CurrentBoard != null)
                {
                    this.BoardNameComboBox.SelectedItem = this.CurrentBoard;
                }
                else
                {
                    this.BoardNameComboBox.SelectedIndex = 0;
                }
            }
            else
            {
                this.RefreshBoardItemsView();
            }
        }

        private void BoardNameComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.BoardNameComboBox.SelectedIndex >= 0)
            {
                this.CurrentBoard = (RemoteBoardModel)this.BoardNameComboBox.SelectedItem;
                this.RefreshBoardItemsView();
            }
        }

        private async void BoardAddButton_Click(object sender, RoutedEventArgs e)
        {
            await this.Window.RunAsyncOperation(async () =>
            {
                string name = await MessageBoxHelper.ShowTextEntryDialog("Board Name");
                if (!string.IsNullOrEmpty(name))
                {
                    if (ChannelSession.Settings.RemoteBoards.Any(b => b.Name.Equals(name)))
                    {
                        await MessageBoxHelper.ShowMessageDialog("There already exists a board with this name");
                        return;
                    }

                    ChannelSession.Settings.RemoteBoards.Add(new RemoteBoardModel(name));
                    await ChannelSession.SaveSettings();

                    this.RefreshBoardsView();
                }
            });
        }

        private async void BoardEditButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.BoardNameComboBox.SelectedIndex >= 0)
            {
                RemoteBoardModel board = (RemoteBoardModel)this.BoardNameComboBox.SelectedItem;
                await this.Window.RunAsyncOperation(async () =>
                {
                    string name = await MessageBoxHelper.ShowTextEntryDialog("Board Name", board.Name);
                    if (!string.IsNullOrEmpty(name))
                    {
                        board.Name = name;
                        await ChannelSession.SaveSettings();

                        this.RefreshBoardsView();
                    }
                });
            }
        }

        private async void BoardDeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.BoardNameComboBox.SelectedIndex >= 0)
            {
                await this.Window.RunAsyncOperation(async () =>
                {
                    if (this.boards.Count == 1)
                    {
                        await MessageBoxHelper.ShowMessageDialog("You must have at least 1 board.");
                        return;
                    }

                    if (await MessageBoxHelper.ShowConfirmationDialog("Are you sure you wish to delete this board?"))
                    {
                        RemoteBoardModel board = (RemoteBoardModel)this.BoardNameComboBox.SelectedItem;
                        ChannelSession.Settings.RemoteBoards.Remove(board);
                        await ChannelSession.SaveSettings();

                        this.CurrentBoard = null;
                        this.RefreshBoardsView();
                    }
                });
            }
        }

        private void RefreshBoardItemsView()
        {
            this.Button00.ClearCommand();
            this.Button10.ClearCommand();
            this.Button20.ClearCommand();
            this.Button30.ClearCommand();
            this.Button40.ClearCommand();
            this.Button01.ClearCommand();
            this.Button11.ClearCommand();
            this.Button21.ClearCommand();
            this.Button31.ClearCommand();
            this.Button41.ClearCommand();
            this.Button02.ClearCommand();
            this.Button12.ClearCommand();
            this.Button22.ClearCommand();
            this.Button32.ClearCommand();
            this.Button42.ClearCommand();

            if (this.CurrentBoard != null)
            {
                foreach (RemoteItemModel item in this.CurrentBoard.Items)
                {
                    if (item.YPosition == 0)
                    {
                        if (item.XPosition == 0)
                        {
                            this.Button00.SetRemoteItem(item);
                        }
                        else if (item.XPosition == 1)
                        {
                            this.Button10.SetRemoteItem(item);
                        }
                        else if (item.XPosition == 2)
                        {
                            this.Button20.SetRemoteItem(item);
                        }
                        else if (item.XPosition == 3)
                        {
                            this.Button30.SetRemoteItem(item);
                        }
                        else if (item.XPosition == 3)
                        {
                            this.Button40.SetRemoteItem(item);
                        }
                    }
                    else if (item.YPosition == 1)
                    {
                        if (item.XPosition == 0)
                        {
                            this.Button01.SetRemoteItem(item);
                        }
                        else if (item.XPosition == 1)
                        {
                            this.Button11.SetRemoteItem(item);
                        }
                        else if (item.XPosition == 2)
                        {
                            this.Button21.SetRemoteItem(item);
                        }
                        else if (item.XPosition == 3)
                        {
                            this.Button31.SetRemoteItem(item);
                        }
                        else if (item.XPosition == 4)
                        {
                            this.Button41.SetRemoteItem(item);
                        }
                    }
                    else if (item.YPosition == 2)
                    {
                        if (item.XPosition == 0)
                        {
                            this.Button02.SetRemoteItem(item);
                        }
                        else if (item.XPosition == 1)
                        {
                            this.Button12.SetRemoteItem(item);
                        }
                        else if (item.XPosition == 2)
                        {
                            this.Button22.SetRemoteItem(item);
                        }
                        else if (item.XPosition == 3)
                        {
                            this.Button32.SetRemoteItem(item);
                        }
                        else if (item.XPosition == 4)
                        {
                            this.Button42.SetRemoteItem(item);
                        }
                    }
                }
            }
        }

        private async void ConnectStreamDeckButton_Click(object sender, RoutedEventArgs e)
        {
            await this.Window.RunAsyncOperation(async () =>
            {
                await ChannelSession.SaveSettings();

                if (await ChannelSession.Services.InitializeStreamDeck(this.CurrentBoard))
                {
                    ChannelSession.Services.StreamDeck.ConnectionStateChangeOccurred += StreamDeck_ConnectionStateChangeOccurred;
                    ChannelSession.Services.StreamDeck.KeyChangeOccurred += StreamDeck_KeyChangeOccurred;
                    ChannelSession.Services.StreamDeck.CommandRun += StreamDeck_CommandRun;

                    this.SetupGrid.Visibility = Visibility.Collapsed;
                    this.ConnectToStreamDeckGrid.Visibility = Visibility.Visible;

                    int keyCount = await ChannelSession.Services.StreamDeck.GetKeyCount();
                    int iconSize = await ChannelSession.Services.StreamDeck.GetIconSize();

                    Bitmap bitmap = new Bitmap(iconSize, iconSize, PixelFormat.Format24bppRgb);
                    for (int x = 0; x < iconSize; x++)
                    {
                        for (int y = 0; y < iconSize; y++)
                        {
                            bitmap.SetPixel(x, y, Color.White);
                        }
                    }

                    using (MemoryStream ms = new MemoryStream())
                    {
                        bitmap.Save(ms, ImageFormat.Bmp);
                        for (int i = 0; i < keyCount; i++)
                        {
                            await ChannelSession.Services.StreamDeck.SetKeyBitmapStream(i, ms);
                        }
                    }


                    byte[] byteData = await this.ResizeImageToStandardSize(@"C:\Users\Matthew\Downloads\fortnite---button-1520296499714_160h.jpg");

                    await ChannelSession.Services.StreamDeck.SetKeyBitmapBytes(7, byteData);
                }
                else
                {
                    await MessageBoxHelper.ShowMessageDialog("Failed to find/connect to a Stream Deck. Please ensure it is plugged in");
                }
            });
        }

        private void StreamDeck_ConnectionStateChangeOccurred(object sender, bool e)
        {
            this.SetStreamDeckEventsText("Stream Deck " + ((e) ? "Connected" : "Disconnected"));
        }

        private void StreamDeck_KeyChangeOccurred(object sender, StreamDeckKeyEvent e)
        {
            if (e.IsDown)
            {
                Tuple<int, int> keyPosition = ChannelSession.Services.StreamDeck.GetPositionIndexForKeyID(e.KeyID);
                this.SetStreamDeckEventsText("Key [" + keyPosition.Item1 + "," + keyPosition.Item2 + "] Pressed");
            }
        }

        private void StreamDeck_CommandRun(object sender, RemoteCommand e)
        {
            this.SetStreamDeckEventsText("Command " + e.Name + " Run");
        }

        private void SetStreamDeckEventsText(string text)
        {
            this.Dispatcher.Invoke(() =>
            {
                this.StreamDeckEventsTextBlock.Text += text + Environment.NewLine;
            });
        }

        private async void DisconnectStreamDeckButton_Click(object sender, RoutedEventArgs e)
        {
            await this.DisconnectStreamDeck();
        }

        private async Task DisconnectStreamDeck()
        {
            await this.Window.RunAsyncOperation(async () =>
            {
                ChannelSession.Services.StreamDeck.ConnectionStateChangeOccurred -= StreamDeck_ConnectionStateChangeOccurred;
                ChannelSession.Services.StreamDeck.KeyChangeOccurred -= StreamDeck_KeyChangeOccurred;
                ChannelSession.Services.StreamDeck.CommandRun -= StreamDeck_CommandRun;

                await ChannelSession.Services.DisconnectStreamDeck();

                this.ConnectToStreamDeckGrid.Visibility = Visibility.Collapsed;
                this.SetupGrid.Visibility = Visibility.Visible;
            });
        }

        private async void ConnectMixItUpRemoteButton_Click(object sender, RoutedEventArgs e)
        {
            await this.Window.RunAsyncOperation(async () =>
            {
                await ChannelSession.SaveSettings();

                ChannelSession.Services.Remote.OnDisconnectOccurred += RemoteService_OnDisconnectOccurred;
                ChannelSession.Services.Remote.OnAuthRequest += RemoteService_OnAuthRequest;
                ChannelSession.Services.Remote.OnNewAccessCode += RemoteService_OnNewAccessCode;
                ChannelSession.Services.Remote.OnBoardRequest += RemoteService_OnBoardRequest;
                ChannelSession.Services.Remote.OnActionRequest += RemoteService_OnActionRequest;

                await ChannelSession.Services.Remote.Connect();

                this.SetupGrid.Visibility = Visibility.Collapsed;
                this.ConnectToRemoteGrid.Visibility = Visibility.Visible;

                this.AccessCodeTextBlock.Text = ChannelSession.Services.Remote.AccessCode;
                this.RemoteEventsTextBlock.Text = string.Empty;
            });
        }

        private async void DisconnectRemoteButton_Click(object sender, RoutedEventArgs e)
        {
            await this.DisconnectRemote();
        }

        private async void RemoteService_OnDisconnectOccurred(object sender, System.Net.WebSockets.WebSocketCloseStatus e)
        {
            this.SetRemoteEventsText("Disconnection occurred, attempting reconnection...");

            await this.DisconnectRemote();
        }

        private async void RemoteService_OnAuthRequest(object sender, AuthRequestRemoteMessage authRequest)
        {
            this.SetRemoteEventsText("Device Authorization Requested: " + authRequest.DeviceInfo);

            await this.Window.RunAsyncOperation(async () =>
            {
                if (await MessageBoxHelper.ShowConfirmationDialog("The following device would like to connect:" + Environment.NewLine + Environment.NewLine +
                    authRequest.DeviceInfo + Environment.NewLine + Environment.NewLine +
                    "Would you like to approve this device?"))
                {
                    await ChannelSession.Services.Remote.SendAuthClientGrant(authRequest);
                    this.SetRemoteEventsText("Device Authorization Approved: " + authRequest.DeviceInfo);
                }
                else
                {
                    await ChannelSession.Services.Remote.SendAuthClientDeny();
                    this.SetRemoteEventsText("Device Authorization Denied: " + authRequest.DeviceInfo);
                }
            });
        }

        private void RemoteService_OnNewAccessCode(object sender, AccessCodeNewRemoteMessage e)
        {
            this.AccessCodeTextBlock.Text = ChannelSession.Services.Remote.AccessCode;
        }

        private void RemoteService_OnBoardRequest(object sender, RemoteBoardModel board)
        {
            this.SetRemoteEventsText("Board Requested: " + board.Name);
        }

        private void RemoteService_OnActionRequest(object sender, RemoteCommand command)
        {
            this.SetRemoteEventsText("Command Run: " + command.Name);
        }

        private void SetRemoteEventsText(string text)
        {
            this.Dispatcher.Invoke(() =>
            {
                this.RemoteEventsTextBlock.Text += text + Environment.NewLine;
            });
        }

        private async Task DisconnectRemote()
        {
            await this.Window.RunAsyncOperation(async () =>
            {
                ChannelSession.Services.Remote.OnDisconnectOccurred -= RemoteService_OnDisconnectOccurred;
                ChannelSession.Services.Remote.OnAuthRequest -= RemoteService_OnAuthRequest;
                ChannelSession.Services.Remote.OnNewAccessCode -= RemoteService_OnNewAccessCode;
                ChannelSession.Services.Remote.OnBoardRequest -= RemoteService_OnBoardRequest;
                ChannelSession.Services.Remote.OnActionRequest -= RemoteService_OnActionRequest;

                await ChannelSession.Services.Remote.Disconnect();

                this.ConnectToRemoteGrid.Visibility = Visibility.Collapsed;
                this.SetupGrid.Visibility = Visibility.Visible;
            });
        }

        private void SecretBetaAccess_Click(object sender, RoutedEventArgs e)
        {
            this.InBetaGrid.Visibility = Visibility.Collapsed;
            this.MainGrid.Visibility = Visibility.Visible;
        }

        private async Task<byte[]> ResizeImageToStandardSize(string imageFilePath)
        {
            if (!string.IsNullOrEmpty(imageFilePath) && File.Exists(imageFilePath))
            {
                return await Task.Run(() =>
                {
                    using (MemoryStream inStream = new MemoryStream(File.ReadAllBytes(imageFilePath)))
                    {
                        using (MemoryStream outStream = new MemoryStream())
                        {
                            using (ImageFactory imageFactory = new ImageFactory())
                            {
                                imageFactory
                                    .Load(inStream)
                                    .Resize(new System.Drawing.Size(RemoteItemModel.RequiredSize, RemoteItemModel.RequiredSize))
                                    .Format(new PngFormat())
                                    .Quality(100)
                                    .Save(outStream);

                                return outStream.ToArray();
                            }
                        }
                    }
                });
            }
            return null;
        }
    }
}
