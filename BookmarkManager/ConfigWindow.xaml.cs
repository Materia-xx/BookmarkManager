using BookmarkDataProvider;
using BookmarkDtos;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace BookmarkManager
{
    /// <summary>
    /// Interaction logic for Config.xaml
    /// </summary>
    public partial class ConfigWindow : Window
    {
        public ConfigDto config { get; set; }

        public ConfigWindow()
        {
            InitializeComponent();
        }

        public void ShowEditor()
        {
            config = DataProvider.DataStore.Value.Config.Value.GetConfig();
            RenderConfig();
            this.ShowDialog();
        }

        private void RenderConfig()
        {
            txtHotkeyKey.Text = config.HotkeyKey;
            chkHotkeyAlt.IsChecked = config.HotkeyAlt;
            chkHotkeyCtrl.IsChecked = config.HotkeyCtrl;
            chkHotkeyShift.IsChecked = config.HotkeyShift;

            chkHideSearchFormOnLostFocus.IsChecked = config.HideSearchFormOnLostFocus;
            chkHideSearchFormOnSearch.IsChecked = config.HideSearchFormOnSearch;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.Topmost = true;
            CenterWindowOnScreen();
        }

        private void CenterWindowOnScreen()
        {
            double screenWidth = SystemParameters.PrimaryScreenWidth;
            double screenHeight = SystemParameters.PrimaryScreenHeight;
            double windowWidth = this.Width;
            double windowHeight = this.Height;
            this.Left = (screenWidth / 2) - (windowWidth / 2);
            this.Top = (screenHeight / 2) - (windowHeight / 2);
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtHotkeyKey.Text))
            {
                MessageBox.Show("Hotkey is required.");
                return;
            }

            CUDResult result = DataProvider.DataStore.Value.Config.Value.Update(config);
            if (result.Errors.Any())
            {
                MessageBox.Show(result.ErrorsCombined);
            }
            else
            {
                this.Close();
            }
        }

        private void TxtHotkeyKey_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (txtHotkeyKey.Text.Length > 0)
            {
                config.HotkeyKey = txtHotkeyKey.Text.Substring(0, 1);
            }
        }

        private void ChkHotkeyCtrl_CheckChanged(object sender, RoutedEventArgs e)
        {
            config.HotkeyCtrl = chkHotkeyCtrl.IsChecked.Value;
        }

        private void ChkHotkeyShift_CheckChanged(object sender, RoutedEventArgs e)
        {
            config.HotkeyShift = chkHotkeyShift.IsChecked.Value;
        }

        private void ChkHotkeyAlt_CheckChanged(object sender, RoutedEventArgs e)
        {
            config.HotkeyAlt = chkHotkeyAlt.IsChecked.Value;
        }

        private void ChkHideSearchFormOnLostFocus_CheckChanged(object sender, RoutedEventArgs e)
        {
            config.HideSearchFormOnLostFocus = chkHideSearchFormOnLostFocus.IsChecked.Value;
        }

        private void ChkHideSearchFormOnSearch_CheckChanged(object sender, RoutedEventArgs e)
        {
            config.HideSearchFormOnSearch = chkHideSearchFormOnSearch.IsChecked.Value;
        }
    }
}
