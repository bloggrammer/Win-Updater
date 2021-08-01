using System.Windows;

namespace WinUpdate.Dialogs
{
    /// <summary>
    /// Interaction logic for WinUpdateAcceptDialog.xaml
    /// </summary>
    internal partial class WinUpdateAcceptDialog : Window
    {
        internal WinUpdateAcceptDialog(IWinUpdatable applicationInfo, WinUpdateXml updateInfo)
        {
            InitializeComponent();
            Title = $"{applicationInfo.ApplicationName}-Upadte Info";
            newVersion.Text = $"New Version: {updateInfo.Version}";
            _applicationInfo = applicationInfo;
            _updateInfo = updateInfo;
        }

        internal bool ShowDialog(IContext owner)
        {
            Owner = (Window)owner;
            return (bool)ShowDialog();
        }

        private void Yes_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void No_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void Details_Click(object sender, RoutedEventArgs e)
        {
            if (_infoDialog == null)
                _infoDialog = new WinUpdateInfoDialog(_applicationInfo, _updateInfo);
            _infoDialog.ShowDialog(_applicationInfo.Context);
        }
        private WinUpdateInfoDialog _infoDialog;
        private readonly IWinUpdatable _applicationInfo;
        private readonly WinUpdateXml _updateInfo;
    }
}
