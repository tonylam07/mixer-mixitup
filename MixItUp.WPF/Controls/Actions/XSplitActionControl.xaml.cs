using Mixer.Base.Util;
using MixItUp.Base;
using MixItUp.Base.Actions;
using MixItUp.Base.Services;
using MixItUp.WPF.Util;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace MixItUp.WPF.Controls.Actions
{
    /// <summary>
    /// Interaction logic for XSplitActionControl.xaml
    /// </summary>
    public partial class XSplitActionControl : ActionControlBase
    {
        private enum XSplitTypeEnum
        {
            Scene,
            [Name("Source Visibility")]
            SourceVisibility,
            [Name("Text Source")]
            TextSource,
            [Name("Web Browser Source")]
            WebBrowserSource,
            [Name("Source Dimensions")]
            SourceDimensions
        }

        private XSplitAction action;

        public XSplitActionControl(ActionContainerControl containerControl) : base(containerControl) { InitializeComponent(); }

        public XSplitActionControl(ActionContainerControl containerControl, XSplitAction action) : this(containerControl) { this.action = action; }

        public override Task OnLoaded()
        {
            this.XSplitTypeComboBox.ItemsSource = EnumHelper.GetEnumNames<XSplitTypeEnum>();
            if (this.action != null)
            {
                if (!string.IsNullOrEmpty(this.action.SceneName))
                {
                    this.XSplitTypeComboBox.SelectedItem = EnumHelper.GetEnumName(XSplitTypeEnum.Scene);
                    this.XSplitSceneNameTextBox.Text = this.action.SceneName;
                }
                else
                {
                    this.XSplitSourceNameTextBox.Text = this.action.SourceName;
                    this.XSplitSourceVisibleCheckBox.IsChecked = this.action.SourceVisible;
                    if (!string.IsNullOrEmpty(this.action.SourceText))
                    {
                        this.XSplitSourceTextTextBox.Text = this.action.SourceText;
                        this.XSplitSourceLoadTextFromTextBox.Text = this.action.LoadTextFromFilePath;
                        this.XSplitTypeComboBox.SelectedItem = EnumHelper.GetEnumName(XSplitTypeEnum.TextSource);
                    }
                    else if (!string.IsNullOrEmpty(this.action.SourceURL))
                    {
                        this.XSplitSourceWebPageTextBox.Text = this.action.SourceURL;
                        this.XSplitTypeComboBox.SelectedItem = EnumHelper.GetEnumName(XSplitTypeEnum.WebBrowserSource);
                    }
                    else if (this.action.SourceDimensions != null)
                    {
                        this.XSplitSourceDimensionsXPositionTextBox.Text = this.action.SourceDimensions.Left.ToString();
                        this.XSplitSourceDimensionsYPositionTextBox.Text = this.action.SourceDimensions.Top.ToString();
                        this.XSplitSourceDimensionsWidthTextBox.Text = this.action.SourceDimensions.Right.ToString();
                        this.XSplitSourceDimensionsHeightTextBox.Text = this.action.SourceDimensions.Bottom.ToString();
                        this.XSplitSourceDimensionsXRotationTextBox.Text = this.action.SourceDimensions.RotateX.ToString();
                        this.XSplitSourceDimensionsYRotationTextBox.Text = this.action.SourceDimensions.RotateY.ToString();
                        this.XSplitSourceDimensionsZRotationTextBox.Text = this.action.SourceDimensions.RotateZ.ToString();
                        this.XSplitTypeComboBox.SelectedItem = EnumHelper.GetEnumName(XSplitTypeEnum.SourceDimensions);
                    }
                    else
                    {
                        this.XSplitTypeComboBox.SelectedItem = EnumHelper.GetEnumName(XSplitTypeEnum.SourceVisibility);
                    }
                }
            }
            return Task.FromResult(0);
        }

        public override ActionBase GetAction()
        {
            if (this.XSplitTypeComboBox.SelectedIndex >= 0)
            {
                if (this.XSplitSceneGrid.Visibility == Visibility.Visible && !string.IsNullOrEmpty(this.XSplitSceneNameTextBox.Text))
                {
                    return new XSplitAction(this.XSplitSceneNameTextBox.Text);
                }
                else if (this.XSplitSourceGrid.Visibility == Visibility.Visible && !string.IsNullOrEmpty(this.XSplitSourceNameTextBox.Text))
                {
                    if (this.XSplitSourceTextGrid.Visibility == Visibility.Visible)
                    {
                        if (!string.IsNullOrEmpty(this.XSplitSourceTextTextBox.Text))
                        {
                            XSplitAction action = new XSplitAction(this.XSplitSourceNameTextBox.Text, this.XSplitSourceVisibleCheckBox.IsChecked.GetValueOrDefault(), this.XSplitSourceTextTextBox.Text);
                            action.UpdateReferenceTextFile();
                            return action;
                        }
                    }
                    else if (this.XSplitSourceWebBrowserGrid.Visibility == Visibility.Visible)
                    {
                        if (!string.IsNullOrEmpty(this.XSplitSourceWebPageTextBox.Text))
                        {
                            return new XSplitAction(this.XSplitSourceNameTextBox.Text, this.XSplitSourceVisibleCheckBox.IsChecked.GetValueOrDefault(), null, this.XSplitSourceWebPageTextBox.Text);
                        }
                    }
                    else if (this.XSplitSourceDimensionsGrid.Visibility == Visibility.Visible)
                    {
                        float left, top, right, bottom, rotateX, rotateY, rotateZ;
                        if (float.TryParse(this.XSplitSourceDimensionsXPositionTextBox.Text, out left) && float.TryParse(this.XSplitSourceDimensionsYPositionTextBox.Text, out top) &&
                            float.TryParse(this.XSplitSourceDimensionsWidthTextBox.Text, out right) && float.TryParse(this.XSplitSourceDimensionsHeightTextBox.Text, out bottom) &&
                            float.TryParse(this.XSplitSourceDimensionsXRotationTextBox.Text, out rotateX) && float.TryParse(this.XSplitSourceDimensionsYRotationTextBox.Text, out rotateY) &&
                            float.TryParse(this.XSplitSourceDimensionsZRotationTextBox.Text, out rotateZ))
                        {
                            return new XSplitAction(this.XSplitSourceNameTextBox.Text, this.XSplitSourceVisibleCheckBox.IsChecked.GetValueOrDefault(),
                                new XSplitSourceDimensions() { Name = this.XSplitSourceNameTextBox.Text, Left = left, Top = top, Right = right, Bottom = bottom,
                                    RotateX = rotateX, RotateY = rotateY, RotateZ = rotateZ });
                        }
                    }
                    else
                    {
                        return new XSplitAction(this.XSplitSourceNameTextBox.Text, this.XSplitSourceVisibleCheckBox.IsChecked.GetValueOrDefault());
                    }
                }
            }
            return null;
        }

        private void XSplitTypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.XSplitSceneGrid.Visibility = Visibility.Collapsed;
            this.XSplitSourceGrid.Visibility = Visibility.Collapsed;
            this.XSplitSourceTextGrid.Visibility = Visibility.Collapsed;
            this.XSplitSourceWebBrowserGrid.Visibility = Visibility.Collapsed;
            this.XSplitSourceDimensionsGrid.Visibility = Visibility.Collapsed;
            if (this.XSplitTypeComboBox.SelectedIndex >= 0)
            {
                XSplitTypeEnum xsplitType = EnumHelper.GetEnumValueFromString<XSplitTypeEnum>((string)this.XSplitTypeComboBox.SelectedItem);
                if (xsplitType == XSplitTypeEnum.Scene)
                {
                    this.XSplitSceneGrid.Visibility = Visibility.Visible;
                }
                else
                {
                    this.XSplitSourceGrid.Visibility = Visibility.Visible;
                    if (xsplitType == XSplitTypeEnum.TextSource)
                    {
                        this.XSplitSourceTextGrid.Visibility = Visibility.Visible;
                    }
                    else if (xsplitType == XSplitTypeEnum.WebBrowserSource)
                    {
                        this.XSplitSourceWebBrowserGrid.Visibility = Visibility.Visible;
                    }
                    else if (xsplitType == XSplitTypeEnum.SourceDimensions)
                    {
                        this.XSplitSourceDimensionsGrid.Visibility = Visibility.Visible;
                    }
                }
            }
        }

        private void XSplitSourceTextTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!string.IsNullOrEmpty(this.XSplitSourceNameTextBox.Text))
            {
                this.XSplitSourceLoadTextFromTextBox.Text = Path.Combine(XSplitAction.XSplitReferenceTextFilesDirectory, this.XSplitSourceNameTextBox.Text + ".txt");
            }
        }

        private void XSplitSourceWebPageBrowseButton_Click(object sender, RoutedEventArgs e)
        {
            string filePath = ChannelSession.Services.FileService.ShowOpenFileDialog();
            if (!string.IsNullOrEmpty(filePath))
            {
                this.XSplitSourceWebPageTextBox.Text = filePath;
            }
        }

        private async void GetSourcesDimensionsButton_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(this.XSplitSourceNameTextBox.Text))
            {
                await this.containerControl.RunAsyncOperation(async () =>
                {
                    if (ChannelSession.Services.XSplitServer != null || await ChannelSession.Services.InitializeXSplitServer())
                    {
                        XSplitSourceDimensions dimensions = await ChannelSession.Services.XSplitServer.GetSourceDimensions(new XSplitSource() { sourceName = this.XSplitSourceNameTextBox.Text });
                        if (dimensions != null)
                        {
                            this.XSplitSourceDimensionsXPositionTextBox.Text = dimensions.Left.ToString();
                            this.XSplitSourceDimensionsYPositionTextBox.Text = dimensions.Top.ToString();
                            this.XSplitSourceDimensionsWidthTextBox.Text = dimensions.Right.ToString();
                            this.XSplitSourceDimensionsHeightTextBox.Text = dimensions.Bottom.ToString();
                            this.XSplitSourceDimensionsXRotationTextBox.Text = dimensions.RotateX.ToString();
                            this.XSplitSourceDimensionsYRotationTextBox.Text = dimensions.RotateY.ToString();
                            this.XSplitSourceDimensionsZRotationTextBox.Text = dimensions.RotateZ.ToString();
                        }
                        else
                        {
                            await MessageBoxHelper.ShowMessageDialog("Could not find a source with the name specified, please check you entered the name correctly.");
                        }
                    }
                    else
                    {
                        await MessageBoxHelper.ShowMessageDialog("Could not connect to XSplit. Please try establishing connection with it in the Services area.");
                    }
                });
            }
        }
    }
}
