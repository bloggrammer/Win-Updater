using System;
using System.Reflection;
using System.Windows;
using WinUpdate;

namespace DemoApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IWinUpdatable, IContext
    {
        public MainWindow()
        {
            InitializeComponent();
            displayText.Text = ApplicationAssembly.GetName().Version.ToString();

            _updater = new WinUpdater(this);
        }
        public string ApplicationName => Title;

        public string ApplicationId => "DemoApp";

        public Assembly ApplicationAssembly => Assembly.GetExecutingAssembly();

        public Uri UpdateXmlLocation => new Uri("https://bloggrammer.com/update.xml");

        public IContext Context => this;

        public string ApplicationExeFolderPath => AppDomain.CurrentDomain.BaseDirectory;

        private readonly WinUpdater _updater;

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            _updater.DoUpdate();
        }
    }
}
