using CommonFeatures.Log;
using CommonFeatures.NetWork;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace CommonFeatures.Test
{
    public class TestDownload : MonoBehaviour
    {
        private void Start()
        {
            string url1 = "http://education.jnaw.top/lubozhibouploaded/quanjing/1615791915263.mp4";
            string url2 = "http://education.jnaw.top/lubozhibouploaded/quanjing/1629796505018.mp4";

            string savePath = Path.Combine(Application.dataPath, "../TestResource/Download");

            DownloadManager.Instance.AddDownload(url1, savePath,
                onDownloading: handler => CommonLog.Trace($"下载文件1, 进度 {handler.downloadedLength} / {handler.downloadTotalLength}, 百分比: {(double)handler.downloadedLength / handler.downloadTotalLength * 100d}%"));
            DownloadManager.Instance.AddDownload(url2, savePath,
                onDownloading: handler => CommonLog.Trace($"下载文件1, 进度 {handler.downloadedLength} / {handler.downloadTotalLength}, 百分比: {(double)handler.downloadedLength / handler.downloadTotalLength * 100d}%"), 
                onDownloadComplete: null);
        }
    }
}
