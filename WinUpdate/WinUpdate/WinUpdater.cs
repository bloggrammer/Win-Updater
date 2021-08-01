using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.Hosting;
using System.Threading;
using System.Windows;
using WinUpdate.Dialogs;

namespace WinUpdate
{
    public class WinUpdater
    {
        public WinUpdater(IWinUpdatable applicationInfo)
        {
            _applicationInfo = applicationInfo;
            _bgWorker = new BackgroundWorker();
            _bgWorker.DoWork += BgWorker_DoWork;
            _bgWorker.RunWorkerCompleted += BgWorker_RunWorkerCompleted;
        }

        public void DoUpdate()
        {
            if (!_bgWorker.IsBusy)
                _bgWorker.RunWorkerAsync(_applicationInfo);
        }
        private void BgWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            var application = (IWinUpdatable)e.Argument;
            if (!WinUpdateXml.ExistOnServer(application.UpdateXmlLocation))
                e.Cancel = true;
            else
                e.Result = WinUpdateXml.Parse(application.UpdateXmlLocation, application.ApplicationId);
        }

        private void BgWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (!e.Cancelled)
                if (e.Result is WinUpdateXml update && update.IsNewerThan(_applicationInfo.ApplicationAssembly.GetName().Version))
                    if (new WinUpdateAcceptDialog(_applicationInfo, update).ShowDialog(_applicationInfo.Context))
                        DownloadUpdate(update);
        }

        private void DownloadUpdate(WinUpdateXml update)
        {
            var downloadDialog = new WinUpdateDownloadDialog(update.Uri, update.MD5, update.FileName);
            var result = downloadDialog.ShowDialog(_applicationInfo.Context);
            if (result)
            {
                string currentFile = _applicationInfo.ApplicationAssembly.Location;
                //string currentFile = $"{_applicationInfo.ApplicationExeFolderPath}\\Lytical.DMA.exe";
                string newFile = $"{_applicationInfo.ApplicationExeFolderPath}\\{update.FileName}";

                UpdateApplication(downloadDialog.TempFile, currentFile, newFile, update.LaunchArgs);
                Application.Current.Shutdown();
            }
            else
            {
                MessageBox.Show("The update download was cancelled.\nThe application has not been updated.", "Update download cancelled", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void UpdateApplication(string tempFile, string currentFile, string newFile, string launchArgs)
        {
            string args = "/C Choice /C Y /N /D Y /T 4 & Del /F /Q \"{0}\" & Choice /C Y /N /D Y /T 2 & Move /Y \"{1}\" \"{2}\" & Start \"\" /D \"{3}\" \"{4}\" {5}";
            ProcessStartInfo info = new ProcessStartInfo
            {
                Arguments = string.Format(args, currentFile, tempFile, newFile, Path.GetDirectoryName(newFile), Path.GetFileName(newFile), launchArgs),
                WindowStyle = ProcessWindowStyle.Hidden,
                CreateNoWindow = true,
                FileName = "cmd.exe"
            };
            Process.Start(info);
        }

        private readonly BackgroundWorker _bgWorker;
        private readonly IWinUpdatable _applicationInfo;
    }
}
