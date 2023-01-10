using BookmarkDataProvider;
using BookmarkDtos;
using System.Linq;
using System.Windows;

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
            chkCloseSearchFormOnLostFocus.IsChecked = config.CloseSearchFormOnLostFocus;
            chkCloseSearchFormOnSearch.IsChecked = config.CloseSearchFormOnSearch;
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

        private void ChkCloseSearchFormOnLostFocus_CheckChanged(object sender, RoutedEventArgs e)
        {
            config.CloseSearchFormOnLostFocus = chkCloseSearchFormOnLostFocus.IsChecked.Value;
        }

        private void ChkCloseSearchFormOnSearch_CheckChanged(object sender, RoutedEventArgs e)
        {
            config.CloseSearchFormOnSearch = chkCloseSearchFormOnSearch.IsChecked.Value;
        }
    }
}
