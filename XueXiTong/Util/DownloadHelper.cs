﻿//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// This code is licensed under the MIT License (MIT).
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Windows.Networking.BackgroundTransfer;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Windows.Web;

namespace XueXiTong.Util
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class DownloadHelper :  IDisposable
    {

        private List<DownloadOperation> activeDownloads;
        private CancellationTokenSource cts;


        public void Dispose()
        {
            if (cts != null)
            {
                cts.Dispose();
                cts = null;
            }

            GC.SuppressFinalize(this);
        }

 
        // Enumerate the downloads that were going on in the background while the app was closed.
        private async Task DiscoverActiveDownloadsAsync()
        {
            activeDownloads = new List<DownloadOperation>();

            IReadOnlyList<DownloadOperation> downloads = null;
            try
            {
                downloads = await BackgroundDownloader.GetCurrentDownloadsAsync();
            }
            catch (Exception ex)
            {
                if (!IsExceptionHandled("Discovery error", ex))
                {
                    throw;
                }
                return;
            }

            Log("Loading background downloads: " + downloads.Count);

            if (downloads.Count > 0)
            {
                List<Task> tasks = new List<Task>();
                foreach (DownloadOperation download in downloads)
                {
                    Log(String.Format(CultureInfo.CurrentCulture,
                        "Discovered background download: {0}, Status: {1}", download.Guid,
                        download.Progress.Status));

                    // Attach progress and completion handlers.
                    tasks.Add(HandleDownloadAsync(download, false));
                }

                // Don't await HandleDownloadAsync() in the foreach loop since we would attach to the second
                // download only when the first one completed; attach to the third download when the second one
                // completes etc. We want to attach to all downloads immediately.
                // If there are actions that need to be taken once downloads complete, await tasks here, outside
                // the loop.
                await Task.WhenAll(tasks);
            }
        }

        private async void StartDownload(Uri source,string destination,BackgroundTransferPriority priority)
        {


            StorageFile destinationFile;
            try
            {
                StorageFolder picturesLibrary = ApplicationData.Current.LocalFolder;
                destinationFile = await picturesLibrary.CreateFileAsync(
                    destination,
                    CreationCollisionOption.GenerateUniqueName);
            }
            catch (FileNotFoundException ex)
            {
               // rootPage.NotifyUser("Error while creating file: " + ex.Message, NotifyType.ErrorMessage);
                return;
            }

            BackgroundDownloader downloader = new BackgroundDownloader();
            DownloadOperation download = downloader.CreateDownload(source, destinationFile);

            Log(String.Format(CultureInfo.CurrentCulture, "Downloading {0} to {1} with {2} priority, {3}",
                source.AbsoluteUri, destinationFile.Name, priority, download.Guid));

            download.Priority = priority;

            // Attach progress and completion handlers.
            await HandleDownloadAsync(download, true);
        }

     

        private void PauseAll_Click(object sender, RoutedEventArgs e)
        {
            Log("Downloads: " + activeDownloads.Count);

            foreach (DownloadOperation download in activeDownloads)
            {
                // DownloadOperation.Progress is updated in real-time while the operation is ongoing. Therefore,
                // we must make a local copy so that we can have a consistent view of that ever-changing state
                // throughout this method's lifetime.
                BackgroundDownloadProgress currentProgress = download.Progress;

                if (currentProgress.Status == BackgroundTransferStatus.Running)
                {
                    download.Pause();
                    Log("Paused: " + download.Guid);
                }
                else
                {
                    Log(String.Format(CultureInfo.CurrentCulture, "Skipped: {0}, Status: {1}", download.Guid,
                        currentProgress.Status));
                }
            }
        }

        private void ResumeAll_Click(object sender, RoutedEventArgs e)
        {
            Log("Downloads: " + activeDownloads.Count);

            foreach (DownloadOperation download in activeDownloads)
            {
                // DownloadOperation.Progress is updated in real-time while the operation is ongoing. Therefore,
                // we must make a local copy so that we can have a consistent view of that ever-changing state
                // throughout this method's lifetime.
                BackgroundDownloadProgress currentProgress = download.Progress;

                if (currentProgress.Status == BackgroundTransferStatus.PausedByApplication)
                {
                    download.Resume();
                    Log("Resumed: " + download.Guid);
                }
                else
                {
                    Log(String.Format(CultureInfo.CurrentCulture, "Skipped: {0}, Status: {1}", download.Guid,
                        currentProgress.Status));
                }
            }
        }

        private void CancelAll_Click(object sender, RoutedEventArgs e)
        {
            Log("Canceling Downloads: " + activeDownloads.Count);

            cts.Cancel();
            cts.Dispose();

            // Re-create the CancellationTokenSource and activeDownloads for future downloads.
            cts = new CancellationTokenSource();
            activeDownloads = new List<DownloadOperation>();
        }

        // Note that this event is invoked on a background thread, so we cannot access the UI directly.
        private void DownloadProgress(DownloadOperation download)
        {
            // DownloadOperation.Progress is updated in real-time while the operation is ongoing. Therefore,
            // we must make a local copy so that we can have a consistent view of that ever-changing state
            // throughout this method's lifetime.
            BackgroundDownloadProgress currentProgress = download.Progress;

            MarshalLog(String.Format(CultureInfo.CurrentCulture, "Progress: {0}, Status: {1}", download.Guid,
                currentProgress.Status));

            double percent = 100;
            if (currentProgress.TotalBytesToReceive > 0)
            {
                percent = currentProgress.BytesReceived * 100 / currentProgress.TotalBytesToReceive;
            }

            MarshalLog(String.Format(
                CultureInfo.CurrentCulture,
                " - Transferred bytes: {0} of {1}, {2}%",
                currentProgress.BytesReceived,
                currentProgress.TotalBytesToReceive,
                percent));

            if (currentProgress.HasRestarted)
            {
                MarshalLog(" - Download restarted");
            }

            if (currentProgress.HasResponseChanged)
            {
                // We have received new response headers from the server.
                // Be aware that GetResponseInformation() returns null for non-HTTP transfers (e.g., FTP).
                ResponseInformation response = download.GetResponseInformation();
                int headersCount = response != null ? response.Headers.Count : 0;

                MarshalLog(" - Response updated; Header count: " + headersCount);

                // If you want to stream the response data this is a good time to start.
                // download.GetResultStreamAt(0);
            }
        }

        private async Task HandleDownloadAsync(DownloadOperation download, bool start)
        {
            try
            {
               // LogStatus("Running: " + download.Guid, NotifyType.StatusMessage);

                // Store the download so we can pause/resume.
                activeDownloads.Add(download);

                Progress<DownloadOperation> progressCallback = new Progress<DownloadOperation>(DownloadProgress);
                if (start)
                {
                    // Start the download and attach a progress handler.
                    await download.StartAsync().AsTask(cts.Token, progressCallback);
                }
                else
                {
                    // The download was already running when the application started, re-attach the progress handler.
                    await download.AttachAsync().AsTask(cts.Token, progressCallback);
                }

                ResponseInformation response = download.GetResponseInformation();

                // GetResponseInformation() returns null for non-HTTP transfers (e.g., FTP).
                string statusCode = response != null ? response.StatusCode.ToString() : String.Empty;

                //LogStatus(
                //    String.Format(
                //        CultureInfo.CurrentCulture,
                //        "Completed: {0}, Status Code: {1}",
                //        download.Guid,
                //        statusCode),
                //    NotifyType.StatusMessage);
            }
            catch (TaskCanceledException)
            {
                //LogStatus("Canceled: " + download.Guid, NotifyType.StatusMessage);
            }
            catch (Exception ex)
            {
                if (!IsExceptionHandled("Execution error", ex, download))
                {
                    throw;
                }
            }
            finally
            {
                activeDownloads.Remove(download);
            }
        }

        private bool IsExceptionHandled(string title, Exception ex, DownloadOperation download = null)
        {
            WebErrorStatus error = BackgroundTransferError.GetStatus(ex.HResult);
            if (error == WebErrorStatus.Unknown)
            {
                return false;
            }

            if (download == null)
            {
                //LogStatus(String.Format(CultureInfo.CurrentCulture, "Error: {0}: {1}", title, error),
                //    NotifyType.ErrorMessage);
            }
            else
            {
                //LogStatus(String.Format(CultureInfo.CurrentCulture, "Error: {0} - {1}: {2}", download.Guid, title,
                //    error), NotifyType.ErrorMessage);
            }

            return true;
        }

        // When operations happen on a background thread we have to marshal UI updates back to the UI thread.
        private void MarshalLog(string value)
        {
            //var ignore = this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            //{
            //    Log(value);
            //});
        }

        private void Log(string message)
        {
            //outputField.Text += message + "\r\n";
        }

        //private void LogStatus(string message, NotifyType type)
        //{
        // //   rootPage.NotifyUser(message, type);
        //   // Log(message);
        //}
    }
}
