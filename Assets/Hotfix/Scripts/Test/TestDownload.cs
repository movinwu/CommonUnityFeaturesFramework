using CommonFeatures.Log;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;
using System.IO;
using UnityEngine;

namespace CommonFeatures.Test
{
    public class TestDownload : MonoBehaviour
    {
        private async void Start()
        {
            string url1 = "http://education.jnaw.top/lubozhibouploaded/quanjing/1615791915263.mp4";
            string url2 = "http://education.jnaw.top/lubozhibouploaded/quanjing/1629796505018.mp4";

            string savePath = Path.Combine(Application.dataPath, "../TestResource/Download");

            var download1 = await CommonFeaturesManager.Download.AddDownload(url1, savePath);
            download1.preDownloadedCompletedLength.ForEachAsync(x =>
            {
                CommonLog.Log($"�����ļ�1, ���� {download1.preDownloadedCompletedLength} / {download1.downloadTotalLength}, �ٷֱ�: {(double)download1.preDownloadedCompletedLength / download1.downloadTotalLength * 100d}%");
            }).Forget();
            var download2 = await CommonFeaturesManager.Download.AddDownload(url2, savePath);
            download2.preDownloadedCompletedLength.ForEachAsync(x =>
            {
                CommonLog.Log($"�����ļ�2, ���� {download2.preDownloadedCompletedLength} / {download2.downloadTotalLength}, �ٷֱ�: {(double)download2.preDownloadedCompletedLength / download2.downloadTotalLength * 100d}%");
            }).Forget();
        }
    }
}
