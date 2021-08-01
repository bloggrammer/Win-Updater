using System.Windows;

namespace WinUpdate.Dialogs
{
    /// <summary>
    /// Dialog to show details about the update
    /// </summary>
    internal partial class WinUpdateInfoDialog : Window
    {
        /// <summary>
        /// Create a new WinUpdateInfo Dialog
        /// </summary>
        /// <param name="applicationInfo"></param>
        /// <param name="updateInfo"></param>
        internal WinUpdateInfoDialog(IWinUpdatable applicationInfo, WinUpdateXml updateInfo)
        {
            InitializeComponent();

            Title = $"{applicationInfo.ApplicationName}-Upadte Info";
            version.Text = $"Current Version: {applicationInfo.ApplicationAssembly.GetName().Version}\nUpdate Version: {updateInfo.Version}";
            description.Text = updateInfo.Description;
        }
        internal bool ShowDialog(IContext owner)
        {
            Owner = (Window)owner;
            return (bool)ShowDialog();
        }

        private void Back_Click(object sender, RoutedEventArgs e) => Close();
    }
}
