using System;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Windows;

namespace WinUpdate.Dialogs
{
    /// <summary>
    /// Interaction logic for WinUpdateDownloadDialog.xaml
    /// </summary>
    internal partial class WinUpdateDownloadDialog : Window
    {
        internal WinUpdateDownloadDialog(Uri location, string md5, string fileName)
        {
            InitializeComponent();

             TempFile = $"{Path.GetTempPath()}{fileName}";
            _md5 = md5;
            _webclient = new WebClient();
            _webclient.DownloadProgressChanged += Webclient_DownloadProgressChanged;
            _webclient.DownloadFileCompleted += Webclient_DownloadFileCompleted;

            _bgWorker = new BackgroundWorker();
            _bgWorker.DoWork += BgWorker_DoWork;
            _bgWorker.RunWorkerCompleted += BgWorker_RunWorkerCompleted;
            try  {
                if (File.Exists(TempFile))
                    File.Delete(TempFile);

                _webclient.DownloadFileAsync(location, TempFile);
            }
            catch
            {

                DialogResult = false;
                Close();
            }
        }
        internal bool ShowDialog(IContext owner)
        {
            Owner =(Window) owner;
            return (bool)ShowDialog();
        }

        private void Webclient_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            pbStatus.Value = e.ProgressPercentage;
            downloadSize.Text = $"Downloaded {FormatBytes(e.BytesReceived, 1, true)} {FormatBytes(e.TotalBytesToReceive, 1, true)}";
        }
        private void Webclient_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            if (e.Error != null || e.Cancelled)
            {
                DialogResult = false;
                Close();
            }
            else
            {
                downloadSize.Text = "Verifying Download...";
                pbStatus.IsIndeterminate = true;
                _bgWorker.RunWorkerAsync(new string[] { TempFile, _md5 });
            }
        }
        private void BgWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            string file = ((string[])e.Argument)[0];
            string updateMD5 = ((string[])e.Argument)[1];

            if (Hasher.HashFile(file, HashType.MD5) != updateMD5)
                e.Result = false;
            else
                e.Result = true;
        }
        private void BgWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            DialogResult = (bool)e.Result;
            Close();
        }
        private void Window_Closed(object sender, EventArgs e)
        {
            if (_webclient.IsBusy)
            {
                _webclient.CancelAsync();
                DialogResult = false;
            }
            if (_bgWorker.IsBusy)
            {
                _bgWorker.CancelAsync();
                DialogResult = false;
            }
        }
        private string FormatBytes(long bytes, int decimalPlaces, bool showByteType)
        {
            double newBytes = bytes;
            string formatString = "{0";
            string byteType = "B";

            if(newBytes>1024 && newBytes < 1048576)
            {
                newBytes /= 1024;
                byteType = "KB";
            }
            else if (newBytes > 1048576 && newBytes < 1073741824)
            {
                newBytes /= 1048576;
                byteType = "MB";
            }
            else 
            {
                newBytes /= 1073741824;
                byteType = "GB";
            }

            if (decimalPlaces > 0)
                formatString += ":0.";
            for (int i = 0; i < decimalPlaces; i++)
                formatString += "0";
            formatString += "}";

            if (showByteType)
                formatString += byteType;
            return string.Format(formatString, newBytes);
        }
        internal string TempFile { get; }
        private readonly WebClient _webclient;
        private readonly BackgroundWorker _bgWorker;
        private readonly string _md5;

        
    }
}
