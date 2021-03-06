﻿using MixItUp.Base;
using MixItUp.WPF.Util;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Navigation;

namespace MixItUp.WPF.Controls.Services
{
    /// <summary>
    /// Interaction logic for XSplitServiceControl.xaml
    /// </summary>
    public partial class XSplitServiceControl : ServicesControlBase
    {
        public XSplitServiceControl()
        {
            InitializeComponent();
        }

        protected override async Task OnLoaded()
        {
            this.SetHeaderText("XSplit");

            if (ChannelSession.Settings.EnableXSplitConnection)
            {
                this.EnableXSplitConnectionButton.Visibility = Visibility.Collapsed;
                this.DisableXSplitConnectionButton.Visibility = Visibility.Visible;

                this.TestXSplitConnectionButton.IsEnabled = true;

                this.SetCompletedIcon(visible: true);
            }

            await base.OnLoaded();
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start("XSplit");
        }

        private async void EnableXSplitConnectionButton_Click(object sender, RoutedEventArgs e)
        {
            await this.groupBoxControl.window.RunAsyncOperation(async () =>
            {
                if (!await this.ConnectXSplitService())
                {
                    await MessageBoxHelper.ShowMessageDialog("Failed to start XSplit Connection, this sometimes means our connection got wonky. If this continues to happen, please try restarting Mix It Up.");
                }
                await ChannelSession.SaveSettings();
            });
        }

        private async void DisableXSplitConnectionButton_Click(object sender, RoutedEventArgs e)
        {
            await this.groupBoxControl.window.RunAsyncOperation(async () =>
            {
                await this.DisconnectXSplitService();
                await ChannelSession.SaveSettings();
            });
        }

        private async void TestXSplitConnectionButton_Click(object sender, RoutedEventArgs e)
        {
            if (ChannelSession.Services.XSplitServer != null)
            {
                await this.groupBoxControl.window.RunAsyncOperation(async () =>
                {
                    if (await ChannelSession.Services.XSplitServer.TestConnection())
                    {
                        await MessageBoxHelper.ShowMessageDialog("XSplit connection test successful!");
                    }
                    else
                    {
                        await MessageBoxHelper.ShowMessageDialog("XSplit connection test failed, please ensure you have the Mix It Up XSplit extension added and open in XSplit.");
                    }
                });
            }
        }

        public async Task<bool> ConnectXSplitService()
        {
            if (!await ChannelSession.Services.InitializeXSplitServer())
            {
                return false;
            }

            ChannelSession.Settings.EnableXSplitConnection = true;
            this.EnableXSplitConnectionButton.Visibility = Visibility.Collapsed;
            this.DisableXSplitConnectionButton.Visibility = Visibility.Visible;

            this.TestXSplitConnectionButton.IsEnabled = true;

            this.SetCompletedIcon(visible: true);

            return true;
        }

        private async Task DisconnectXSplitService()
        {
            this.EnableXSplitConnectionButton.Visibility = Visibility.Visible;
            this.DisableXSplitConnectionButton.Visibility = Visibility.Collapsed;
            this.TestXSplitConnectionButton.IsEnabled = false;

            await ChannelSession.Services.DisconnectXSplitServer();

            ChannelSession.Settings.EnableXSplitConnection = false;

            this.SetCompletedIcon(visible: false);
        }
    }
}
