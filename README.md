![GitHub](https://img.shields.io/github/license/Blogrammer/Win-Updater?logo=github) [![NuGet](https://img.shields.io/nuget/v/Win-Updater)](https://www.nuget.org/packages/Win-Updater/) ![Nuget](https://img.shields.io/nuget/dt/Win-Updater?logo=nuget)

# Windows Auto Updater

A C# library that will allow you to automatically update any .NET application from the server.  

`WinUpdate` is a multi-threaded .NET application updater that won't freeze any Windows applications while checking for updates or downloading them.  

This awesome updater supports: 

- WPF application
- Windows Forms application
- Console application

## Sample Usage

1. Download the library from nugdet package via Visual Studio or CLI
![nuget package](img/nuget-package.PNG)

2. Implement the `IWinUpdatable` and `IContext`
 
 ```c#
        public interface IWinUpdatable
        {
            /// <summary>
            /// The name of your application as you want it displayed on the update window.
            /// </summary>
            string ApplicationName { get; }
            /// <summary>
            /// An identifier string to use to identify your application in the update.xml
            /// Should be the same as your appId in the update.xml
            /// </summary>
            string ApplicationId { get; }
            /// <summary>
            /// The application executable folder path (the folder path where the exe file is placed)
            /// </summary>
            string ApplicationExeFolderPath { get; }   
            /// <summary>
            /// The current assembly
            /// </summary>
            Assembly ApplicationAssembly { get; }
            /// <summary>
            /// The location of the update.xml on the server
            /// </summary>
            Uri UpdateXmlLocation { get; }
            /// <summary>
            /// The program main window.
            /// </summary>
            IContext Context { get; }
        }
   ``` 
   
 ```c#
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

   ``` 
    
3. Create a the application meta data in xml  named "update.xml" 
	```xml
	    <?xml version="1.0"?>
        <winUpdate>
           <update appId="DemoApp">
             <version>1.0.2.</version>         
             <url>https://bloggrammer.com/DemoApp.exe</url>
             <fileName>DemoApp.exe</fileName>
             <md5>b82bbef98e7bb6a9f98b9a7d3724b64b</md5>
             <description>Testing updater</description> 
             <launchArgs></launchArgs>
           </update>
        </winUpdate>

> NB: The MD5 hash is used to ensure the integrity of the exe file that
> is uploaded to the server. The updater watches out for changes in
> version number. If the server exe file version number is higher than
> that of the client, the update from the server is downloaded to the
> client and the older exe file replaced.

4. Update the app version number and in Visual Studio and rebuild the project
 ![Win Updater](img/update-version.PNG)
 5. Upload the updated exe file and the update.xml file to the server. https://bloggrammer.com/update.xml
 6. Run the demo application and click "Check for updates"

#### Acknowledgment
I would love to acknowledge Dan (a.k.a BetterCoder) whose [YouTube video series](https://www.youtube.com/watch?v=325Uxy5nVI0) helped in putting this together.
